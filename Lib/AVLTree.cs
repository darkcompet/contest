namespace Tool.Compet.Core;

public class AATree<TKey, TValue> where TKey : IComparable<TKey> {
	private class Node {
		// Node internal data
		internal int level;
		internal Node left;
		internal Node right;

		// User data
		internal TKey key;
		internal TValue value;

		// Constuctor for the sentinel node
		internal Node() {
			this.level = 0;
			this.left = this;
			this.right = this;
		}

		// Constuctor for regular nodes (that all start life as leaf nodes)
		internal Node(TKey key, TValue value, Node sentinel) {
			this.level = 1;
			this.left = sentinel;
			this.right = sentinel;
			this.key = key;
			this.value = value;
		}
	}

	Node root;
	Node sentinel;
	Node? deleted;

	public AATree() {
		this.root = this.sentinel = new Node();
		this.deleted = null;
	}

	private void Skew(ref Node node) {
		if (node.level == node.left.level) {
			// rotate right
			var left = node.left;
			node.left = left.right;
			left.right = node;
			node = left;
		}
	}

	private void Split(ref Node node) {
		if (node.right.right.level == node.level) {
			// rotate left
			var right = node.right;
			node.right = right.left;
			right.left = node;
			node = right;
			node.level++;
		}
	}

	private bool Insert(ref Node node, TKey key, TValue value) {
		if (node == this.sentinel) {
			node = new Node(key, value, this.sentinel);
			return true;
		}

		var compare = key.CompareTo(node.key);
		if (compare < 0) {
			if (!this.Insert(ref node.left, key, value)) {
				return false;
			}
		}
		else if (compare > 0) {
			if (!this.Insert(ref node.right, key, value)) {
				return false;
			}
		}
		else {
			return false;
		}

		this.Skew(ref node);
		this.Split(ref node);

		return true;
	}

	private bool Delete(ref Node node, TKey key) {
		if (node == this.sentinel) {
			return this.deleted != null;
		}

		var compare = key.CompareTo(node.key);
		if (compare < 0) {
			if (!this.Delete(ref node.left, key)) {
				return false;
			}
		}
		else {
			if (compare == 0) {
				this.deleted = node;
			}
			if (!this.Delete(ref node.right, key)) {
				return false;
			}
		}

		var del = this.deleted;
		if (del != null) {
			del.key = node.key;
			del.value = node.value;
			this.deleted = null;
			node = node.right;
		}
		else if (node.left.level < node.level - 1 || node.right.level < node.level - 1) {
			--node.level;
			if (node.right.level > node.level) {
				node.right.level = node.level;
			}
			this.Skew(ref node);
			this.Skew(ref node.right);
			this.Skew(ref node.right.right);
			this.Split(ref node);
			this.Split(ref node.right);
		}

		return true;
	}

	private Node? Search(Node node, TKey key) {
		if (node == this.sentinel) {
			return null;
		}

		var compare = key.CompareTo(node.key);
		if (compare < 0) {
			return Search(node.left, key);
		}
		else if (compare > 0) {
			return Search(node.right, key);
		}
		else {
			return node;
		}
	}

	public bool Add(TKey key, TValue value) {
		return this.Insert(ref this.root, key, value);
	}

	public bool Remove(TKey key) {
		return this.Delete(ref this.root, key);
	}

	public TValue this[TKey key] {
		get {
			var node = this.Search(this.root, key);
			return node is null ? default : node.value;
		}
		set {
			var node = this.Search(root, key);
			if (node == null) {
				this.Add(key, value);
			}
			else {
				node.value = value;
			}
		}
	}

	public TValue? Lowerbound(TKey key) {
		var node = this.Lowerbound(this.root, key);
		return node is null ? default : node.value;
	}

	private Node? Lowerbound(Node node, TKey key) {
		if (node == this.sentinel) {
			return null;
		}

		var compare = key.CompareTo(node.key);
		if (compare < 0) {
			if (node.left is null || node.left.key.CompareTo(key) <= 0) {
				return node.left;
			}
			return this.Lowerbound(node.left, key);
		}

		if (compare > 0) {
			if (node.right is null || node.right.key.CompareTo(key) >= 0) {
				return node.right;
			}
			return this.Lowerbound(node.right, key);
		}

		return node;
	}
}
