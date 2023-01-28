using System.Runtime.CompilerServices;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

/// Run with stdin: cd Source && mcs Contest.cs && mono Contest.exe < ../Data/in.txt
public class Contest : SolutionWithFastIO {
	// Src: https://www.hackerearth.com/practice/algorithms/searching/linear-search/practice-problems/algorithm/equal-parity-zeros-25eb4114/?
	protected override void Solve() {
		var T = ReadInt();
		while (T-- > 0) {
			var N = ReadInt();
			var arr = ReadInts(N);

			var sum_diffs = new long[N];
			sum_diffs[0] = arr[0];
			for (var index = 1; index < N; ++index) {
				sum_diffs[index] = sum_diffs[index - 1] + (((index & 1) == 0) ? arr[index] : -arr[index]);
			}
			WriteLine(Solve(arr, 0, N - 1, sum_diffs) ? "YES" : "NO");
		}
	}

	// Each depth level should run at most N actions to archive time-complexity at N * log (N).
	// That is, action-count should be approx with [leftIndex, rightIndex].
	bool Solve(int[] arr, int leftIndex, int rightIndex, long[] sum_diffs) {
		if (leftIndex >= rightIndex) {
			// Mask here
			if (leftIndex == rightIndex) {
				return sum_diffs[arr.Length - 1] - (((leftIndex & 1) == 0) ? 2 * arr[leftIndex] : -2 * arr[leftIndex]) == 0;
			}
			return false;
		}

		// Make tree
		var midIndex = (leftIndex + rightIndex) >> 1;
		var left_result = Solve(arr, leftIndex, midIndex, sum_diffs);
		if (left_result) {
			return true;
		}
		var right_result = Solve(arr, midIndex + 1, rightIndex, sum_diffs);
		if (right_result) {
			return true;
		}

		// Mask in [left -> mid]
		var leftmost_sumdiff = sum_diffs[midIndex];
		var diffs_left = new HashSet<long>();
		for (var index = midIndex; index >= leftIndex; --index) {
			leftmost_sumdiff -= ((index & 1) == 0) ? 2 * arr[index] : -2 * arr[index];
			diffs_left.Add(leftmost_sumdiff);
		}

		// Mask in [mid + 1 -> right]
		var rightmost_sumdiff = sum_diffs[arr.Length - 1] - sum_diffs[midIndex];
		var diffs_right = new HashSet<long>();
		for (var index = midIndex + 1; index <= rightIndex; ++index) {
			rightmost_sumdiff -= ((index & 1) == 0) ? 2 * arr[index] : -2 * arr[index];
			diffs_right.Add(rightmost_sumdiff);
		}

		foreach (var diff in diffs_left) {
			if (diffs_right.Contains(-diff)) {
				return true;
			}
		}

		return false;
	}

	public static void Main(string[] args) {
		new Contest().Start();
	}
}

/// This performs read/write on ASCII bytes which be in range [0, 255].
/// See: https://www.asciitable.com/
/// Ref: /// https://github.com/davidsekar/C-sharp-Programming-IO/blob/master/ConsoleInOut/InputOutput.cs
public abstract class SolutionWithFastIO {
	/// Subclass should perform solution inside this method
	protected abstract void Solve();

	/// White space chars: space, tab, linefeed
	private const int WHITE_SPACE_CODE = 32;
	private const int IN_BUFFER_SIZE = 1 << 13;
	private const int OUT_BUFFER_SIZE = 1 << 13;

	private Stream inStream;
	private Stream outStream;

	private readonly byte[] inBuffer;
	private int nextReadByteIndex;
	private int readByteCount;

	private readonly byte[] outBuffer;
	private int nextWriteByteIndex;

	/// To store bytes for int, long when write
	private readonly byte[] scratchBytes = new byte[32];

	public SolutionWithFastIO() {
		this.inStream = Console.OpenStandardInput();
		this.outStream = Console.OpenStandardOutput();

		// var filePath = Path.GetFullPath("../Data/in.txt");
		// inStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

		this.inBuffer = new byte[IN_BUFFER_SIZE];
		this.outBuffer = new byte[OUT_BUFFER_SIZE];
	}

	protected void Start() {
		this.Solve();

		// Flush out buffer
		this.FlushOutBuffer();

		// Close IO stream
		this.inStream.Close();
		this.outStream.Close();
	}

	public int ReadInt() {
		var num = 0;

		var nextByte = this._ReadNextByteSkipWhitespace();

		// Check negative value
		var isNegative = (nextByte == '-');
		if (isNegative) {
			_TryReadNextByte(out nextByte);
		}

		// Assert digit
		if (nextByte < '0' || nextByte > '9') {
			throw new Exception("Digit expected");
		}

		while (true) {
			num = (num << 1) + (num << 3) + nextByte - '0';
			if (!_TryReadNextByte(out nextByte)) {
				break;
			}
			// Unread if we have read non-digit char
			if (nextByte < '0' || nextByte > '9') {
				_UnreadNextByte();
				break;
			}
		}

		return isNegative ? -num : num;
	}

	public long ReadLong() {
		var num = 0L;

		var nextByte = this._ReadNextByteSkipWhitespace();

		// Check negative value
		var isNegative = (nextByte == '-');
		if (isNegative) {
			_TryReadNextByte(out nextByte);
		}

		// Assert digit
		if (nextByte < '0' || nextByte > '9') {
			throw new Exception("Digit expected");
		}

		while (true) {
			num = (num << 1) + (num << 3) + nextByte - '0';
			if (!_TryReadNextByte(out nextByte)) {
				break;
			}
			// Unread if we have read non-digit char
			if (nextByte < '0' || nextByte > '9') {
				_UnreadNextByte();
				break;
			}
		}

		return isNegative ? -num : num;
	}

	public float ReadFloat() {
		var pre = 0.0f;
		var suf = 0.0f;

		var nextByte = this._ReadNextByteSkipWhitespace();

		// Check negative value
		bool isNegative = (nextByte == '-');
		if (isNegative) {
			_TryReadNextByte(out nextByte);
		}

		// Assert digit
		if (nextByte < '0' || nextByte > '9') {
			throw new Exception("Digit expected");
		}

		var endOfStream = false;
		while (true) {
			pre = 10 * pre + (nextByte - '0');
			if (!_TryReadNextByte(out nextByte)) {
				endOfStream = true;
				break;
			}
			if (nextByte < '0' || nextByte > '9') {
				break;
			}
		}

		if (nextByte == '.') {
			var div = 1.0f;
			while (_TryReadNextByte(out nextByte)) {
				if (nextByte < '0' || nextByte > '9') {
					break;
				}
				suf += (nextByte - '0') / (div *= 10);
			}
		}
		// Unread if we have read some `non-digit` char, and not `dot` char.
		else if (!endOfStream) {
			_UnreadNextByte();
		}

		return isNegative ? -(pre + suf) : (pre + suf);
	}

	public double ReadDouble() {
		var pre = 0.0;
		var suf = 0.0;

		var nextByte = this._ReadNextByteSkipWhitespace();

		// Check negative value
		bool isNegative = (nextByte == '-');
		if (isNegative) {
			_TryReadNextByte(out nextByte);
		}

		// Assert digit
		if (nextByte < '0' || nextByte > '9') {
			throw new Exception("Digit expected");
		}

		var endOfStream = false;
		while (true) {
			pre = 10 * pre + (nextByte - '0');
			if (!_TryReadNextByte(out nextByte)) {
				endOfStream = true;
				break;
			}
			if (nextByte < '0' || nextByte > '9') {
				break;
			}
		}

		if (nextByte == '.') {
			var div = 1.0;
			while (_TryReadNextByte(out nextByte)) {
				if (nextByte < '0' || nextByte > '9') {
					break;
				}
				suf += (nextByte - '0') / (div *= 10);
			}
		}
		// Unread if we have read some `non-digit` char, and not `dot` char.
		else if (!endOfStream) {
			_UnreadNextByte();
		}

		return isNegative ? -(pre + suf) : (pre + suf);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public char ReadChar() {
		return (char)this._ReadNextByteSkipWhitespace();
	}

	public string ReadString() {
		var nextByte = this._ReadNextByteSkipWhitespace();

		var sb = new StringBuilder();
		while (true) {
			sb.Append((char)nextByte);

			if (!_TryReadNextByte(out nextByte) || nextByte <= WHITE_SPACE_CODE) {
				break;
			}
		}

		return sb.ToString();
	}

	public int[] ReadInts(int count) {
		var res = new int[count];
		for (var index = 0; index < count; ++index) {
			res[index] = ReadInt();
		}
		return res;
	}

	public long[] ReadLongs(int count) {
		var res = new long[count];
		for (var index = 0; index < count; ++index) {
			res[index] = ReadLong();
		}
		return res;
	}

	public float[] ReadFloats(int count) {
		var res = new float[count];
		for (var index = 0; index < count; ++index) {
			res[index] = ReadFloat();
		}
		return res;
	}

	public double[] ReadDoubles(int count) {
		var res = new double[count];
		for (var index = 0; index < count; ++index) {
			res[index] = ReadDouble();
		}
		return res;
	}

	public char[] ReadChars(int count) {
		var res = new char[count];
		for (var index = 0; index < count; ++index) {
			res[index] = ReadChar();
		}
		return res;
	}

	public int[] ReadDigits(int count) {
		var res = new int[count];
		for (var index = 0; index < count; ++index) {
			res[index] = ReadChar() - '0';
		}
		return res;
	}

	public string[] ReadStrings(int count) {
		var res = new string[count];
		for (var index = 0; index < count; ++index) {
			res[index] = ReadString();
		}
		return res;
	}

	public int[][] ReadInts(int rowCount, int colCount) {
		var res = new int[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = ReadInts(colCount);
		}
		return res;
	}

	public long[][] ReadLongs(int rowCount, int colCount) {
		var res = new long[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = ReadLongs(colCount);
		}
		return res;
	}

	public float[][] ReadFloats(int rowCount, int colCount) {
		var res = new float[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = ReadFloats(colCount);
		}
		return res;
	}

	public double[][] ReadDoubles(int rowCount, int colCount) {
		var res = new double[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = ReadDoubles(colCount);
		}
		return res;
	}

	public char[][] ReadChars(int rowCount, int colCount) {
		var res = new char[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = ReadChars(colCount);
		}
		return res;
	}

	public string[][] ReadStrings(int rowCount, int colCount) {
		var res = new string[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = ReadStrings(colCount);
		}
		return res;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Write(byte b) {
		if (this.nextWriteByteIndex >= OUT_BUFFER_SIZE) {
			FlushOutBuffer();
		}
		this.outBuffer[this.nextWriteByteIndex++] = b;
	}

	public void Write(int num) {
		// Mirror number to avoid write minus symbol
		var outBuffer = this.outBuffer;
		if (num < 0) {
			if (this.nextWriteByteIndex >= OUT_BUFFER_SIZE) {
				FlushOutBuffer();
			}
			outBuffer[this.nextWriteByteIndex] = (byte)'-';
			num = -num;
		}

		// Convert num to bytes
		var scratchBytes = this.scratchBytes;
		var digitIndex = 0;
		do {
			scratchBytes[digitIndex++] = (byte)((num % 10) + '0');
			num /= 10;
		}
		while (num > 0);

		// Write to buffer
		for (var index = digitIndex - 1; index >= 0; --index) {
			this.Write(scratchBytes[index]);
		}
	}

	public void Write(long num) {
		// Mirror number to avoid write minus symbol
		var outBuffer = this.outBuffer;
		if (num < 0) {
			if (this.nextWriteByteIndex >= OUT_BUFFER_SIZE) {
				FlushOutBuffer();
			}
			outBuffer[this.nextWriteByteIndex] = (byte)'-';
			num = -num;
		}

		// Convert num to bytes
		var scratchBytes = this.scratchBytes;
		var digitIndex = 0;
		do {
			scratchBytes[digitIndex++] = (byte)((num % 10) + '0');
			num /= 10;
		}
		while (num > 0);

		// Write to buffer
		for (var index = digitIndex - 1; index >= 0; --index) {
			this.Write(scratchBytes[index]);
		}
	}

	public void Write(string message) {
		var bytes = Encoding.ASCII.GetBytes(message);
		for (int index = 0, count = bytes.Length; index < count; ++index) {
			this.Write((byte)bytes[index]);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void WriteLine() {
		this.Write((byte)'\n');
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void WriteLine(int num) {
		Write(num);
		Write(((byte)'\n'));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void WriteLine(long num) {
		Write(num);
		Write(((byte)'\n'));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void WriteLine(string message) {
		this.Write(message);
		this.Write((byte)'\n');
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void FlushOutBuffer() {
		this.outStream.Write(this.outBuffer, 0, this.nextWriteByteIndex);
		this.outStream.Flush();
		this.nextWriteByteIndex = 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private byte _ReadNextByteSkipWhitespace() {
		byte nextByte;
		while (_TryReadNextByte(out nextByte)) {
			if (nextByte > WHITE_SPACE_CODE) {
				return nextByte;
			}
		}
		throw new Exception("Cannot read more");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void _UnreadNextByte() {
		if (this.nextReadByteIndex <= 0) {
			throw new Exception("Cannot unread more");
		}
		--this.nextReadByteIndex;
	}

	///
	/// Utilities
	///

	protected static void Debug(string message) {
		Console.WriteLine(message);
	}

	protected long Now() {
		return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}
}
