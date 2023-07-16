
public class Search {
	/// Find lower bound index such that: value >= target.
	/// Time: O(log(N))
	/// @param arr: Must be sort in asc.
	/// @return -1 means target is greater than value at _endIndex.
	public int FindLeftmostIndex(int[] arr, int target, int _startIndex, int _endIndex) {
		var startIndex = _startIndex;
		var endIndex = _endIndex;
		var midIndex = 0;

		while (startIndex < endIndex) {
			midIndex = (startIndex + endIndex) >> 1;
			if (target <= arr[midIndex]) { // Equals means Leftmost
				endIndex = midIndex;
			}
			else {
				startIndex = midIndex + 1;
			}
		}

		// Assert: startIndex == endIndex
		midIndex = startIndex;

		if (midIndex - 1 >= _startIndex && arr[midIndex - 1] >= target) {
			return midIndex - 1;
		}
		if (arr[midIndex] >= target) {
			return midIndex;
		}
		if (midIndex + 1 <= _endIndex && arr[midIndex + 1] >= target) {
			return midIndex + 1;
		}

		return -1;
	}

	/// Find upper bound index such that: value <= target.
	/// Time: O(log(N))
	/// @param arr: Must be sort in asc.
	/// @return -1 means target is smaller than value at _startIndex.
	public int FindRightmostIndex(int[] arr, int target, int _startIndex, int _endIndex) {
		var startIndex = _startIndex;
		var endIndex = _endIndex;
		var midIndex = 0;

		while (startIndex < endIndex) {
			midIndex = (startIndex + endIndex) >> 1;
			if (target < arr[midIndex]) {
				endIndex = midIndex;
			}
			else { // Equals means Rightmost
				startIndex = midIndex + 1;
			}
		}

		// Assert: startIndex == endIndex
		midIndex = endIndex;

		if (midIndex + 1 <= _endIndex && arr[midIndex + 1] <= target) {
			return midIndex + 1;
		}
		if (arr[midIndex] <= target) {
			return midIndex;
		}
		if (midIndex - 1 >= _startIndex && arr[midIndex - 1] <= target) {
			return midIndex - 1;
		}

		return -1;
	}
}
