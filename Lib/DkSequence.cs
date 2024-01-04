
public class DkSequence {
	/// <summary>
	/// Just expand endIndex and check occurence of current number to find longest substring.
	/// Time: O(N)
	/// Src: https://leetcode.com/problems/longest-substring-without-repeating-characters/
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	public string LongestSubstringWithoutRepeatingChars(string s) {
		var N = s.Length;

		// Store last index of each char
		var lastIndexMap = new Dictionary<char, int>();

		// Longest substring.
		var bestStartIndex = 0;
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
				bestStartIndex = startIndex;
			}
		}

		return bestLen <= 0 ? string.Empty : s.Substring(bestStartIndex, bestLen);
	}
}
