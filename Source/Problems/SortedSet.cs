namespace Compet.Problems;

public class SortedSet {
	public class Node {
		public int index;
		public int num;
	}
	public long[] UnmarkedSumArray(int[] nums, int[][] queries) {
		var N = nums.Length;
		var ans = new long[queries.Length];
		var marked = new bool[N];
		var sumOfUnmarked = nums.Select(m => (long)m).Sum();

		var nodes = new List<Node>();
		var index2node = new Dictionary<int, Node>();
		for (var index = 0; index < N; ++index) {
			var node = new Node() { index = index, num = nums[index] };
			index2node[index] = node;
			nodes.Add(node);
		}

		// index, num
		var heap = new SortedSet<Node>(nodes, Comparer<Node>.Create((a, b) => { return a.num == b.num ? a.index - b.index : a.num - b.num; }));
		// var heap = new PriorityQueue<int, (int, int)>();

		var step = 0;
		foreach (var q in queries) {
			var markIndex = q[0];
			var markMore = q[1];
			if (!marked[markIndex]) {
				marked[markIndex] = true;
				var removeNode = index2node[markIndex];
				index2node.Remove(markIndex);
				heap.Remove(removeNode);
				sumOfUnmarked -= nums[markIndex];
			}

			while (markMore-- > 0 && heap.Count > 0) {
				var markNode = heap.Min!;

				marked[markNode.index] = true;
				heap.Remove(markNode);
				index2node.Remove(markNode.index);
				sumOfUnmarked -= markNode.num;
			}

			ans[step++] = sumOfUnmarked;
		}

		return ans;
	}
}
