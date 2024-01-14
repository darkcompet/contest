namespace Contest.String;

public class Search {
	public List<int> FindOccurrencyIndices(string s, string pattern) {
		var indices = new List<int>();
		var N = s.Length;
		var firstPatternCh = pattern[0];

		for (var index = 0; index < N; ++index) {
			if (s[index] != firstPatternCh) {
				continue;
			}
		}

		return indices;
	}
}
