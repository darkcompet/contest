namespace Tool.Compet.Core {
	public class DkMaths {
		/// Method 1: express n as binary (0, 1)
		/// 19 = 10011
		/// x^19 = x^(2^4) * x^(2^1) * x^(2^0)
		/// Method 2: split n as 2
		/// x => x^2 => x^4 => x^9 => x^19
		/// path: 0011, 19 = 10011
		public static long Pow(long x, uint n) {
			if (n < 0) {
				return 0;
			}
			if (n == 0) {
				return x == 0 ? 0 : 1;
			}
			var res = 1L;
			while (n > 0) {
				if ((n & 1) == 1) {
					res = (res * x); // mod here
				}
				n >>= 1;
				x = (x * x); // mod here
			}

			return res;
		}

		// y = c + x * (b + x * (a + x * 0))
		public static long Poly(long[] arr, long x) {
			var p = 0L;
			for (int index = 0, N = arr.Length; index < N; ++index) {
				p = arr[index] + x * p;
			}
			return p;
		}
	}
}
