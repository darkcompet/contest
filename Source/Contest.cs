using System.Runtime.CompilerServices;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

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

	public int ni() {
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

	public long nl() {
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

	public float nf() {
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

	public double nd() {
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
	public char nc() {
		return (char)this._ReadNextByteSkipWhitespace();
	}

	public string ns() {
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

	public int[] ni(int count) {
		var res = new int[count];
		for (var index = 0; index < count; ++index) {
			res[index] = ni();
		}
		return res;
	}

	public long[] nl(int count) {
		var res = new long[count];
		for (var index = 0; index < count; ++index) {
			res[index] = nl();
		}
		return res;
	}

	public float[] nf(int count) {
		var res = new float[count];
		for (var index = 0; index < count; ++index) {
			res[index] = nf();
		}
		return res;
	}

	public double[] nd(int count) {
		var res = new double[count];
		for (var index = 0; index < count; ++index) {
			res[index] = nd();
		}
		return res;
	}

	public char[] nc(int count) {
		var res = new char[count];
		for (var index = 0; index < count; ++index) {
			res[index] = nc();
		}
		return res;
	}

	public string[] ns(int count) {
		var res = new string[count];
		for (var index = 0; index < count; ++index) {
			res[index] = ns();
		}
		return res;
	}

	public int[][] ni(int rowCount, int colCount) {
		var res = new int[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = ni(colCount);
		}
		return res;
	}

	public long[][] nl(int rowCount, int colCount) {
		var res = new long[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = nl(colCount);
		}
		return res;
	}

	public float[][] nf(int rowCount, int colCount) {
		var res = new float[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = nf(colCount);
		}
		return res;
	}

	public double[][] nd(int rowCount, int colCount) {
		var res = new double[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = nd(colCount);
		}
		return res;
	}

	public char[][] nc(int rowCount, int colCount) {
		var res = new char[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = nc(colCount);
		}
		return res;
	}

	public string[][] ns(int rowCount, int colCount) {
		var res = new string[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = ns(colCount);
		}
		return res;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void print(char ch) {
		this.print((byte)ch);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void print(byte ch) {
		if (this.nextWriteByteIndex >= OUT_BUFFER_SIZE) {
			FlushOutBuffer();
		}
		this.outChars[this.nextWriteByteIndex++] = ch;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void print(byte[] arr, int fromIndex, int count) {
		if (this.nextWriteByteIndex + count >= OUT_BUFFER_SIZE) {
			FlushOutBuffer();
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void print(string text) {
		var arr = Encoding.ASCII.GetBytes(text);
		this.print(arr, 0, arr.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void println() {
		this.print((byte)'\n');
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void println(int num) {
		print(num);
		print(((byte)'\n'));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void println(long num) {
		print(num);
		print(((byte)'\n'));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void println(string text) {
		this.print(text);
		this.print((byte)'\n');
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

	/// Utc epoch time in millis.
	protected static long Now() {
		return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}
}

/// mcs ./Source/Contest.cs && mono ./Source/Contest.exe < ./Source/in.txt
public class Contest : SolutionWithFastIO {
	// protected override bool inputFromFile => true;
	// protected override bool outputToFile => true;

	public static void Main(string[] args) {
		new Contest().Start();
	}

	/// C++: https://atcoder.jp/contests/abc310/submissions/43606683
	/// C#: https://atcoder.jp/contests/abc310/submissions/43585432
	protected override void Solve() {
	}
}
