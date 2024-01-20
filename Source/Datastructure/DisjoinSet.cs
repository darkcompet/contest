namespace Compet.Datastructure;

/// <summary>
/// This is known as Union-Find.
/// Ref:
/// https://cp-algorithms.com/data_structures/disjoint_set_union.html
/// https://www.geeksforgeeks.org/disjoint-set-data-structures/
/// </summary>
public class DisjoinSet {
	/// <summary>
	/// Element count.
	/// </summary>
	private readonly int N;

	/// <summary>
	/// Opt: compress path.
	/// </summary>
	private readonly int[] set;

	/// <summary>
	/// At each set, this holds its down tree and itself.
	/// </summary>
	private readonly List<int>[] descendant;

	/// <summary>
	/// Opt: when merge 2 sets.
	/// </summary>
	private readonly int[] rank;

	public DisjoinSet(int elementCount) {
		this.N = elementCount;
		var set = this.set = new int[elementCount];
		var descendant = this.descendant = new List<int>[elementCount];
		this.rank = new int[elementCount];

		// Add default set for each element
		for (var v = 0; v < elementCount; ++v) {
			set[v] = v;
			descendant[v] = new() { v };
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
		var u_s = this.FindSet(u);
		var v_s = this.FindSet(v);
		if (u_s != v_s) {
			// Opt: Only attach lower rank node to higher rank node to make tree height small as possible.
			var rank = this.rank;
			if (rank[u] > rank[v]) {
				(u, v) = (v, u);
			}
			this.set[u] = v;
			this.descendant[v_s].AddRange(this.descendant[u_s]);
			this.descendant[u_s].Clear();
			if (rank[v] == rank[u]) {
				++rank[v];
			}
		}
	}

	/// <summary>
	/// Find index of set (element, parent) that contains the value.
	/// </summary>
	/// <param name="v">Find the set that element belongs to (must smaller than N)</param>
	/// <returns>Index of set that contains the element</returns>
	public int FindSet(int v) {
		var set = this.set;
		if (set[v] == v) {
			return v;
		}
		// Opt: Compress path by remember highest parent of the element.
		return set[v] = this.FindSet(set[v]);
	}

	public List<int> GetSubtree(int v) {
		return this.descendant[v];
	}
}
