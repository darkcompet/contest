namespace Compet.Problems;

public class DifferentAdjacent {
	/// <summary>
	/// Count number of subarray that every adjacent-elements are different.
	/// Time: O(N)
	/// Ref: https://leetcode.com/problems/count-alternating-subarrays/
	/// </summary>
	/// <param name="arr"></param>
	/// <returns></returns>
	public long CountSubarrayWithDifferentAdjacent(int[] arr) {
		var N = arr.Length;
		var ans = 0L;
		for (var i = 0; i < N;) {
			var j = i;
			while (j + 1 < N && arr[j] != arr[j + 1]) {
				++j;
			}

			// Assert: j == N - 1 || arr[j] == arr[j + 1]
			var len = j - i + 1L;
			ans += len * (len + 1) / 2;
			i = j + 1;
		}
		return ans;
	}

	/// <summary>
	/// Find one of subarray that sum of its elements is maximum.
	/// Time: O(N)
	/// Ref: https://atcoder.jp/contests/arc174/tasks/arc174_a
	/// </summary>
	/// <param name="arr"></param>
	/// <returns></returns>
	public List<(int, int)> FindSubarrayThatMaximumSum(int[] arr) {
		var se = new List<(int, int)>();
		return se;
	}
}
