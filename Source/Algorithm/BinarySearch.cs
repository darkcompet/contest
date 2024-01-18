namespace Compet.Algorithm;

public class BinarySearch {
	/// <summary>
	/// Find first index such that value >= minValue.
	/// Time: O(log(N)) where N is length of the values.
	/// </summary>
	/// <param name="values">Must be sorted as ASC</param>
	/// <param name="minValue"></param>
	/// <param name="startIndex"></param>
	/// <param name="endIndex"></param>
	/// <returns>Index of first value that >= target. Return -1 if not found such value.</returns>
	public int LowerBound(int[] values, int minValue, int startIndex, int endIndex) {
		if (startIndex < 0 || startIndex > endIndex || endIndex >= values.Length) {
			return -1;
		}
		var midIndex = 0;
		while (startIndex < endIndex) {
			midIndex = (startIndex + endIndex) >> 1;
			// Equals means Leftmost
			if (minValue <= values[midIndex]) {
				endIndex = midIndex;
			}
			else {
				startIndex = midIndex + 1;
			}
		}

		// Assert: startIndex == endIndex
		midIndex = startIndex;

		if (midIndex - 1 >= startIndex && values[midIndex - 1] >= minValue) {
			return midIndex - 1;
		}
		if (values[midIndex] >= minValue) {
			return midIndex;
		}
		if (midIndex + 1 <= endIndex && values[midIndex + 1] >= minValue) {
			return midIndex + 1;
		}

		return -1;
	}

	/// <summary>
	/// Find last index such that value <= maxValue.
	/// Time: O(log(N)) where N is length of the values.
	/// </summary>
	/// <param name="values">Must be sorted as ASC</param>
	/// <param name="maxValue"></param>
	/// <param name="startIndex"></param>
	/// <param name="endIndex"></param>
	/// <returns>Index of first value that <= target. Return -1 if not found such value.</returns>
	public int UpperBound(int[] values, int maxValue, int startIndex, int endIndex) {
		if (startIndex < 0 || startIndex > endIndex || endIndex >= values.Length) {
			return -1;
		}
		var midIndex = 0;

		while (startIndex < endIndex) {
			midIndex = (startIndex + endIndex) >> 1;
			if (maxValue < values[midIndex]) {
				endIndex = midIndex;
			}
			// Equals means Rightmost
			else {
				startIndex = midIndex + 1;
			}
		}

		// Assert: startIndex == endIndex
		midIndex = endIndex;

		if (midIndex + 1 <= endIndex && values[midIndex + 1] <= maxValue) {
			return midIndex + 1;
		}
		if (values[midIndex] <= maxValue) {
			return midIndex;
		}
		if (midIndex - 1 >= startIndex && values[midIndex - 1] <= maxValue) {
			return midIndex - 1;
		}

		return -1;
	}
}
