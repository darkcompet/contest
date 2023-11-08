
public class DkMaths {
	/// Fast pow with O(logN) time.
	/// Method 1: x^9 = x^0 * x^1 * x^0 * x^0 * x^8. Note: 9 = 1001.
	/// Method 2: x^9 = x^4 * x^4 * x
	public static long Pow(long x, uint n) {
		var result = 1L;
		while (n > 0) {
			// Mul if meet bit 1
			if ((n & 1) == 1) {
				result = (result * x); // Mod here
			}
			// Down n and Up x
			n >>= 1;
			x = (x * x); // Mod here
		}

		return result;
	}

	/// <summary>
	/// For positive integer N, this finds largest number k such that: 2^k <= N.
	/// Time: O(logN).
	/// </summary>
	/// <param name="n">Must be >= 1</param>
	/// <returns></returns>
	public static int FloorLog2(long n) {
		// if (n <= 0) {
		// 	throw new Exception("N must be > 0");
		// }
		var k = 0;
		while ((1L << k) <= n) {
			++k;
		}
		return k - 1;
	}


	/// <summary>
	/// For any integer N, this finds lowest number k such that: 2^k >= N.
	/// Time: O(logN).
	/// </summary>
	/// <param name="n">Should be >= 0</param>
	/// <returns></returns>
	public static int CeilLog2(long n) {
		// if (n < 0) {
		// 	throw new Exception("N must be >= 0");
		// }
		var k = 0;
		while ((1L << k) < n) {
			++k;
		}
		return k;
	}


	/// <summary>
	/// For positive integer N, this checks given N is power of 2 or not.
	/// Time: O(1).
	/// </summary>
	/// <param name="N">Any</param>
	/// <returns></returns>
	public static bool IsPowOf2(long N) {
		return N > 0 && (N & (N - 1)) == 0;
	}
}
