#pragma warning disable IDE1006 // 命名スタイル
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

	public BaseSolution() {
		// Before dotnet 6, we should use Path.GetFileName() instead.
		// From dotnet 7, we can use Path.Exists() to test the file.
		this.isDebug = System.IO.Path.GetFileName(".compet.local.proof") != null;
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
	// 	var sol = new Solution();
	// 	sol.inputFromFile = sol.isDebug;
	// 	sol.Start();
	// }

	// protected override void Solve() {
	// 	debugln("ans: " + string.Join(", ", BeautifulIndices("isawsquirrelnearmysquirrelhouseohmy", "my", "squirrel", 15)));
	// }

	public IList<int> BeautifulIndices(string s, string a, string b, int k) {
		var indices = new List<int>();
		var N = s.Length;

		var aIndices = this.KMPSearch(s, a);
		var bIndices = this.KMPSearch(s, b);

		bIndices.Sort();
		foreach (var ai in aIndices) {
			// ai - k <= bi <= ai + k
			var bIndex = this.LowerBound(bIndices, ai - k, 0, bIndices.Count - 1);
			if (bIndex >= 0 && bIndices[bIndex] <= ai + k) {
				indices.Add(ai);
			}
		}

		indices.Sort();
		return indices;
	}

	public int LowerBound(List<int> values, int target, int startIndex, int endIndex) {
		if (startIndex < 0 || startIndex > endIndex || endIndex >= values.Count) {
			return -1;
		}
		var midIndex = 0;

		while (startIndex < endIndex) {
			midIndex = (startIndex + endIndex) >> 1;
			if (target <= values[midIndex]) { // Equals means Leftmost
				endIndex = midIndex;
			}
			else {
				startIndex = midIndex + 1;
			}
		}

		// Assert: startIndex == endIndex
		midIndex = startIndex;

		if (midIndex - 1 >= startIndex && values[midIndex - 1] >= target) {
			return midIndex - 1;
		}
		if (values[midIndex] >= target) {
			return midIndex;
		}
		if (midIndex + 1 <= endIndex && values[midIndex + 1] >= target) {
			return midIndex + 1;
		}

		return -1;
	}

	public List<int> KMPSearch(string s, string pattern) {
		var indices = new List<int>();
		var M = pattern.Length;
		var N = s.Length;

		// Create lps[] that will hold the longest prefix suffix values for pattern
		var lps = new int[M];
		// Index for pat[]
		var j = 0;

		// Preprocess the pattern (calculate lps[] array)
		this.ComputeLPSArray(pattern, M, lps);

		// Index for txt[]
		var index = 0;
		while (index < N) {
			if (pattern[j] == s[index]) {
				++j;
				++index;
			}

			if (j == M) {
				// Console.Write("Found pattern " + "at index " + (index - j));
				indices.Add(index - j);
				j = lps[j - 1];
			}
			// Mismatch after j matches
			else if (index < N && pattern[j] != s[index]) {
				// Do not match lps[0..lps[j-1]] characters, they will match anyway
				if (j != 0) {
					j = lps[j - 1];
				}
				else {
					++index;
				}
			}
		}
		return indices;
	}

	private void ComputeLPSArray(string pat, int M, int[] lps) {
		// Length of the previous longest prefix suffix
		var len = 0;
		var index = 1;

		lps[0] = 0;

		// The loop calculates lps[i] for i = 1 to M-1
		while (index < M) {
			if (pat[index] == pat[len]) {
				len++;
				lps[index] = len;
				index++;
			}
			else {
				// This is tricky. Consider the example.
				// AAACAAAA and i = 7. The idea is similar to search step.
				if (len != 0) {
					len = lps[len - 1];
					// Also, note that we do not increment i here
				}
				else {
					lps[index] = len;
					index++;
				}
			}
		}
	}
}
