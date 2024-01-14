
public class BinarySearch {
	/// Find first index such that element >= target.
	/// Time: O(log(N)) where N is length of the values.
	/// @param arr: Must be sort in asc.
	/// @return -1 means target is greater than value at _endIndex.
	public int LowerBound(int[] values, int target, int startIndex, int endIndex) {
		if (startIndex < 0 || startIndex > endIndex || endIndex >= values.Length) {
			return -1;
		}
		var midIndex = 0;
		while (startIndex < endIndex) {
			midIndex = (startIndex + endIndex) >> 1;
			if (target <= values[midIndex]) { // Equals means Leftmost
				endIndex = midIndex;
			}
			else {
				startIndex = midIndex + 1;
			}
		}

		// Assert: startIndex == endIndex
		midIndex = startIndex;

		if (midIndex - 1 >= startIndex && values[midIndex - 1] >= target) {
			return midIndex - 1;
		}
		if (values[midIndex] >= target) {
			return midIndex;
		}
		if (midIndex + 1 <= endIndex && values[midIndex + 1] >= target) {
			return midIndex + 1;
		}

		return -1;
	}

	/// Find last index that element <= target.
	/// Time: O(log(N)) where N is length of the values.
	/// @param arr: Must be sort in asc.
	/// @return -1 means target is smaller than value at _startIndex.
	public int UpperBound(int[] values, int target, int startIndex, int endIndex) {
		if (startIndex < 0 || startIndex > endIndex || endIndex >= values.Length) {
			return -1;
		}
		var midIndex = 0;

		while (startIndex < endIndex) {
			midIndex = (startIndex + endIndex) >> 1;
			if (target < values[midIndex]) {
				endIndex = midIndex;
			}
			else { // Equals means Rightmost
				startIndex = midIndex + 1;
			}
		}

		// Assert: startIndex == endIndex
		midIndex = endIndex;

		if (midIndex + 1 <= endIndex && values[midIndex + 1] <= target) {
			return midIndex + 1;
		}
		if (values[midIndex] <= target) {
			return midIndex;
		}
		if (midIndex - 1 >= startIndex && values[midIndex - 1] <= target) {
			return midIndex - 1;
		}

		return -1;
	}
}
