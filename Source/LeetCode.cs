public class Solution {
	public int[] ResultArray(int[] nums) {
		var a1 = new List<int>();
		var a2 = new List<int>();
		a1.Add(nums[0]);
		a2.Add(nums[1]);
		for (var index = 2; index < nums.Length; ++index) {
			if (a1.Last() > a2.Last()) {
				a1.Add(nums[index]);
			}
			else {
				a2.Add(nums[index]);
			}
		}
		a1.AddRange(a2);
		return a1.ToArray();
	}

	public int CountSubmatrices(int[][] grid, int k) {
		var count = 0;
		var N = grid.Length;
		var M = grid[0].Length;
		var dpRow = new int[N][];
		var dpCol = new int[M][];

		for (var i = 0; i < N; ++i) {
			dpRow[i] = new int[M];
		}
		for (var i = 0; i < M; ++i) {
			dpCol[i] = new int[N];
		}

		dpRow[0][0] = dpCol[0][0] = grid[0][0];
		for (var i = 0; i < N; ++i) {
			dpRow[i][0] = grid[i][0];
			for (var j = 1; j < M; ++j) {
				dpRow[i][j] = dpRow[i][j - 1] + grid[i][j];
			}
		}
		for (var j = 0; j < M; ++j) {
			dpCol[j][0] = grid[0][j];
			for (var i = 1; i < N; ++i) {
				dpCol[j][i] = dpCol[j][i - 1] + grid[i][j];
			}
		}

		var dp = new int[N][];
		for (var i = 0; i < N; ++i) {
			dp[i] = new int[M];
		}
		for (var i = 0; i < N; ++i) {
			for (var j = 0; j < M; ++j) {
				var prev = 0;
				if (i > 0 && j > 0) {
					prev = dp[i - 1][j - 1];
				}
				dp[i][j] = prev + dpRow[i][j] + dpCol[j][i] - grid[i][j];
			}
		}

		for (var i = 0; i < N; ++i) {
			for (var j = 0; j < M; ++j) {
				if (dp[i][j] > k) {
					break;
				}
				++count;
			}
		}
		return count;
	}
}
