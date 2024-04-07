public class Solution {
	public int LongestMonotonicSubarray(int[] nums) {
		var ans = 1;
		for (var i = 0; i < nums.Length; ++i) {
			for (var j = i + 1; j < nums.Length; ++j) {
				var ok = true;
				for (var k = i; k < j; ++k) {
					if ((nums[i] <= nums[j] && nums[k] >= nums[k + 1]) || (nums[i] >= nums[j] && nums[k] <= nums[k + 1])) {
						ok = false;
						break;
					}
				}
				if (ok) {
					ans = Math.Max(ans, j - i + 1);
				}
			}
		}
		return ans;
	}

	public string GetSmallestString(string s, int k) {
		var N = s.Length;
		var output = string.Empty;
		for (var index = 0; index < N; ++index) {
			var ch = s[index];
			if (k > 0) {
				var target = NextChar(ch, k);
				output += target;
				k -= Dist(target, ch);
			}
			else {
				output += ch;
			}
		}
		return output;
	}

	private static char NextChar(char ch, int maxDist) {
		for (var pos = 0; pos < 26; ++pos) {
			var target = (char)('a' + pos);
			if (Dist(target, ch) <= maxDist) {
				return target;
			}
		}
		return 'a';
	}

	private static int Dist(char c1, char c2) {
		return Math.Min(Math.Abs(c1 - c2), 26 - Math.Abs(c1 - c2));
	}

	public long MinOperationsToMakeMedianK(int[] arr, int k) {
		var ans = 0L;
		var N = arr.Length;

		Array.Sort(arr);

		for (var index = 0; index < N; index++) {
			if (index < N / 2) {
				ans += Math.Max(0, arr[index] - k);
			}
			else if (index == N / 2) {
				ans += Math.Abs(k - arr[index]);
			}
			else {
				ans += Math.Max(0, k - arr[index]);
			}
		}

		return ans;
	}

	public int[] MinimumCost(int N, int[][] edges, int[][] query) {
		var ds = new DisjoinSet(N);
		var andBit = new int[N];
		Array.Fill(andBit, int.MinValue);

		foreach (var e in edges) {
			var u = e[0];
			var v = e[1];
			ds.MergeSets(u, v);
		}

		foreach (var e in edges) {
			var u = e[0];
			var w = e[2];

			var p = ds.FindSet(u);
			if (andBit[p] == int.MinValue) {
				andBit[p] = w;
			}
			else {
				andBit[p] &= w;
			}
		}

		var M = query.Length;
		var ans = new int[M];
		for (var index = 0; index < M; ++index) {
			var q = query[index];
			var u = q[0];
			var v = q[1];
			if (u == v) {
				ans[index] = 0;
				continue;
			}
			var p = ds.FindSet(u);
			if (p != ds.FindSet(v)) {
				ans[index] = -1;
			}
			else {
				ans[index] = andBit[p];
			}
		}
		return ans;
	}
	public class DisjoinSet {
		/// <summary>
		/// Element count.
		/// </summary>
		private readonly int elementCount;

		/// <summary>
		/// Opt: compress path.
		/// </summary>
		private readonly int[] set;

		/// <summary>
		/// Opt: when merge 2 sets.
		/// </summary>
		private readonly int[] rank;

		public DisjoinSet(int elementCount) {
			this.elementCount = elementCount;
			var set = this.set = new int[elementCount];
			this.rank = new int[elementCount];

			// Add default set (root) for each element
			for (var v = 0; v < elementCount; ++v) {
				set[v] = v;
			}
		}

		// private void AddSet(int v) {
		// 	this.set[v] = v;
		// }

		/// <summary>
		/// Merge 2 sets that contains given u, v.
		/// This is known as union action.
		/// </summary>
		/// <param name="u">Element 1 (must smaller than N)</param>
		/// <param name="v">Element 2 (must smaller than N)</param>
		public void MergeSets(int u, int v) {
			var pu = this.FindSet(u);
			var pv = this.FindSet(v);
			if (pu != pv) {
				// Opt: Only attach lower rank node to higher rank node to make tree height small as possible.
				var rank = this.rank;
				if (rank[pu] > rank[pv]) {
					(pu, pv) = (pv, pu);
				}
				// Attach set u to set v
				this.set[pu] = pv;
				if (rank[pv] == rank[pu]) {
					++rank[pv];
				}
			}
		}

		/// <summary>
		/// Find index of set (parent element) that contains the value.
		/// </summary>
		/// <param name="v">Find the set that element belongs to (must smaller than N)</param>
		/// <returns>Index of set that contains the element</returns>
		public int FindSet(int v) {
			var parent = this.set;
			if (parent[v] == v) {
				return v;
			}
			// Opt: Compress path by remember highest parent of the element.
			return parent[v] = this.FindSet(parent[v]);
		}

		/// <summary>
		/// At each set, we collect descendant elements under parent.
		/// </summary>
		/// <returns>Parent will be excluded from the set.</returns>
		public Dictionary<int, List<int>> CalcSetElements() {
			var descendants = new Dictionary<int, List<int>>();
			for (var v = this.elementCount - 1; v >= 0; --v) {
				var p = this.FindSet(v);
				var descendant = descendants[p] = descendants.GetValueOrDefault(p) ?? [];
				descendant.Add(v);
			}
			return descendants;
		}
	}
}
