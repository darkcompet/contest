using System.Runtime.CompilerServices;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

/// LCA (Least Common Ancestor)
/// It finds height-lowest common ancestor (parent) of 2 nodes in a tree.
public class DkLCA {
	public class Node {
		public List<int> children = new List<int>();
		public bool visited;
	}

	/// @param a: Index of node A
	/// @param b: Index of node B
	public void Run(Node[] G, int a, int b) {
		// Build parent via dfs
		Dfs(G, a);
	}

	private void Dfs(Node[] nodes, int p) {
		nodes[p].visited = true;

		foreach (var c in nodes[p].children) {
			if (!nodes[c].visited) {
				Dfs(nodes, c);
			}
		}
	}
}
