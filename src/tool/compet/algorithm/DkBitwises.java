package tool.compet.algorithm;

public class DkBitwises {
	public boolean isPower2(long x) {
		return x > 0 && (x & (x - 1)) == 0;
	}

	public long lastBit(long x) {
		return x & -x;
	}

	public long divide(long x, long power2) {
		int n = 1;
		while ((power2 >> n++) != 1);
		long y = x >= 0 ? x : -x;
		y >>= (n - 1);
		return x >= 0 ? y : -y;
	}

	public long modulo(long x, int power2) {
		return x & (power2 - 1);
	}

	public long increment(long x) {
		return -(~x);
	}

	public int bitCount(long x) {
		int cnt = 0;
		while (x > 0) {
			if ((x & 1) == 1) ++cnt;
			x >>= 1;
		}
		return cnt;
	}

	public static long add(long a, long b) {
		long c;

		while (a != 0) {
			c = (a & b) << 1;
			b = a ^ b;
			a = c;
		}
		return b;
	}

	public static long mul(int a, int b) {
		long res = 0;

		if (a < b) {
			a ^= b;
			b ^= a;
			a ^= b;
		}
		while (b != 0) {
			if ((b & 1) == 1) res += a;
			a <<= 1;
			b >>= 1;
		}
		return res;
	}

	public static String toBinaryString(long x) {
		StringBuilder sb = new StringBuilder(65);

		for (int i = 63; i >= 0; --i) {
			sb.append((x >> i) & 1);
			if (i > 0 && (i & 3) == 0) {
				sb.append('_');
				if (i == 32) {
					sb.append('_');
				}
			}
		}
		return sb.toString();
	}

	public void test() {
		System.out.println(lastBit(0x10));
	}
}
