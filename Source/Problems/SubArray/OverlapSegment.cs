namespace Compet.Problems;

public class OverlapSegment {
	/// <summary>
	/// Input: overlapped ranges [(start, end)] on the infinite line.
	/// Output: count elements (on the line) that be covered by the ranges.
	/// Time: O(N), where N is length of `overlapRanges`.
	///</summary>
	public int CountCoveredElements(int[][] overlapRanges) {
		var N = overlapRanges.Length;
		if (N == 0) {
			return 0;
		}
		if (N == 1) {
			return overlapRanges[0][1] - overlapRanges[0][0] + 1;
		}

		// Sort as ascending of `start`
		Array.Sort(overlapRanges, (a, b) => { return a[0] - b[0]; });

		// Merge segments to [start, end]
		var start = overlapRanges[0][0];
		var end = overlapRanges[0][1];
		var nonOverlapRanges = new List<(int, int)>();
		var lastIndex = N - 1;
		for (var i = 1; i <= lastIndex; ++i) {
			var nextRange = overlapRanges[i];
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

		// Sum up elements that be occupied by segments
		var covered = 0;
		foreach (var (s, e) in nonOverlapRanges) {
			covered += e - s + 1;
		}

		return covered;
	}
}
