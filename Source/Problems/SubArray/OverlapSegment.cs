namespace Compet.Problems;

public class OverlapSegment {
	/// <summary>
	/// We have overlapped ranges [s, e] on the infinite line.
	/// We need count elements (on the line) that be covered by the ranges.
	/// Time: O(N), where N is length of `overlapRanges`.
	///</summary>
	public int CountOccupiedElements(int[][] overlapRanges) {
		var N = overlapRanges.Length;

		// Sort as ascending of `start`
		Array.Sort(overlapRanges, (a, b) => { return a[0] - b[0]; });

		// Merge segments to [start, end]
		var start = overlapRanges[0][0];
		var end = overlapRanges[0][1];
		var nonOverlapRanges = new List<(int, int)>();
		if (N == 1) {
			nonOverlapRanges.Add((start, end));
		}
		for (var i = 1; i < N; ++i) {
			var range = overlapRanges[i];
			var curStart = range[0];
			var curEnd = range[1];
			// Merge current range
			if (curStart <= end) {
				end = Math.Max(end, curEnd);
			}
			// Add as new segment
			else {
				nonOverlapRanges.Add((start, end));
				start = curStart;
				end = curEnd;
			}

			// Add if meets last segment
			if (i == N - 1) {
				nonOverlapRanges.Add((start, end));
			}
		}

		// Sum up elements that be occupied by segments
		var occupied = 0;
		foreach (var (s, e) in nonOverlapRanges) {
			occupied += e - s + 1;
		}

		return occupied;
	}
}
