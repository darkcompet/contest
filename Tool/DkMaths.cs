
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
}
