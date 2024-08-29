namespace Compet.Problems;

public class MaxSumOfSubArray {
	/// <summary>
	/// Find max sum of subarray. It can find best range too.
	/// Time: O(N)
	/// </summary>
	/// <param name="a"></param>
	/// <returns></returns>
	public long MaxSum(int[] a) {
		var N = a.Length;

		var sum = new long[N];
		sum[0] = a[0];
		for (var i = 1; i < N; ++i) {
			sum[i] = sum[i - 1] + a[i];
		}

		// For each index i, it holds end index performs best max sum from i.
		var range = new int[N];
		range[N - 1] = N - 1;

		for (var i = N - 2; i >= 0; --i) {
			// Sum of: a[i+1], ..., a[range[i+1]]
			if (sum[range[i + 1]] - sum[i] >= 0) {
				range[i] = range[i + 1];
			}
			else {
				range[i] = i;
			}
		}

		var maxSum = sum[0];
		for (var i = 0; i < N; ++i) {
			maxSum = Math.Max(maxSum, sum[range[i]] - sum[i] + a[i]);
		}

		return maxSum;
	}
}
