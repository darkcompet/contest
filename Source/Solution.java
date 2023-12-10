package Source;
import java.io.*;

// javac Contest.java; java Contest debug:1 localInput:0 localOutput:0
public class Solution {
	private InputStream in = System.in;
	private OutputStream out = System.out;
	private PrintStream err = System.err;
	protected static final int MOD = (int) 1E9 + 7;

	private static final int BUFFER_SIZE = (1 << 13);
	private static final int WHITE_SPACE = 32; // space, tab, linefeed
	private final byte[] inBuffer = new byte[BUFFER_SIZE];
	private final byte[] outBuffer = new byte[BUFFER_SIZE];

	private int inNextByte;
	private int inNextIndex;
	private int inReadByteCount;
	private int outNextIndex;

	protected void startWithAsciiIO(String[] args) throws Exception {
		String ls = System.lineSeparator();
		boolean showDebugTrace = false;
		boolean inputFromLocalFile = false;
		boolean outputToLocalFile = false;

		if (args != null) {
			for (String arg : args) {
				String[] arr = arg.split(":");
				if (arr.length == 2) {
					arr[0] = arr[0].toLowerCase();
					arr[1] = arr[1].toLowerCase();
					if ("debug".equals(arr[0])) {
						showDebugTrace = "1".equals(arr[1]) || "true".equals(arr[1]);
					}
					if ("localInput".equals(arr[0])) {
						inputFromLocalFile = "1".equals(arr[1]) || "true".equals(arr[1]);
					}
					if ("localOutput".equals(arr[0])) {
						outputToLocalFile = "1".equals(arr[1]) || "true".equals(arr[1]);
					}
				}
			}
		}

		if (inputFromLocalFile) {
			String fs = File.separator;
			String root = new File("").getAbsolutePath();
			String inPath = root + fs + "testdata" + fs + "in.txt";

			if (showDebugTrace) {
				err.println("Input: " + inPath);
			}

			this.in = new FileInputStream(inPath);
		} else if (showDebugTrace) {
			err.println("Input: Console");
		}

		if (outputToLocalFile) {
			String fs = File.separator;
			String root = new File("").getAbsolutePath();
			String outPath = root + fs + "testdata" + fs + "in.txt";

			if (showDebugTrace) {
				err.println("Output: " + outPath);
			}

			this.out = new FileOutputStream(outPath);
		} else if (showDebugTrace) {
			err.println("Output: Console");
		}

		long start = 0;
		if (showDebugTrace) {
			start = System.currentTimeMillis();
		}

		if (showDebugTrace) {
			err.printf("%sSolve completed in %.3f [s]%s", ls, (System.currentTimeMillis() - start) / 1000.0, ls);
		}

		this.nextByte();
		this.solve();
		this.in.close();
		this.flushOutBuf();
	}

	protected int nextByte() throws IOException {
		if (inNextIndex >= inReadByteCount) {
			inReadByteCount = in.read(inBuffer, 0, BUFFER_SIZE);

			if (inReadByteCount == -1) {
				return (inNextByte = -1);
			}

			inNextIndex = 0;
		}

		return (inNextByte = inBuffer[inNextIndex++]);
	}

	protected final char nc() throws IOException {
		while (inNextByte <= WHITE_SPACE) {
			nextByte();
		}

		char res = (char) inNextByte;
		nextByte();

		return res;
	}

	protected final int ni() throws IOException {
		int res = 0;

		while (inNextByte <= WHITE_SPACE) {
			nextByte();
		}

		boolean minus = (inNextByte == '-');

		if (minus) {
			nextByte();
		}
		if (inNextByte < '0' || inNextByte > '9') {
			throw new RuntimeException("Invalid integer value format to read");
		}

		do {
			res = (res << 1) + (res << 3) + inNextByte - '0';
		} while (nextByte() >= '0' && inNextByte <= '9');

		return minus ? -res : res;
	}

	protected final long nl() throws IOException {
		long res = 0;

		while (inNextByte <= WHITE_SPACE) {
			nextByte();
		}

		boolean minus = (inNextByte == '-');

		if (minus) {
			nextByte();
		}
		if (inNextByte < '0' || inNextByte > '9') {
			throw new RuntimeException("Invalid long value format to read");
		}

		do {
			res = (res << 1) + (res << 3) + inNextByte - '0';
		} while (nextByte() >= '0' && inNextByte <= '9');

		return minus ? -res : res;
	}

	protected final double nd() throws IOException {
		double pre = 0.0;
		double suf = 0.0;
		double div = 1.0;

		while (inNextByte <= WHITE_SPACE) {
			nextByte();
		}

		boolean minus = (inNextByte == '-');

		if (minus) {
			nextByte();
		}
		if (inNextByte < '0' || inNextByte > '9') {
			throw new RuntimeException("Invalid double value format to read");
		}

		do {
			pre = 10 * pre + (inNextByte - '0');
		} while (nextByte() >= '0' && inNextByte <= '9');

		if (inNextByte == '.') {
			while (nextByte() >= '0' && inNextByte <= '9') {
				suf += (inNextByte - '0') / (div *= 10);
			}
		}

		return minus ? -(pre + suf) : (pre + suf);
	}

	protected final String ns() throws IOException {
		while (inNextByte <= WHITE_SPACE) {
			nextByte();
		}

		StringBuilder sb = new StringBuilder();

		while (inNextByte > WHITE_SPACE) {
			sb.append((char) inNextByte);
			nextByte();
		}

		return sb.toString();
	}

	protected final char[] nc(int n) throws IOException {
		char[] a = new char[n];

		for (int i = 0; i < n; ++i) {
			a[i] = nc();
		}

		return a;
	}

	protected final char[][] nc(int r, int c) throws IOException {
		char[][] a = new char[r][c];

		for (int i = 0; i < r; ++i) {
			a[i] = nc(c);
		}

		return a;
	}

	protected final int[] ni(int n) throws IOException {
		int[] a = new int[n];

		for (int i = 0; i < n; ++i) {
			a[i] = ni();
		}

		return a;
	}

	protected final int[][] ni(int r, int c) throws IOException {
		int[][] a = new int[r][c];

		for (int i = 0; i < r; ++i) {
			a[i] = ni(c);
		}

		return a;
	}

	protected final long[] nl(int n) throws IOException {
		long[] a = new long[n];

		for (int i = 0; i < n; ++i) {
			a[i] = nl();
		}

		return a;
	}

	protected final long[][] nl(int r, int c) throws IOException {
		long[][] a = new long[r][c];

		for (int i = 0; i < r; ++i) {
			a[i] = nl(c);
		}

		return a;
	}

	protected final double[] nd(int n) throws IOException {
		double[] a = new double[n];

		for (int i = 0; i < n; ++i) {
			a[i] = nd();
		}

		return a;
	}

	protected final double[][] nd(int r, int c) throws IOException {
		double[][] a = new double[r][c];

		for (int i = 0; i < r; ++i) {
			a[i] = nd(c);
		}

		return a;
	}

	protected final String[] ns(int n) throws IOException {
		String[] a = new String[n];

		for (int i = 0; i < n; ++i) {
			a[i] = ns();
		}

		return a;
	}

	protected final String[][] ns(int r, int c) throws IOException {
		String[][] a = new String[r][c];

		for (int i = 0; i < r; ++i) {
			a[i] = ns(c);
		}

		return a;
	}

	protected void flushOutBuf() {
		try {
			if (outNextIndex <= 0) {
				return;
			}
			out.write(outBuffer, 0, outNextIndex);
			out.flush();
			outNextIndex = 0;
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	protected final void print(String s) {
		if (s == null) {
			s = "null";
		}
		for (int i = 0, N = s.length(); i < N; ++i) {
			outBuffer[outNextIndex++] = (byte) s.charAt(i);

			if (outNextIndex >= BUFFER_SIZE) {
				flushOutBuf();
			}
		}
	}

	protected final void println(String s) {
		print(s);
		print('\n');
	}

	protected final void print(Object obj) {
		if (obj == null) {
			print("null");
		} else {
			print(obj.toString());
		}
	}

	protected final void println(Object obj) {
		print(obj);
		print('\n');
	}

	protected final void print(String format, Object... args) {
		if (args != null) {
			format = String.format(format, args);
		}
		print(format);
	}

	protected final void println(String format, Object... args) {
		if (args != null) {
			format = String.format(format, args);
		}
		println(format);
	}

	protected void debug(String format, Object... args) {
		if (args != null) {
			format = String.format(format, args);
		}
		err.print(format);
	}

	protected void debugln(String format, Object... args) {
		if (args != null) {
			format = String.format(format, args);
		}
		err.println(format);
	}

	public static void main(String[] args) {
		try {
			new Solution().startWithAsciiIO(args);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	private void solve() throws Exception {
		int t = ni();
		while (t-- > 0) {
			final long N = nl();
			final long K = nl();

			final long segCount = N / K;
			final long segValue = mul(K - 1, K) / 2;
			final long remainCount = N % K;
			final long remainSegValue = mul(remainCount, remainCount + 1) / 2;

			println(add(mul(segCount, segValue), remainSegValue));
		}
	}

	long add(long a, long b) {
		return ((a % MOD) + (b % MOD)) % MOD;
	}

	long mul(long a, long b) {
		return ((a % MOD) * (b % MOD)) % MOD;
	}
}
