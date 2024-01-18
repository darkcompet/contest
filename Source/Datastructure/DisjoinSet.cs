namespace Compet.Datastructure;

/// <summary>
/// This is known as Union-Find.
/// Ref:
/// https://cp-algorithms.com/data_structures/disjoint_set_union.html
/// </summary>
public class DisjoinSet {
	/// <summary>
	/// Element count.
	/// </summary>
	private readonly int N;

	/// <summary>
	/// Opt: compress path.
	/// </summary>
	private readonly int[] parent;

	/// <summary>
	/// Opt: when merge 2 sets.
	/// </summary>
	private readonly int[] rank;

	public DisjoinSet(int elementCount) {
		this.N = elementCount;
		this.parent = new int[elementCount];
		this.rank = new int[elementCount];

		Array.Fill(this.parent, -1);
	}

	/// <summary>
	/// Merge 2 sets that contains given u, v.
	/// This is known as union action.
	/// </summary>
	/// <param name="u">Index of element 1 (must smaller than N)</param>
	/// <param name="v">Index of element 2 (must smaller than N)</param>
	public void MergeSets(int u, int v) {
		var s1 = this.FindSet(u);
		var s2 = this.FindSet(v);
		if (s1 != s2) {
			// Opt: Only attach lower rank node to higher rank node to make tree height small as possible.
			var rank = this.rank;
			if (rank[u] > rank[v]) {
				(u, v) = (v, u);
			}
			this.parent[u] = v;
			if (rank[v] == rank[u]) {
				++rank[v];
			}
		}
	}

	/// <summary>
	/// Find index of set that hold the value.
	/// </summary>
	/// <param name="v">Index to find its set (must smaller than N)</param>
	/// <returns>Index of set that contains the element</returns>
	public int FindSet(int v) {
		var parent = this.parent;
		if (v == parent[v]) {
			return v;
		}
		// Opt: Compress path by remember highest parent of the element.
		return parent[v] = this.FindSet(parent[v]);
	}
}
