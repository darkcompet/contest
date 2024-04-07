namespace Compet.Problems;

public class DifferentAdjacent {
	/// <summary>
	/// Time: O(N)
	/// Ref: https://leetcode.com/problems/count-alternating-subarrays/
	/// </summary>
	/// <param name="arr"></param>
	/// <returns></returns>
	public long CountAlternatingSubarrays(int[] arr) {
		var N = arr.Length;
		var ans = 0L;
		for (var i = 0; i < N;) {
			var j = i;
			while (j < N) {
				if (j == N - 1 || arr[j] == arr[j + 1]) {
					break;
				}
				++j;
			}
			var len = j - i + 1;
			ans += 1L * len * (len + 1) / 2;
			i = j + 1;
		}
		return ans;
	}
}
