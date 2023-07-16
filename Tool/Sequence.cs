
public class Sequence {
	/// Time: O(N)
	public string LongestSubstring_UniqueChars(string s) {
		var N = s.Length;

		// Store last index of each char
		var lastIndexMap = new Dictionary<char, int>();

		// Longest substring
		var startIndex = 0;
		var endIndex = 0;
		var longestLength = 0;

		for (var index = 0; index < N; ++index) {
			var ch = s[index];
			var lastIndex = lastIndexMap.GetValueOrDefault(ch, -1);

			if (lastIndex >= 0) {
				startIndex = Math.Max(startIndex, lastIndex);
			}

			lastIndexMap[ch] = index;
			if (index - startIndex + 1 > longestLength) {
				longestLength = index - startIndex + 1;
				endIndex = index;
			}
		}

		return s.Substring(startIndex, endIndex - startIndex + 1);
	}
}
