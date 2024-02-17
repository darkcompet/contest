namespace Compet.Algorithm;

public class Zarray {
	/// <summary>
	/// Ref: https://www.geeksforgeeks.org/z-algorithm-linear-time-pattern-searching-algorithm
	/// https://cp-algorithms.com/string/z-function.html
	/// https://codeforces.com/blog/entry/3107
	/// </summary>
	/// <param name="s"></param>
	/// <param name="p"></param>
	/// <param name="concanator"></param>
	/// <returns></returns>
	public static int[] CalcZarray(string s, string p, char concanator = '$') {
		var str = p + concanator + s;
		var zarr = new int[str.Length];
		var N = str.Length;

		// [L,R] make a window which
		// matches with prefix of s
		var L = 0;
		var R = 0;

		for (var i = 1; i < N; ++i) {
			// if i>R nothing matches so we will
			// calculate. Z[i] using naive way.
			if (i > R) {
				L = R = i;

				// R-L = 0 in starting, so it will start
				// checking from 0'th index. For example,
				// for "ababab" and i = 1, the value of R
				// remains 0 and Z[i] becomes 0. For string
				// "aaaaaa" and i = 1, Z[i] and R become 5
				while (R < N && str[R - L] == str[R]) {
					R++;
				}

				zarr[i] = R - L;
				R--;

			}
			else {
				// k = i-L so k corresponds to number
				// which matches in [L,R] interval.
				var k = i - L;

				// if Z[k] is less than remaining interval
				// then Z[i] will be equal to Z[k].
				// For example, str = "ababab", i = 3,
				// R = 5 and L = 2
				if (zarr[k] < R - i + 1) {
					zarr[i] = zarr[k];
				}

				// For example str = "aaaaaa" and
				// i = 2, R is 5, L is 0
				else {


					// else start from R and
					// check manually
					L = i;
					while (R < N && str[R - L] == str[R]) {
						R++;
					}

					zarr[i] = R - L;
					R--;
				}
			}
		}

		return zarr;
	}
}
