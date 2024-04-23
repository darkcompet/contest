public class Solution {
	public class Node {
		public int v;
		// v, w
		public List<(int, int)> adjacent = [];
	}
	public int[] MinimumTime(int N, int[][] edges, int[] disappear) {
		var nodes = new Node[N];
		for (var v = 0; v < N; ++v) {
			nodes[v] = new() { v = v };
		}
		foreach (var e in edges) {
			var u = e[0];
			var v = e[1];
			var w = e[2];
			nodes[u].adjacent.Add((v, w));
			nodes[v].adjacent.Add((u, w));
		}

		var MAX_DIST = 100_000_000;
		var distTo = new int[N];
		Array.Fill(distTo, MAX_DIST);
		distTo[0] = 0;

		var visited = new bool[N];
		visited[0] = true;

		var stack = new Stack<int>();
		stack.Push(0);
		while (stack.Count > 0) {
			var u = stack.Pop();
			visited[u] = true;
			var cnt = 0;
			foreach (var (v, w) in nodes[u].adjacent) {
				if (!visited[v] && (distTo[u] == MAX_DIST || distTo[u] + w < disappear[v])) {
					++cnt;
					distTo[v] = Math.Min(distTo[v], distTo[u] + w);
					stack.Push(v);
				}
			}
			// Backtracking
			if (cnt == 0) {
				visited[u] = false;
			}
		}

		for (var v = 0; v < N; ++v) {
			if (distTo[v] >= disappear[v]) {
				distTo[v] = -1;
			}
		}

		return distTo;
	}
}
