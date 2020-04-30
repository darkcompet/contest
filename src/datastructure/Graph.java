package tool.compet.datastructure;

import java.util.*;

public class Graph {
	private int V, E;
	private Node[] nodes;

	public class Node {
		int id;
		Map<Integer, Double> adj = new TreeMap<>();

		boolean marked;
		int depth;
		int parent;

		private Node(int id) {this.id = id;}
	}

	public Graph(int n) {
		V = n;
		nodes = new Node[n];
		for (int i = 0; i < n; ++i) {
			nodes[i] = new Node(i);
		}
	}

	public int V() {
		return V;
	}

	public int E() {
		return E;
	}

	public Set<Integer> adj(int u) {
		return nodes[u].adj.keySet();
	}

	public boolean exist(int u, int v) {
		return nodes[u].adj.containsKey(v) && nodes[v].adj.containsKey(u);
	}

	public void add(int u, int v, double cost) {
		if (exist(u, v)) return;
		++E;
		nodes[u].adj.put(v, cost);
		nodes[v].adj.put(u, cost);
	}

	public void delete(int u, int v) {
		if (!exist(u, v)) return;
		--E;
		nodes[u].adj.remove(v);
		nodes[v].adj.remove(u);
	}

	public double getCost(int u, int v) {
		return nodes[u].adj.get(v);
	}

	public void setCost(int u, int v, double cost) {
		if (!exist(u, v)) return;
		nodes[u].adj.put(v, cost);
		nodes[v].adj.put(u, cost);
	}

	public void dfs(int u) {
		LinkedList<Integer> stack = new LinkedList<>();
		stack.addLast(u);
		nodes[u].marked = true;
		while (stack.size() > 0) {
			u = stack.removeLast();
			for (int v : adj(u)) {
				if (nodes[v].marked) continue;
				nodes[v].marked = true;
				stack.addLast(v);
			}
		}
	}

	public void bfs(int u) {
		LinkedList<Integer> que = new LinkedList<>();
		que.addLast(u);
		nodes[u].parent = u;
		while (que.size() > 0) {
			u = que.removeFirst();
			for (int v : adj(u)) {
				if (nodes[v].marked) continue;
				nodes[v].marked = true;
				nodes[v].depth++;
				nodes[v].parent = u;
				que.addLast(v);
			}
		}
	}

	public void makeTest() {
		for (int v = 1; v < nodes.length; ++v) {
			add(0, v, 1);
		}
		add(1, 2, 1);
	}
}
