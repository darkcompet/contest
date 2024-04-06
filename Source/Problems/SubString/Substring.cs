public class Substring {
	/// <summary>
	/// For each string, find shortest substring that another string does not contain it.
	/// Ref: https://leetcode.com/contest/weekly-contest-388/problems/shortest-uncommon-substring-in-an-array/
	/// Time: O(N * logN)
	/// </summary>
	/// <param name="arr"></param>
	/// <returns></returns>
	public string[] ShortestUncommonSubstrings(string[] arr) {
		var N = arr.Length;

		// Build all substring of each string
		var subsArr = new HashSet<string>[N];
		for (var index = 0; index < N; ++index) {
			subsArr[index] = new();
			var s = arr[index];
			for (var i = 0; i < s.Length; ++i) {
				for (var j = i; j < s.Length; ++j) {
					subsArr[index].Add(s.Substring(i, j - i + 1));
				}
			}
		}

		// Convert set to list for sorting
		var subsList = new List<string>[N];
		for (var index = 0; index < N; ++index) {
			subsList[index] = subsArr[index].ToList();

			// Sort as (length, alphabet order)
			subsList[index].Sort((a, b) => {
				if (a.Length == b.Length) {
					return a.CompareTo(b);
				}
				return a.Length - b.Length;
			});
		}

		// For each string, find shortest one that another string does not contain it.
		var ans = new string[N];
		for (var i = 0; i < N; ++i) {
			foreach (var sub in subsList[i]) {
				var ok = true;
				for (var j = 0; j < N; ++j) {
					if (i != j) {
						if (subsArr[j].Contains(sub)) {
							ok = false;
						}
					}
				}
				if (ok) {
					ans[i] = sub;
					break;
				}
			}
		}
		return ans;
	}
}
