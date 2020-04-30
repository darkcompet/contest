package tool.compet.algorithm;

public class DkUnionFind {
	private Node T[];

	public class Node {
		int parent, rank;
	}

	public void build(int n) {
		T = new Node[n];
		for (int v = 0; v < n; ++v) {
			T[v] = new Node();
			T[v].parent = v;
		}
	}

	public void union(final int u, final int v) {
		int uRoot = find(u), vRoot = find(v);
		if (uRoot == vRoot) return;
		Node uRootNode = T[uRoot], vRootNode = T[vRoot];
		if (uRootNode.rank > vRootNode.rank) {
			vRootNode.parent = uRoot;
		} else if (uRootNode.rank < vRootNode.rank) {
			uRootNode.parent = vRoot;
		} else {
			vRootNode.parent = uRoot;
			uRootNode.rank++;
		}
	}

	public int find(final int v) {
		int par = v;
		while (T[par].parent != par) {
			par = T[par].parent;
		}
		T[v].parent = par;
		return par;
	}

	public void test() {
		build(5);
		union(0, 1);
		union(0, 4);
		union(3, 2);
		for (int v = 0; v < T.length; ++v) {
			System.out.println(v + " -> " + find(v));
		}
	}
}
