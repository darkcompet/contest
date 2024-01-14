
public class LongestSubstringWithUniqueChars {
	/// <summary>
	/// Find all longest substrings such that all chars in it is unique.
	/// Idea: Start from 0-index, just expand endIndex. If next char (nextIndex) found in [startIndex, endIndex],
	/// just set startIndex = nextIndex.
	/// Time: O(N) where N is length of given string.
	/// Src: https://leetcode.com/problems/longest-substring-without-repeating-characters/
	/// </summary>
	/// <param name="s"></param>
	/// <returns>List of longest substring. Each item is [index, length] of the longest substring.</returns>
	public List<int[]> LongestSubstringWithoutRepeatingChars_Impl(string s) {
		var ans = new List<int[]>();
		var N = s.Length;

		// Store last index of each char
		var lastIndexMap = new Dictionary<char, int>();

		// Longest substring.
		var bestLen = 0;
		var startIndex = 0;

		for (var index = 0; index < N; ++index) {
			var ch = s[index];

			// Last occurence
			var lastIndex = lastIndexMap.GetValueOrDefault(ch, -1);
			if (lastIndex >= 0) {
				startIndex = Math.Max(startIndex, lastIndex + 1);
			}

			lastIndexMap[ch] = index;

			// Update new len
			var newLen = index - startIndex + 1;
			if (newLen > bestLen) {
				bestLen = newLen;

				ans.Clear();
				ans.Add(new int[] { startIndex, newLen });
			}
			else if (newLen == bestLen) {
				ans.Add(new int[] { startIndex, bestLen });
			}
		}

		return ans;
	}

	/// <summary>
	/// Find all longest substrings such that all chars in it is unique.
	/// Idea: By using DP, divide to 2 subtasks. First task is remove index and solve on range [index + 1, s.Length - 1].
	/// Seconds task is accept the index, and find endIndex in range [index + 1, s.Length - 1].
	/// Time: O(N) where N is length of given string.
	/// Src: https://leetcode.com/problems/longest-substring-without-repeating-characters/
	/// </summary>
	/// <param name="s"></param>
	/// <returns>List of longest substring. Each item is [index, length] of the longest substring.</returns>
	public List<int[]> LongestSubstringWithoutRepeatingChars_DP(string s) {
		var ans = new List<int[]>();
		var N = s.Length;
		return ans;
	}
}
