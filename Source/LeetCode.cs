public class Solution {
	public string MinimizeStringValue(string s) {
		var f = new int[26];
		var N = s.Length;
		foreach (var ch in s) {
			if (ch != '?') {
				f[ch - 'a']++;
			}
		}
		var arr = s.ToCharArray();
		for (var i = 0; i < N; ++i) {
			if (arr[i] == '?') {
				var min = f.Min();
				var target = 'a';
				for (var pos = 0; pos < f.Length; ++pos) {
					if (f[pos] == min) {
						target = (char)(pos + 'a');
						f[pos]++;
						break;
					}
				}
				arr[i] = target;
			}
		}
		return new string(arr);
	}

	public int SumOfPower(int[] nums, int k) {
		var mod = (int)(1E9 + 7);
		return (int)(dp(nums, k, nums.Length - 1) % mod);
	}

	long dp(int[] nums, int k, int endIndex) {
		if (k == 0) {
			return 1;
		}
		if (endIndex < 0 || k < 0) {
			return 0;
		}
		if (endIndex == 0) {
			return nums[0] == k ? 1 : 0;
		}
		return dp(nums, k, endIndex - 1) + dp(nums, k - nums[endIndex], endIndex - 1);
	}
}
