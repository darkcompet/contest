#pragma warning disable IDE0078 // パターン マッチングを使用します
#pragma warning disable IDE0058 // 式の値が使用されていません

/// <summary>
/// This performs read/write on ASCII chars which be in range [0, 255] (see: https://www.asciitable.com/).
/// TechNotes:
/// - Use hyphen (_) to separate/group long number (but it does not work in mono).
/// - Use new keyword to override base method that does not declare with virtual keyword.
/// </summary>
public abstract class BaseSolution {
	protected readonly bool isDebug;
	protected bool inputFromFile;
	protected bool outputToFile;

	/// <summary>
	/// Subclass can override this method to give the solution.
	/// </summary>
	protected virtual void Solve() { }

	/// White space chars: space, tab, linefeed
	private const int WHITE_SPACE_CODE = 32;
	private const int IN_BUFFER_SIZE = 1 << 14;
	private const int OUT_BUFFER_SIZE = 1 << 14;

	private Stream inStream;
	private Stream outStream;

	private byte[] inBytes;
	private int nextReadByteIndex;
	private int readByteCount;

	private byte[] outBytes;
	private int nextWriteByteIndex;

	/// To store chars of int, long (9223372036854775807) when write to out-buffer
	private readonly byte[] scratchBytes = new byte[19];

	public BaseSolution() {
		// Before dotnet 6, we should use Path.GetFileName() instead.
		// From dotnet 7, we can use Path.Exists() to test the file.
		this.isDebug = this.inputFromFile = System.IO.Path.GetFileName("local.proof") != null;
	}

	private long startTimeMillis;

	protected void Start() {
		if (this.isDebug) {
			this.startTimeMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		}

		// Init IO
		this.inStream = this.inputFromFile ?
			new FileStream(Path.GetFullPath("Data/in.txt"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite) :
			Console.OpenStandardInput();

		this.outStream = this.outputToFile ?
			new FileStream(Path.GetFullPath("Data/out.txt"), FileMode.Open, FileAccess.Write, FileShare.ReadWrite) :
			Console.OpenStandardOutput();

		this.inBytes = new byte[IN_BUFFER_SIZE];
		this.outBytes = new byte[OUT_BUFFER_SIZE];

		this.Solve();

		// Flush out buffer
		this.FlushOutBuffer();

		// Close IO stream
		this.inStream.Close();
		this.outStream.Close();

		if (this.isDebug) {
			Console.WriteLine($"Elapsed: {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - this.startTimeMillis} ms");
		}
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
		this.outBytes[this.nextWriteByteIndex++] = ch;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void print(byte[] arr, int fromIndex, int count) {
		if (this.nextWriteByteIndex + count >= OUT_BUFFER_SIZE) {
			this.FlushOutBuffer();
		}
		Array.Copy(arr, fromIndex, this.outBytes, this.nextWriteByteIndex, count);
		this.nextWriteByteIndex += count;
	}

	public void print(int num) {
		var buffer = this.scratchBytes;
		var curIndex = buffer.Length;
		var isNegative = num < 0;

		do {
			buffer[--curIndex] = (byte)((isNegative ? -(num % 10) : (num % 10)) + '0');
			num /= 10;
		}
		while (num != 0);

		// Write to buffer
		if (isNegative) {
			this.print((byte)'-');
		}
		this.print(buffer, curIndex, buffer.Length - curIndex);
	}

	public void print(long num) {
		var buffer = this.scratchBytes;
		var curIndex = buffer.Length;
		var isNegative = num < 0;

		do {
			buffer[--curIndex] = (byte)((isNegative ? -(num % 10) : (num % 10)) + '0');
			num /= 10;
		}
		while (num != 0);

		// Write to buffer
		if (isNegative) {
			this.print((byte)'-');
		}
		this.print(buffer, curIndex, buffer.Length - curIndex);
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
		this.outStream.Write(this.outBytes, 0, this.nextWriteByteIndex);
		this.outStream.Flush();
		this.nextWriteByteIndex = 0;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	private bool _TryReadNextByte(out byte result) {
		if (this.nextReadByteIndex >= this.readByteCount) {
			this.readByteCount = this.inStream.Read(this.inBytes, 0, IN_BUFFER_SIZE);
			this.nextReadByteIndex = 0;

			if (this.readByteCount <= 0) {
				result = 0;
				return false;
			}
		}
		result = this.inBytes[this.nextReadByteIndex++];
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

	/// <summary>
	/// Utc epoch time in millis.
	/// </summary>
	/// <returns>Elapsed UTC-time from Epoch in milliseconds</returns>
	protected static long Now() {
		return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}

	protected void debug(string text) {
		if (this.isDebug) {
			Console.Write(text);
		}
	}
	protected void debugln(string text) {
		if (this.isDebug) {
			Console.WriteLine(text);
		}
	}

	protected void assert(bool condition, string? message = null) {
		System.Diagnostics.Debug.Assert(condition, message);
	}
}

/// Run: dotnet run
public class Solution : BaseSolution {
	// public static void Main(params string[] args) {
	// 	new Solution().Start();
	// }

	// protected override void Solve() {
	// }

	public class Weight {
		public string name;
		public double mul;
	}
	public class Node {
		public int rootId;
		public string name;
		public double value;
		public List<Weight> adjs = new();
		public bool visited;
	}
	public double[] CalcEquation(IList<IList<string>> equations, double[] values, IList<IList<string>> queries) {
		var N = values.Length;
		var nodes = new Dictionary<string, Node>();
		for (var i = 0; i < N; ++i) {
			var pair = equations[i];
			var node1 = nodes.GetValueOrDefault(pair[0]);
			var node2 = nodes.GetValueOrDefault(pair[1]);
			node1 ??= nodes[pair[0]] = new();
			node2 ??= nodes[pair[1]] = new();
			node1.adjs.Add(new() { name = pair[1], mul = values[i] });
			node2.adjs.Add(new() { name = pair[0], mul = 1 / values[i] });
			node1.name = pair[0];
			node2.name = pair[1];
		}

		var nextRootId = 0;
		foreach (var entry in nodes) {
			var node = entry.Value;
			if (!node.visited) {
				node.visited = true;
				node.value = 1;
				node.rootId = ++nextRootId;
				calcValueForNodes(nodes, node);
			}
		}

		var Q = queries.Count;
		var ans = new double[Q];
		for (var i = 0; i < Q; ++i) {
			var query = queries[i];
			var nodeA = nodes.GetValueOrDefault(query[0]);
			var nodeB = nodes.GetValueOrDefault(query[1]);
			if (nodeA is null || nodeB is null || nodeA.rootId != nodeB.rootId) {
				ans[i] = -1;
			}
			else {
				ans[i] = nodeA.value / nodeB.value;
			}
		}
		return ans;
	}

	private void calcValueForNodes(Dictionary<string, Node> nodes, Node parent) {
		foreach (var adj in parent.adjs) {
			var child = nodes.GetValueOrDefault(adj.name)!;
			if (!child.visited) {
				child.visited = true;
				child.rootId = parent.rootId;
				child.value = parent.value / adj.mul;
				calcValueForNodes(nodes, child);
			}
		}
	}

	// public bool IsPossibleToSplit(int[] nums) {
	// 	var count = new int[101];
	// 	foreach (var num in nums) {
	// 		count[num]++;
	// 	}
	// 	foreach (var cnt in count) {
	// 		if (cnt > 2) {
	// 			return false;
	// 		}
	// 	}
	// 	return true;
	// }

	// public long LargestSquareArea(int[][] bottomLeft, int[][] topRight) {
	// 	var N = bottomLeft.Length;
	// 	var max = 0L;
	// 	var rects = new Rectangle[N];
	// 	for (var i = 0; i < N; ++i) {
	// 		rects[i] = new Rectangle(bottomLeft[i][0], bottomLeft[i][1], topRight[i][0] - bottomLeft[i][0], topRight[i][1] - bottomLeft[i][1]);
	// 	}
	// 	for (var i = 0; i < N; ++i) {
	// 		var rect1 = rects[i];
	// 		for (var j = i + 1; j < N; ++j) {
	// 			var rect2 = rects[j];
	// 			var intersect = Rectangle.Intersect(rect1, rect2);
	// 			if (intersect.Width > 0 && intersect.Height > 0) {
	// 				long d = Math.Min(intersect.Width, intersect.Height);
	// 				max = Math.Max(max, d * d);
	// 			}
	// 		}
	// 	}
	// 	return max;
	// }

	// public class Node {
	// 	public int index;
	// 	public int value;
	// 	public int deadline;
	// }
	// public int EarliestSecondToMarkIndices(int[] nums, int[] changeIndices) {
	// 	var N = nums.Length;
	// 	var M = changeIndices.Length;
	// 	if (M < N) {
	// 		return -1;
	// 	}
	// 	var index2second = new int[N];
	// 	var nodes = new Node[N];
	// 	for (var index = 0; index < N; ++index) {
	// 		nodes[index] = new() { index = index, value = nums[index] };
	// 	}
	// 	Array.Fill(index2second, -1);
	// 	for (var sec = M - 1; sec >= 0; --sec) {
	// 		var numIndex = changeIndices[sec] - 1;
	// 		if (numIndex >= 0 && numIndex < N) {
	// 			if (index2second[numIndex] == -1) {
	// 				index2second[numIndex] = sec;
	// 				nodes[numIndex].deadline = sec;
	// 			}
	// 		}
	// 	}
	// 	foreach (var index in index2second) {
	// 		if (index < 0) {
	// 			return -1;
	// 		}
	// 	}

	// 	Array.Sort(nodes, (a, b) => {
	// 		return a.deadline - b.deadline;
	// 	});

	// 	var elapsedSec = 0;
	// 	for (var index = 0; index < N; ++index) {
	// 		var node = nodes[index];
	// 		elapsedSec += node.value;
	// 		if (elapsedSec > node.deadline) {
	// 			return -1;
	// 		}
	// 	}

	// 	return elapsedSec;
	// }
}
