package tool.compet.algorithm;

public class DkMaths {
	private static int mod = (int) 1E9 + 7;

	public static long add(long... args) {
		long res = 0;
		for (long v : args) {
			if (v >= mod || v <= -mod) v %= mod;
			res = (res + v) % mod;
		}
		return res;
	}

	public static long mul(long... args) {
		long res = 1;
		for (long v : args) {
			if (v >= mod || v <= -mod) v %= mod;
			res = res * v % mod;
		}
		return res;
	}

	public static long pow(long x, int n) {
		if (n < 0) throw new RuntimeException();
		long res = 1;
		if (x >= mod || x <= -mod) x %= mod;
		while (n > 0) {
			if ((n & 1) == 1) res = res * x % mod;
			n >>= 1;
			x = x * x % mod;
		}
		return res;
	}

	// fermat theorem: for a prime p, then a^(p-1) ≡ 1 (mod p)
	// that is, 1/a ≡ a^(p-2) (mod p)
	public static long inverse(long x) {
		return pow(x, mod - 2);
	}

	public static long[][] fif(int n) {
		long fact[] = new long[n + 1], ifact[] = new long[n + 1];
		fact[0] = ifact[0] = 1;
		for (int i = 1; i <= n; ++i) {
			fact[i] = mul(fact[i - 1], i);
			ifact[i] = inverse(fact[i]);
		}
		return new long[][]{fact, ifact};
	}

	public static long C(int n, int k, long[] fact, long[] ifact) {
		if (k < 0 || k > n) return 0;
		return mul(mod, fact[n], ifact[k], ifact[n - k]);
	}

	public static int log2(long x) {
		if (x < 1) throw new RuntimeException();
		int n = 0;
		while ((1 << n) <= x) ++n;
		return n - 1;
	}

	public static long gcd(long a, long b) {
		while (b > 0) {
			long tmp = a % b;
			a = b;
			b = tmp;
		}
		return a;
	}

	public static long lcm(long a, long b) {
		return a * (b / gcd(a, b));
	}

	public static long max(long... args) {
		long res = Long.MIN_VALUE;
		for (long v : args) if (res < v) res = v;
		return res;
	}

	public static long min(long... args) {
		long res = Long.MAX_VALUE;
		for (long v : args) if (res > v) res = v;
		return res;
	}

	public static double pow(double x, int n) {
		if (n < 0) throw new RuntimeException();
		double res = 1;
		while (n > 0) {
			if ((n & 1) == 1) res *= x;
			n >>= 1;
			x *= x;
		}
		return res;
	}

	public static double epsilon() {
		double eps = 1.0;
		while (eps / 2 != 0.0) eps /= 2;
		return eps;
	}

	public static double root(double a, int n) {
		double x1, x2 = 1;
		final double eps = epsilon();
		do {
			x1 = x2;
			x2 = ((n - 1) * x1 + a / pow(x1, n - 1)) / n;
		} while (x2 - x1 > eps || x2 - x1 < -eps);
		return x2;
	}

	public static long[] bernoulliNumbers(int n) {
		final int N = n + 1;
		long fif[][] = fif(N);
		long[] fact = fif[0], ifact = fif[1];
		long res[] = new long[N];
		res[0] = 1;
		for (int i = 1; i < N; ++i) {
			long sum = 0;
			for (int j = 0; j < i; ++j) {
				long tmp = mul(C(i + 1, j, fact, ifact), res[j]);
				sum = add(sum, tmp);
			}
			res[i] = -mul(inverse(i + 1), sum);
		}
		return res;
	}

	// return 1^c + 2^c + ... + n^c
	public static long powerSum(final long n, final int c) {
		if (n < 1) return 0;

		final long N = n + 1;
		final int K = c + 1;

		long[][] fif = fif(K);
		long fact[] = fif[0], ifact[] = fif[1];
		long B[] = bernoulliNumbers(K);

		long sum = 0;
		for (int i = 0; i < K; ++i) {
			long tmp = mul(C(K, i, fact, ifact), B[i], pow(N, K - i));
			sum = add(sum, tmp);
		}

		long ans = mul(inverse(K), sum);
		if (ans < 0) ans += mod;

		return ans;
	}

	public static int binInsertionSearch(int[] a, int key, boolean increased, boolean leftmost) {
		final int N = a.length;
		int left = 0, mid, right = N - 1;

		if (increased && leftmost) {
			if (key < a[0]) return 0;
			if (key > a[N - 1]) return N;
			while (left <= right) {
				mid = (left + right) >> 1;
				if (key <= a[mid]) {
					right = mid - 1;
				} else {
					left = mid + 1;
				}
			}
			return left;
		}

		if (increased && !leftmost) {
			if (key < a[0]) return 0;
			if (key > a[N - 1]) return N;
			while (left <= right) {
				mid = (left + right) >> 1;
				if (key >= a[mid]) {
					left = mid + 1;
				} else {
					right = mid - 1;
				}
			}
			return left;
		}

		if (!increased && leftmost) {
			if (key < a[N - 1]) return N;
			if (key > a[0]) return 0;
			while (left <= right) {
				mid = (left + right) >> 1;
				if (key < a[mid]) {
					left = mid + 1;
				} else {
					right = mid - 1;
				}
			}
			return left;
		}

		if (!increased && !leftmost) {
			if (key < a[N - 1]) return N;
			if (key > a[0]) return 0;
			while (left <= right) {
				mid = (left + right) >> 1;
				if (key <= a[mid]) {
					left = mid + 1;
				} else {
					right = mid - 1;
				}
			}
			return left;
		}
		return left;
	}

	public static int sqrt(int n) {
		int r = (int) Math.sqrt(n + 0.5);
		while ((long) r * r > n) --r;
		return r;
	}
}
