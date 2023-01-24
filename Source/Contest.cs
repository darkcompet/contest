using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

/// Run: cd Source; mcs Contest.cs; mono Contest.exe < ../Data/in.txt
public class Contest : SolutionWithFastIO {
	protected override void Solve() {
	}

	public static void Main(string[] args) {
		new Contest().Start();
	}
}

/// This read, write ascii bytes which be in range [0, 255].
/// See: https://www.asciitable.com/
/// Ref: /// https://github.com/davidsekar/C-sharp-Programming-IO/blob/master/ConsoleInOut/InputOutput.cs
public abstract class SolutionWithFastIO {
	/// Subclass should perform solution inside this method
	protected abstract void Solve();

	/// White space chars: space, tab, linefeed
	private const int WHITE_SPACE = 32;
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

	public void SetFilePath(string filePath) {
		// filePath = Path.GetFullPath(filePath);
		inStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
	}

	public int ReadInt() {
		var num = 0;

		var nextByte = this.ReadNextByteSkipWhitespace();

		// Check negative value
		var isNegative = (nextByte == '-');
		if (isNegative) {
			TryReadNextByte(out nextByte);
		}

		// Assert digit
		if (nextByte < '0' || nextByte > '9') {
			throw new Exception("Digit expected");
		}

		while (true) {
			num = (num << 1) + (num << 3) + nextByte - '0';
			if (!TryReadNextByte(out nextByte)) {
				break;
			}
			// Unread if we have read non-digit char
			if (nextByte < '0' || nextByte > '9') {
				UnreadNextByte();
				break;
			}
		}

		return isNegative ? -num : num;
	}

	public long ReadLong() {
		var num = 0L;

		var nextByte = this.ReadNextByteSkipWhitespace();

		// Check negative value
		var isNegative = (nextByte == '-');
		if (isNegative) {
			TryReadNextByte(out nextByte);
		}

		// Assert digit
		if (nextByte < '0' || nextByte > '9') {
			throw new Exception("Digit expected");
		}

		while (true) {
			num = (num << 1) + (num << 3) + nextByte - '0';
			if (!TryReadNextByte(out nextByte)) {
				break;
			}
			// Unread if we have read non-digit char
			if (nextByte < '0' || nextByte > '9') {
				UnreadNextByte();
				break;
			}
		}

		return isNegative ? -num : num;
	}

	public float ReadFloat() {
		var pre = 0.0f;
		var suf = 0.0f;

		var nextByte = this.ReadNextByteSkipWhitespace();

		// Check negative value
		bool isNegative = (nextByte == '-');
		if (isNegative) {
			TryReadNextByte(out nextByte);
		}

		// Assert digit
		if (nextByte < '0' || nextByte > '9') {
			throw new Exception("Digit expected");
		}

		var endOfStream = false;
		while (true) {
			pre = 10 * pre + (nextByte - '0');
			if (!TryReadNextByte(out nextByte)) {
				endOfStream = true;
				break;
			}
			if (nextByte < '0' || nextByte > '9') {
				break;
			}
		}

		if (nextByte == '.') {
			var div = 1.0f;
			while (TryReadNextByte(out nextByte)) {
				if (nextByte < '0' || nextByte > '9') {
					break;
				}
				suf += (nextByte - '0') / (div *= 10);
			}
		}
		// Unread if we have read some `non-digit` char, and not `dot` char.
		else if (!endOfStream) {
			UnreadNextByte();
		}

		return isNegative ? -(pre + suf) : (pre + suf);
	}

	public double ReadDouble() {
		var pre = 0.0;
		var suf = 0.0;

		var nextByte = this.ReadNextByteSkipWhitespace();

		// Check negative value
		bool isNegative = (nextByte == '-');
		if (isNegative) {
			TryReadNextByte(out nextByte);
		}

		// Assert digit
		if (nextByte < '0' || nextByte > '9') {
			throw new Exception("Digit expected");
		}

		var endOfStream = false;
		while (true) {
			pre = 10 * pre + (nextByte - '0');
			if (!TryReadNextByte(out nextByte)) {
				endOfStream = true;
				break;
			}
			if (nextByte < '0' || nextByte > '9') {
				break;
			}
		}

		if (nextByte == '.') {
			var div = 1.0;
			while (TryReadNextByte(out nextByte)) {
				if (nextByte < '0' || nextByte > '9') {
					break;
				}
				suf += (nextByte - '0') / (div *= 10);
			}
		}
		// Unread if we have read some `non-digit` char, and not `dot` char.
		else if (!endOfStream) {
			UnreadNextByte();
		}

		return isNegative ? -(pre + suf) : (pre + suf);
	}

	public char ReadChar() {
		return (char)this.ReadNextByteSkipWhitespace();
	}

	public string ReadString() {
		var nextByte = this.ReadNextByteSkipWhitespace();

		var sb = new StringBuilder();
		while (true) {
			sb.Append((char)nextByte);

			if (!TryReadNextByte(out nextByte) || nextByte <= WHITE_SPACE) {
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
			this.WriteByteToOutBuffer(scratchBytes[index]);
		}
	}

	public void WriteLine(int num) {
		Write(num);
		Write("\n");
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
			this.WriteByteToOutBuffer(scratchBytes[index]);
		}
	}

	public void WriteLine(long num) {
		Write(num);
		Write("\n");
	}

	public void Write(string message) {
		var bytes = Encoding.ASCII.GetBytes(message);
		for (int index = 0, count = bytes.Length; index < count; ++index) {
			this.WriteByteToOutBuffer((byte)bytes[index]);
		}
	}

	public void WriteLine() {
		this.WriteByteToOutBuffer((byte)'\n');
	}

	public void WriteLine(string message) {
		this.Write(message);
		this.WriteByteToOutBuffer((byte)'\n');
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void WriteByteToOutBuffer(byte b) {
		if (this.nextWriteByteIndex >= OUT_BUFFER_SIZE) {
			FlushOutBuffer();
		}
		this.outBuffer[this.nextWriteByteIndex++] = b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void FlushOutBuffer() {
		this.outStream.Write(this.outBuffer, 0, this.nextWriteByteIndex);
		this.outStream.Flush();
		this.nextWriteByteIndex = 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool TryReadNextByte(out byte result) {
		var inBuffer = this.inBuffer;
		if (this.nextReadByteIndex >= this.readByteCount) {
			this.readByteCount = this.inStream.Read(inBuffer, 0, IN_BUFFER_SIZE);
			this.nextReadByteIndex = 0;

			if (this.readByteCount <= 0) {
				result = 0;
				return false;
			}
		}
		result = inBuffer[this.nextReadByteIndex++];
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private byte ReadNextByteSkipWhitespace() {
		byte nextByte;
		while (TryReadNextByte(out nextByte)) {
			if (nextByte > WHITE_SPACE) {
				return nextByte;
			}
		}
		throw new Exception("Cannot read more");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void UnreadNextByte() {
		if (this.nextReadByteIndex <= 0) {
			throw new Exception("Cannot unread more");
		}
		--this.nextReadByteIndex;
	}

	protected static void Debug(string message) {
		Console.WriteLine(message);
	}

	protected long Now() {
		return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}
}
