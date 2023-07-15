using System.Runtime.CompilerServices;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

/// This performs read/write on ASCII chars which be in range [0, 255].
/// See: https://www.asciitable.com/

/// TechNotes:
/// - In dotnet, can use hyphen (_) to separate/group long number. But it does not word in mono.
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

	private readonly byte[] inBuffer;
	private int nextReadByteIndex;
	private int readByteCount;

	private readonly byte[] outChars;
	private int nextWriteByteIndex;

	/// To store bytes for int, long when write
	private readonly byte[] scratchBytes = new byte[32];

	public SolutionWithFastIO() {
		this.inStream = this.inputFromFile ?
			new FileStream(Path.GetFullPath("Data/in.txt"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite) :
			Console.OpenStandardInput();

		this.outStream = this.outputToFile ?
			new FileStream(Path.GetFullPath("Data/out.txt"), FileMode.Open, FileAccess.Write, FileShare.ReadWrite) :
			Console.OpenStandardOutput();

		this.inBuffer = new byte[IN_BUFFER_SIZE];
		this.outChars = new byte[OUT_BUFFER_SIZE];
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

		var nextChar = this._ReadNextByteSkipWhitespace();

		// Check negative value
		var isNegative = (nextChar == '-');
		if (isNegative) {
			_TryReadNextByte(out nextChar);
		}

		// Assert digit
		if (nextChar < '0' || nextChar > '9') {
			throw new Exception("Digit expected");
		}

		while (true) {
			num = (num << 1) + (num << 3) + nextChar - '0';
			if (!_TryReadNextByte(out nextChar)) {
				break;
			}
			// Unread if we have read non-digit char
			if (nextChar < '0' || nextChar > '9') {
				_UnreadNextByte();
				break;
			}
		}

		return isNegative ? -num : num;
	}

	public long ReadLong() {
		var num = 0L;

		var nextChar = this._ReadNextByteSkipWhitespace();

		// Check negative value
		var isNegative = (nextChar == '-');
		if (isNegative) {
			_TryReadNextByte(out nextChar);
		}

		// Assert digit
		if (nextChar < '0' || nextChar > '9') {
			throw new Exception("Digit expected");
		}

		while (true) {
			num = (num << 1) + (num << 3) + nextChar - '0';
			if (!_TryReadNextByte(out nextChar)) {
				break;
			}
			// Unread if we have read non-digit char
			if (nextChar < '0' || nextChar > '9') {
				_UnreadNextByte();
				break;
			}
		}

		return isNegative ? -num : num;
	}

	public float ReadFloat() {
		var pre = 0.0f;
		var suf = 0.0f;

		var nextChar = this._ReadNextByteSkipWhitespace();

		// Check negative value
		bool isNegative = (nextChar == '-');
		if (isNegative) {
			_TryReadNextByte(out nextChar);
		}

		// Assert digit
		if (nextChar < '0' || nextChar > '9') {
			throw new Exception("Digit expected");
		}

		var endOfStream = false;
		while (true) {
			pre = 10 * pre + (nextChar - '0');
			if (!_TryReadNextByte(out nextChar)) {
				endOfStream = true;
				break;
			}
			if (nextChar < '0' || nextChar > '9') {
				break;
			}
		}

		if (nextChar == '.') {
			var div = 1.0f;
			while (_TryReadNextByte(out nextChar)) {
				if (nextChar < '0' || nextChar > '9') {
					break;
				}
				suf += (nextChar - '0') / (div *= 10);
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

		var nextChar = this._ReadNextByteSkipWhitespace();

		// Check negative value
		bool isNegative = (nextChar == '-');
		if (isNegative) {
			_TryReadNextByte(out nextChar);
		}

		// Assert digit
		if (nextChar < '0' || nextChar > '9') {
			throw new Exception("Digit expected");
		}

		var endOfStream = false;
		while (true) {
			pre = 10 * pre + (nextChar - '0');
			if (!_TryReadNextByte(out nextChar)) {
				endOfStream = true;
				break;
			}
			if (nextChar < '0' || nextChar > '9') {
				break;
			}
		}

		if (nextChar == '.') {
			var div = 1.0;
			while (_TryReadNextByte(out nextChar)) {
				if (nextChar < '0' || nextChar > '9') {
					break;
				}
				suf += (nextChar - '0') / (div *= 10);
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
	public void Write(char ch) {
		this.Write((byte)ch);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Write(byte ch) {
		if (this.nextWriteByteIndex >= OUT_BUFFER_SIZE) {
			FlushOutBuffer();
		}
		this.outChars[this.nextWriteByteIndex++] = ch;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Write(byte[] arr, int fromIndex, int count) {
		if (this.nextWriteByteIndex + count >= OUT_BUFFER_SIZE) {
			FlushOutBuffer();
		}
		Array.Copy(arr, fromIndex, this.outChars, this.nextWriteByteIndex, count);
		this.nextWriteByteIndex += count;
	}

	public void Write(int num) {
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
			this.Write((byte)'-');
		}
		this.Write(tmpArr, curIndex, tmpArr.Length - curIndex);
	}

	public void Write(long num) {
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
			this.Write((byte)'-');
		}
		this.Write(tmpArr, curIndex, tmpArr.Length - curIndex);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Write(string text) {
		var arr = Encoding.ASCII.GetBytes(text);
		this.Write(arr, 0, arr.Length);
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
	public void WriteLine(string text) {
		this.Write(text);
		this.Write((byte)'\n');
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void FlushOutBuffer() {
		this.outStream.Write(this.outChars, 0, this.nextWriteByteIndex);
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
		if (this.nextReadByteIndex-- <= 0) {
			throw new Exception("Cannot unread more");
		}
	}

	///
	/// Utilities
	///

	/// Utc epoch time.
	protected static long Now() {
		return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}
}

/// Run with stdin: cd Source && mcs Contest.cs && mono Contest.exe < ../Data/in.txt
public class Contest : SolutionWithFastIO {
	// protected override bool inputFromFile => true;
	// protected override bool outputToFile => true;

	public static void Main(string[] args) {
		new Contest().Start();
	}

	protected override void Solve() {
		var arr = new int[] { 3, 2, 1 };
		WriteLine(CountInversions(arr));
	}

	int CountInversions(int[] arr) {
		return CountInversions(arr, 0, arr.Length - 1);
	}

	int CountInversions(int[] arr, int startIndex, int endIndex) {
		if (startIndex >= endIndex) {
			return 0;
		}

		// Divide and Conquer (on independent subarray)
		var midIndex = (startIndex + endIndex) >> 1;
		var leftResult = CountInversions(arr, startIndex, midIndex);
		var rightResult = CountInversions(arr, midIndex + 1, endIndex);

		return leftResult + rightResult + binaryResult(arr, startIndex, midIndex, midIndex + 1, endIndex);
	}

	private int binaryResult(int[] arr, int start1, int end1, int start2, int end2) {
		Array.Sort(arr, start2, end2 - start2 + 1);

		var ans = 0;
		for (var index = start1; index <= end1; ++index) {
			var smallerIndex = findIndexOfSmaller(arr, arr[index], start2, end2);
			if (smallerIndex >= 0) {
				ans += smallerIndex - start2 + 1;
			}
		}

		return ans;
	}

	int findIndexOfSmaller(int[] arr, int value, int _startIndex, int _endIndex) {
		var startIndex = _startIndex;
		var endIndex = _endIndex;
		var midIndex = 0;

		while (startIndex < endIndex) {
			midIndex = (startIndex + endIndex) >> 1;

			if (value <= arr[midIndex]) {
				endIndex = midIndex;
			}
			else {
				startIndex = midIndex + 1;
			}
		}

		if (startIndex != endIndex) {
			Console.WriteLine("---> Oops");
		}

		midIndex = (startIndex + endIndex) >> 1;

		if (midIndex + 1 <= _endIndex && value > arr[midIndex + 1]) {
			return midIndex + 1;
		}
		if (value > arr[midIndex]) {
			return midIndex;
		}
		if (midIndex - 1 >= _startIndex && value > arr[midIndex - 1]) {
			return midIndex - 1;
		}

		return -1;
	}
}
