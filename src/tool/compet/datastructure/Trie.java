package tool.compet.datastructure;

import java.util.ArrayList;
import java.util.List;

public class Trie {
	private Node root = new Node();

	private class Node {
		private char data;
		private List<Node> children = new ArrayList<>();

		@Override
		public String toString() {
			String res = "";
			res += data + ": {";
			for (int i = 0, N = children.size(); i < N; ++i) {
				res += children.get(i).toString();
				if (i < N - 1) res += ", ";
			}
			return res + "}";
		}
	}

	public void add(char[] a) {
		Node node = root;
		for (char data : a) {
			boolean found = false;
			for (Node child : node.children) {
				if (child.data == data) {
					found = true;
					node = child;
					break;
				}
			}
			if (!found) {
				Node child = new Node();
				child.data = data;
				node.children.add(child);
				node = child;
			}
		}
	}

	public void remove(char[] a) {}

	public void test() {
		add("abc".toCharArray());
		System.out.println(root.toString());
	}
}
