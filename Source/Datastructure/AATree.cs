namespace Compet.Datastructure;

/// <summary>
/// Binary Search Tree (BST) version that always rebalance tree to keep height of the tree near to log(N).
/// This is like as Red-Black Tree, but provide more efficient implementation.
/// Ref:
/// https://github.com/JuYanYan/AA-Tree/blob/master/aatree.cpp
/// https://users.cs.fiu.edu/~weiss/dsaa_c++/code/AATree.cpp
/// https://www.nayuki.io/res/aa-tree-set/BasicAaTreeSet.java
/// https://en.wikipedia.org/wiki/AA_tree
/// </summary>
/// <typeparam name="T"></typeparam>
public class AATree<T> where T : IComparable<T> {
	private AATreeNode? root;

	public AATree() {
	}

	public void Add(T value) {
		this.root = Insert(this.root, value);
	}

	/// <summary>
	/// Time: O(log(N)) where N is number of node.
	/// </summary>
	/// <param name="node"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	private static AATreeNode Insert(AATreeNode? node, T value) {
		if (node is null) {
			return new AATreeNode(value, null, null);
		}

		var compare = value.CompareTo(node.value);
		if (compare == 0) {
			// Add to bucket for value that equals to this node's value but different instance.
			if (!value.Equals(node.value)) {
				node.bucket ??= new();
				node.bucket.Add(value);
			}
			return node;
		}

		if (compare < 0) {
			node.left = Insert(node.left, value);
		}
		else {
			node.right = Insert(node.right, value);
		}

		node = _Skew(node);
		node = _Split(node);

		return node;
	}

	/// <summary>
	/// Time: O(log(N)) where N is number of node.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public bool Remove(T value) {
		this.root = _Delete(this.root, value);
		return this.root != null;
	}

	private static AATreeNode? _Delete(AATreeNode? node, T value, bool forceDeleteLeaf = false) {
		if (node is null) {
			return null;
		}

		var compare = value.CompareTo(node.value);
		if (compare < 0) {
			// Delete left node (we will rebalancing tree later)
			node.left = _Delete(node.left, value, forceDeleteLeaf);
		}
		else if (compare > 0) {
			// Delete right node (we will rebalancing tree later)
			node.right = _Delete(node.right, value, forceDeleteLeaf);
		}
		else {
			if (!forceDeleteLeaf) {
				// Try remove from bucket first.
				if (node.bucket != null && node.bucket.Remove(value)) {
					return node;
				}
				// Try remove value from node itself.
				if (!node.value.Equals(value)) {
					return node;
				}

				// Remove node value. Then bring one of bucket value to node's value.
				if (node.bucket?.Count > 0) {
					node.value = node.bucket.First();
					node.bucket.Remove(node.value);
					return node;
				}
			}

			// Always remove leaf node.
			if (node.left is null && node.right is null) {
				return null;
			}

			// Remove successor node
			if (node.left is null) {
				// Successor is leaf node. Find it by go one right, then turn left until meet leaf node.
				var successorNode = _Successor(node);
				// Remove the node by overwrite successor data to it
				node.value = successorNode.value;
				node.bucket = successorNode.bucket;
				node.right = _Delete(node.right, successorNode.value, true);
			}
			// Remove predecessor node
			else {
				// Predecessor is leaf node. Find it by go one left, then turn right until meet leaf node.
				var predecessorNode = _Predecessor(node);
				// Remove the node by overwrite predecessor data to it
				node.value = predecessorNode.value;
				node.bucket = predecessorNode.bucket;
				node.left = _Delete(node.left, predecessorNode.value, true);
			}
		}

		node = _DecreaseLevel(node);
		node = _Skew(node);

		if (node.right != null) {
			node.right = _Skew(node.right);

			if (node.right.right != null) {
				node.right.right = _Skew(node.right.right);
			}
		}

		node = _Split(node);
		if (node.right != null) {
			node.right = _Split(node.right);
		}

		return node;
	}

	private static AATreeNode _DecreaseLevel(AATreeNode node) {
		if (node.left != null && node.right != null) {
			var shouldBeLevel = Math.Min(node.left.level, node.right.level) + 1;
			if (node.level > shouldBeLevel) {
				node.level = shouldBeLevel;
				if (node.right != null && node.right.level > shouldBeLevel) {
					node.right.level = shouldBeLevel;
				}
			}
		}
		return node;
	}

	private static AATreeNode _Predecessor(AATreeNode curNode) {
		curNode = curNode.left!;
		while (curNode.right != null) {
			curNode = curNode.right;
		}
		return curNode;
	}

	private static AATreeNode _Successor(AATreeNode curNode) {
		curNode = curNode.right!;
		while (curNode.left != null) {
			curNode = curNode.left;
		}
		return curNode;
	}

	private static AATreeNode _Skew(AATreeNode node) {
		if (node.left != null) {
			// Rotate right
			if (node.level == node.left.level) {
				var left = node.left;
				node.left = left.right;
				left.right = node;

				return left;
			}
		}
		return node;
	}

	private static AATreeNode _Split(AATreeNode node) {
		if (node.right?.right != null) {
			// Rotate left
			if (node.right.right.level == node.level) {
				var right = node.right;
				node.right = right.left;
				right.left = node;
				++right.level;

				return right;
			}
		}
		return node;
	}

	private static AATreeNode? _Search(AATreeNode? node, T value) {
		if (node is null) {
			return null;
		}
		var compare = value.CompareTo(node.value);
		if (compare < 0) {
			return _Search(node.left, value);
		}
		if (compare > 0) {
			return _Search(node.right, value);
		}

		return (node.value.Equals(value) || (node.bucket != null && node.bucket.Contains(value))) ? node : null;
	}

	/// <summary>
	/// Time: O(log(N)) where N is number of node.
	/// </summary>
	/// <returns></returns>
	/// <exception cref="Exception">If tree root null</exception>
	public T FindMin() {
		if (this.root is null) {
			throw new Exception("Tree must not be empty");
		}
		var node = this.root;
		T result;

		do {
			result = node.value;
			node = node.left;
		}
		while (node != null);

		return result;
	}

	/// <summary>
	/// Time: O(log(N)) where N is number of node.
	/// </summary>
	/// <returns></returns>
	/// <exception cref="Exception">If tree root null</exception>
	public T FindMax() {
		if (this.root is null) {
			throw new Exception("Tree must not be empty");
		}

		var node = this.root;
		T result;

		do {
			result = node.value;
			node = node.right;
		}
		while (node != null);

		return result;
	}

	/// <summary>
	/// Check whether the tree has any node or not.
	/// Time: O(1).
	/// </summary>
	/// <returns></returns>
	public bool IsEmpty() {
		return this.root is null;
	}

	/// <summary>
	/// Find first node that value is greater than equals to the given `minValue`.
	/// Time: O(log(N)) where N is number of node.
	/// </summary>
	/// <param name="minValue"></param>
	/// <param name="result"></param>
	/// <returns></returns>
	public bool Lowerbound(T minValue, out T? result) {
		return _Lowerbound(this.root, minValue, out result);
	}

	private static bool _Lowerbound(AATreeNode? node, T value, out T? result) {
		if (node is null) {
			result = default;
			return false;
		}

		var compare = value.CompareTo(node.value);

		// Find at left subtree to get smaller value.
		if (compare < 0) {
			if (_Lowerbound(node.left, value, out result)) {
				return true;
			}
			result = node.value;
			return true;
		}

		// Find at right subtree to get bigger value.
		if (compare > 0) {
			if (_Lowerbound(node.right, value, out result)) {
				return true;
			}
			result = default;
			return false;
		}

		result = node.value;
		return true;
	}

	public override string ToString() {
		return _NodeToString(this.root, this.root);
	}

	private static string _NodeToString(AATreeNode? root, AATreeNode? node) {
		if (node is null) {
			return string.Empty;
		}

		var sb = new System.Text.StringBuilder();

		if (node != node.left) {
			var leftExp = _NodeToString(root, node.left);
			if (leftExp.Length > 0) {
				sb.Append('{').Append(leftExp).Append('}').Append(" <- ");
			}
		}

		sb.Append(node.value).Append(node == root ? "(R)" : string.Empty);
		if (node.bucket?.Count > 0) {
			sb.Append('*').Append(node.bucket.Count);
		}

		if (node != node.right) {
			var rightExp = _NodeToString(root, node.right);
			if (rightExp.Length > 0) {
				sb.Append(" -> ").Append('{').Append(rightExp).Append('}');
			}
		}

		return sb.ToString();
	}

	internal class AATreeNode {
		/// <summary>
		/// User data.
		/// </summary>
		public T value;

		/// <summary>
		/// To rebalance tree (reduce height near to log(N)).
		/// </summary>
		internal int level;

		/// <summary>
		/// Node internal data.
		/// </summary>
		internal AATreeNode? left;
		internal AATreeNode? right;

		/// <summary>
		/// To store user data such that same value but does not equal.
		/// </summary>
		internal HashSet<T>? bucket;

		internal AATreeNode(T value, AATreeNode? left, AATreeNode? right) {
			this.left = left;
			this.right = right;
			this.value = value;
			this.level = 1;
		}
	}
}
