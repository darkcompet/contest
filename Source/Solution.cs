#pragma warning disable IDE1006 // 命名スタイル
#pragma warning disable IDE0078 // パターン マッチングを使用します

/// This performs read/write on ASCII chars which be in range [0, 255].
/// See: https://www.asciitable.com/
/// TechNotes:
/// - In dotnet, can use hyphen (_) to separate/group long number. But it does not work in mono.
public abstract class SolutionWithFastIO {
	protected virtual bool inputFromFile { get; set; }
	protected virtual bool outputToFile { get; set; }
	protected abstract void Solve();

	/// White space chars: space, tab, linefeed
	private const int WHITE_SPACE_CODE = 32;
	private const int IN_BUFFER_SIZE = 1 << 13;
	private const int OUT_BUFFER_SIZE = 1 << 13;

	private Stream inStream;
	private Stream outStream;

	private byte[] inBuffer;
	private int nextReadByteIndex;
	private int readByteCount;

	private byte[] outChars;
	private int nextWriteByteIndex;

	/// To store bytes of int, long values when write to out-buffer
	private readonly byte[] scratchBytes = new byte[32];

	public SolutionWithFastIO() {
	}

	protected void Start() {
		// Init IO
		this.inStream = this.inputFromFile ?
			new FileStream(Path.GetFullPath("Data/in.txt"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite) :
			Console.OpenStandardInput();

		this.outStream = this.outputToFile ?
			new FileStream(Path.GetFullPath("Data/out.txt"), FileMode.Open, FileAccess.Write, FileShare.ReadWrite) :
			Console.OpenStandardOutput();

		this.inBuffer = new byte[IN_BUFFER_SIZE];
		this.outChars = new byte[OUT_BUFFER_SIZE];

		this.Solve();

		// Flush out buffer
		this.FlushOutBuffer();

		// Close IO stream
		this.inStream.Close();
		this.outStream.Close();
	}

	public int ni() {
		var num = 0;

		var nextChar = this._ReadNextByteSkipWhitespace();

		// Check negative value
		var isNegative = nextChar == '-';
		if (isNegative) {
			_ = this._TryReadNextByte(out nextChar);
		}

		// Assert digit
		if (nextChar < '0' || nextChar > '9') {
			throw new Exception("Digit expected");
		}

		while (true) {
			num = (num << 1) + (num << 3) + nextChar - '0';
			if (!this._TryReadNextByte(out nextChar)) {
				break;
			}
			// Unread if we have read non-digit char
			if (nextChar < '0' || nextChar > '9') {
				this._UnreadNextByte();
				break;
			}
		}

		return isNegative ? -num : num;
	}

	public long nl() {
		var num = 0L;

		var nextChar = this._ReadNextByteSkipWhitespace();

		// Check negative value
		var isNegative = nextChar == '-';
		if (isNegative) {
			_ = this._TryReadNextByte(out nextChar);
		}

		// Assert digit
		if (nextChar < '0' || nextChar > '9') {
			throw new Exception("Digit expected");
		}

		while (true) {
			num = (num << 1) + (num << 3) + nextChar - '0';
			if (!this._TryReadNextByte(out nextChar)) {
				break;
			}
			// Unread if we have read non-digit char
			if (nextChar < '0' || nextChar > '9') {
				this._UnreadNextByte();
				break;
			}
		}

		return isNegative ? -num : num;
	}

	public float nf() {
		var pre = 0.0f;
		var suf = 0.0f;

		var nextChar = this._ReadNextByteSkipWhitespace();

		// Check negative value
		var isNegative = nextChar == '-';
		if (isNegative) {
			_ = this._TryReadNextByte(out nextChar);
		}

		// Assert digit
		if (nextChar < '0' || nextChar > '9') {
			throw new Exception("Digit expected");
		}

		var endOfStream = false;
		while (true) {
			pre = (10 * pre) + (nextChar - '0');
			if (!this._TryReadNextByte(out nextChar)) {
				endOfStream = true;
				break;
			}
			if (nextChar < '0' || nextChar > '9') {
				break;
			}
		}

		if (nextChar == '.') {
			var div = 1.0f;
			while (this._TryReadNextByte(out nextChar)) {
				if (nextChar < '0' || nextChar > '9') {
					break;
				}
				suf += (nextChar - '0') / (div *= 10);
			}
		}
		// Unread if we have read some `non-digit` char, and not `dot` char.
		else if (!endOfStream) {
			this._UnreadNextByte();
		}

		return isNegative ? -(pre + suf) : (pre + suf);
	}

	public double nd() {
		var pre = 0.0;
		var suf = 0.0;

		var nextChar = this._ReadNextByteSkipWhitespace();

		// Check negative value
		var isNegative = nextChar == '-';
		if (isNegative) {
			_ = this._TryReadNextByte(out nextChar);
		}

		// Assert digit
		if (nextChar < '0' || nextChar > '9') {
			throw new Exception("Digit expected");
		}

		var endOfStream = false;
		while (true) {
			pre = (10 * pre) + (nextChar - '0');
			if (!this._TryReadNextByte(out nextChar)) {
				endOfStream = true;
				break;
			}
			if (nextChar < '0' || nextChar > '9') {
				break;
			}
		}

		if (nextChar == '.') {
			var div = 1.0;
			while (this._TryReadNextByte(out nextChar)) {
				if (nextChar < '0' || nextChar > '9') {
					break;
				}
				suf += (nextChar - '0') / (div *= 10);
			}
		}
		// Unread if we have read some `non-digit` char, and not `dot` char.
		else if (!endOfStream) {
			this._UnreadNextByte();
		}

		return isNegative ? -(pre + suf) : (pre + suf);
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public char nc() {
		return (char)this._ReadNextByteSkipWhitespace();
	}

	public string ns() {
		var nextByte = this._ReadNextByteSkipWhitespace();

		var sb = new System.Text.StringBuilder();
		while (true) {
			_ = sb.Append((char)nextByte);

			if (!this._TryReadNextByte(out nextByte) || nextByte <= WHITE_SPACE_CODE) {
				break;
			}
		}

		return sb.ToString();
	}

	public int[] ni(int count) {
		var res = new int[count];
		for (var index = 0; index < count; ++index) {
			res[index] = this.ni();
		}
		return res;
	}

	public long[] nl(int count) {
		var res = new long[count];
		for (var index = 0; index < count; ++index) {
			res[index] = this.nl();
		}
		return res;
	}

	public float[] nf(int count) {
		var res = new float[count];
		for (var index = 0; index < count; ++index) {
			res[index] = this.nf();
		}
		return res;
	}

	public double[] nd(int count) {
		var res = new double[count];
		for (var index = 0; index < count; ++index) {
			res[index] = this.nd();
		}
		return res;
	}

	public char[] nc(int count) {
		var res = new char[count];
		for (var index = 0; index < count; ++index) {
			res[index] = this.nc();
		}
		return res;
	}

	public string[] ns(int count) {
		var res = new string[count];
		for (var index = 0; index < count; ++index) {
			res[index] = this.ns();
		}
		return res;
	}

	public int[][] ni(int rowCount, int colCount) {
		var res = new int[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = this.ni(colCount);
		}
		return res;
	}

	public long[][] nl(int rowCount, int colCount) {
		var res = new long[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = this.nl(colCount);
		}
		return res;
	}

	public float[][] nf(int rowCount, int colCount) {
		var res = new float[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = this.nf(colCount);
		}
		return res;
	}

	public double[][] nd(int rowCount, int colCount) {
		var res = new double[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = this.nd(colCount);
		}
		return res;
	}

	public char[][] nc(int rowCount, int colCount) {
		var res = new char[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = this.nc(colCount);
		}
		return res;
	}

	public string[][] ns(int rowCount, int colCount) {
		var res = new string[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = this.ns(colCount);
		}
		return res;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void print(char ch) {
		this.print((byte)ch);
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void print(byte ch) {
		if (this.nextWriteByteIndex >= OUT_BUFFER_SIZE) {
			this.FlushOutBuffer();
		}
		this.outChars[this.nextWriteByteIndex++] = ch;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void print(byte[] arr, int fromIndex, int count) {
		if (this.nextWriteByteIndex + count >= OUT_BUFFER_SIZE) {
			this.FlushOutBuffer();
		}
		Array.Copy(arr, fromIndex, this.outChars, this.nextWriteByteIndex, count);
		this.nextWriteByteIndex += count;
	}

	public void print(int num) {
		var tmpArr = this.scratchBytes;
		var curIndex = tmpArr.Length;
		var isNegative = num < 0;

		do {
			tmpArr[--curIndex] = (byte)((isNegative ? -(num % 10) : (num % 10)) + '0');
			num /= 10;
		}
		while (num != 0);

		// Write to buffer
		if (isNegative) {
			this.print((byte)'-');
		}
		this.print(tmpArr, curIndex, tmpArr.Length - curIndex);
	}

	public void print(long num) {
		var tmpArr = this.scratchBytes;
		var curIndex = tmpArr.Length;
		var isNegative = num < 0;

		do {
			tmpArr[--curIndex] = (byte)((isNegative ? -(num % 10) : (num % 10)) + '0');
			num /= 10;
		}
		while (num != 0);

		// Write to buffer
		if (isNegative) {
			this.print((byte)'-');
		}
		this.print(tmpArr, curIndex, tmpArr.Length - curIndex);
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void print(string text) {
		var arr = System.Text.Encoding.ASCII.GetBytes(text);
		this.print(arr, 0, arr.Length);
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void println() {
		this.print((byte)'\n');
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void println(int num) {
		this.print(num);
		this.print((byte)'\n');
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void println(long num) {
		this.print(num);
		this.print((byte)'\n');
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void println(string text) {
		this.print(text);
		this.print((byte)'\n');
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	protected void FlushOutBuffer() {
		this.outStream.Write(this.outChars, 0, this.nextWriteByteIndex);
		this.outStream.Flush();
		this.nextWriteByteIndex = 0;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	private bool _TryReadNextByte(out byte result) {
		if (this.nextReadByteIndex >= this.readByteCount) {
			this.readByteCount = this.inStream.Read(this.inBuffer, 0, IN_BUFFER_SIZE);
			this.nextReadByteIndex = 0;

			if (this.readByteCount <= 0) {
				result = 0;
				return false;
			}
		}
		result = this.inBuffer[this.nextReadByteIndex++];
		return true;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	private byte _ReadNextByteSkipWhitespace() {
		while (this._TryReadNextByte(out var nextByte)) {
			if (nextByte > WHITE_SPACE_CODE) {
				return nextByte;
			}
		}
		throw new Exception("Cannot read more");
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	private void _UnreadNextByte() {
		if (this.nextReadByteIndex-- <= 0) {
			throw new Exception("Cannot unread more");
		}
	}

	/// Utc epoch time in millis.
	protected static long Now() {
		return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}
}

/// dotnet run
/// mcs ./Source/Solution.cs && mono ./Source/Solution.exe < ./Source/in.txt
public class Solution : SolutionWithFastIO {
	// protected override bool inputFromFile => true;
	// protected override bool outputToFile => true;

	// // public static void Main(string[] args) {
	// // 	new Solution().Start();
	// // }

	protected override void Solve() {
	}

	public int[] LexicographicallySmallestArray(int[] nums, int limit) {
		// foreach (var item in nums) {
		// 	Console.Write(item + " ");
		// }
		// Console.WriteLine();

		var N = nums.Length;

		var nodes = new List<Node>(N);
		for (var index = 0; index < N; ++index) {
			nodes.Add(new Node() {
				value = nums[index],
				originIndex = index
			});
		}
		nodes.Sort((a, b) => { return a.value - b.value; });

		for (var index = 0; index < N; ++index) {
			var target = nums[index] - limit;
			var swapNode = FindLeftNode(nodes, index, target);
			if (swapNode is null) {
				continue;
			}
			nodes.Remove(swapNode);
		}

		// foreach (var item in nums) {
		// 	Console.Write(item + " ");
		// }
		// Console.WriteLine();

		return nums;
	}

	private Node? FindLeftNode(List<Node> nodes, int index, int target) {
		var targetIndex = FindLeftmostIndex(nodes, target, 0, nodes.Count - 1);
		if (targetIndex < 0 || nodes[index].value < nodes[targetIndex].value) {
			return null;
		}
		return nodes[targetIndex];
	}

	public int FindLeftmostIndex(List<Node> nodes, int target, int _startIndex, int _endIndex) {
		var startIndex = _startIndex;
		var endIndex = _endIndex;
		var midIndex = 0;

		while (startIndex < endIndex) {
			midIndex = (startIndex + endIndex) >> 1;
			if (target <= nodes[midIndex].value) { // Equals means Leftmost
				endIndex = midIndex;
			}
			else {
				startIndex = midIndex + 1;
			}
		}

		// Assert: startIndex == endIndex
		midIndex = startIndex;

		if (midIndex - 1 >= _startIndex && nodes[midIndex - 1].value >= target) {
			return midIndex - 1;
		}
		if (nodes[midIndex].value >= target) {
			return midIndex;
		}
		if (midIndex + 1 <= _endIndex && nodes[midIndex + 1].value >= target) {
			return midIndex + 1;
		}

		return -1;
	}

	public class Node {
		public int value;
		public int originIndex;
		public bool used;
	}
}
