namespace Compet.Problems;

public class Segment {
	/// <summary>
	/// Input: Overlapable ranges.
	/// Output: Non-overlap ranges after merge given ranges.
	/// Time: O(N), where N is length of given `ranges`.
	///</summary>
	public List<(int, int)> MergeSegments(int[][] ranges) {
		var nonOverlapRanges = new List<(int, int)>();
		var N = ranges.Length;
		if (N == 0) {
			return nonOverlapRanges;
		}
		if (N == 1) {
			nonOverlapRanges.Add((ranges[0][0], ranges[0][1]));
			return nonOverlapRanges;
		}

		// Sort as ascending of `start`
		Array.Sort(ranges, (a, b) => { return a[0] - b[0]; });

		// Merge segments to [start, end]
		var start = ranges[0][0];
		var end = ranges[0][1];
		var lastIndex = N - 1;
		for (var i = 1; i <= lastIndex; ++i) {
			var nextRange = ranges[i];
			var nextStart = nextRange[0];
			var nextEnd = nextRange[1];
			// Merge with next range
			if (nextStart <= end) {
				end = Math.Max(end, nextEnd);
			}
			// Add current range as new range
			else {
				nonOverlapRanges.Add((start, end));
				start = nextStart;
				end = nextEnd;
			}

			// Add if meets last segment
			if (i == lastIndex) {
				nonOverlapRanges.Add((start, end));
			}
		}

		return nonOverlapRanges;
	}
}
