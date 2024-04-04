public class Solution {
	public static void Main(string[] args) {
		// Console.WriteLine(new Solution().CountAlternatingSubarrays([1, 0, 1, 0]));
		Console.WriteLine(new Solution().CountAlternatingSubarrays([0, 1, 1, 1]));
	}

	public class Node {
		public int value;
		public int first;
		public int last;

		public override string ToString() {
			return value + "," + first + "," + last;
		}
	}
	public long CountAlternatingSubarrays(int[] nums) {
		var N = nums.Length;
		if (N == 1) {
			return 1;
		}
		long ans = N;
		var segments = new List<Node>();

		for (var index = 0; index < N; ++index) {
			var sameCount = 0;
			var cur = nums[index];
			while (true) {
				if (index + 1 == N) {
					segments.Add(new Node { value = 1, first = cur, last = nums[index] });
					break;
				}
				if (nums[index] == nums[index + 1]) {
					++sameCount;
					if (++index == N - 1) {
						++index;
						break;
					}
				}
				else {
					break;
				}
			}
			if (sameCount > 0) {
				segments.Add(new Node { value = 0, first = cur, last = cur });
				if (index < N) {
					--index;
				}
				continue;
			}

			var diffCount = 0;
			while (true) {
				if (index + 1 == N) {
					segments.Add(new Node { value = 1, first = cur, last = nums[index] });
					break;
				}
				if (nums[index] != nums[index + 1]) {
					++diffCount;
					if (++index == N - 1) {
						++index;
						break;
					}
				}
				else {
					break;
				}
			}
			if (diffCount > 0) {
				segments.Add(new Node { value = diffCount + 1, first = cur, last = nums[index - 1] });
				--index;
			}
		}

		Console.WriteLine($"segments: {string.Join(',', segments)}");

		var list = new List<(Node, Node, Node)>();
		var M = segments.Count;
		for (var index = 0; index < M; ++index) {
			if (segments[index].value > 0) {
				list.Add((segments[index], index > 0 ? segments[index - 1] : null, index + 1 < M ? segments[index + 1] : null));
			}
		}

		foreach (var num in list) {
			var len = num.Item1.value + (num.Item2 == null ? 0 : (num.Item2.last == num.Item1.first ? 0 : 1)) + (num.Item3 == null ? 0 : (num.Item3.first == num.Item1.last ? 0 : 1));
			ans += 1L * len * (len - 1) / 2;
			Console.WriteLine($"{len}, {num}, ans: {ans}");
		}

		return ans;
	}
}
