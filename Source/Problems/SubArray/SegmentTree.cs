namespace Compet.Problems;

public class SegmentTree {
	/// <summary>
	/// Minimize OR over all sub array.
	/// Time: O(N * logN)
	/// </summary>
	/// <param name="nums"></param>
	/// <param name="K"></param>
	/// <returns></returns>
	public int MinimizeOr(int[] nums, int K) {
		var ans = int.MaxValue;
		var N = nums.Length;

		// We need 4N nodes
		var tree = new int[N << 2];
		Build(tree, nums, 1, 0, N - 1);

		var count = 0;
		for (var index = 0; index < N; ++index) {
			// Check for subarrays starting with index i
			var low = index;
			var high = N - 1;
			var minPos = int.MaxValue;
			while (low <= high) {
				var mid = (low + high) >> 1;

				// If OR of subarray [i..mid] >= K, then all subsequent subarrays will have OR >= K,
				// therefore reduce high to mid - 1 to find the minimal length subarray [i..mid] having OR >= K
				if (Query(tree, 1, 0, N - 1, index, mid) >= K) {
					minPos = Math.Min(minPos, mid);
					high = mid - 1;
				}
				else {
					low = mid + 1;
				}
			}

			// Increase count with number of subarrays
			// having OR >= K and starting with index i
			if (minPos != int.MaxValue) {
				count += N - minPos;
				ans = Math.Min(ans, minPos - index + 1);
			}
		}

		return ans == int.MaxValue ? -1 : ans;
	}

	// Builds the segment tree
	private static void Build(int[] tree, int[] arr, int node, int start, int end) {
		if (start == end) {
			tree[node] = arr[start];
			return;
		}
		var mid = (start + end) >> 1;
		var leftNode = node << 1;
		var rightNode = leftNode + 1;
		Build(tree, arr, leftNode, start, mid);
		Build(tree, arr, rightNode, mid + 1, end);

		// Aggregate here
		tree[node] = tree[leftNode] | tree[rightNode];
	}

	// Calculate bitwise OR of segment [L..R]
	private static int Query(int[] tree, int node, int start, int end, int left, int right) {
		if (start > end || start > right || end < left) {
			return 0;
		}

		if (start >= left && end <= right) {
			return tree[node];
		}

		var mid = (start + end) >> 1;
		var leftNode = node << 1;
		var rightNode = leftNode + 1;
		var q1 = Query(tree, leftNode, start, mid, left, right);
		var q2 = Query(tree, rightNode, mid + 1, end, left, right);

		// Aggregate here
		return q1 | q2;
	}
}
