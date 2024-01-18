namespace Compet.Contest;

public class Inversion {
	public int CountInversions(int[] arr) {
		return CountInversions(arr, 0, arr.Length - 1);
	}

	/// Count all pairs such that: at 2 indices i < j we have a[i] > a[j].
	/// Approach: divide and conquer.
	/// Time complexity: N * log(N)
	/// @param startIndex: Inclusive
	/// @param endIndex: Inclusive
	public int CountInversions(int[] arr, int startIndex, int endIndex) {
		if (startIndex >= endIndex) {
			return 0;
		}

		// Divide and Conquer (on independent subarray)
		var midIndex = (startIndex + endIndex) >> 1;
		var leftResult = CountInversions(arr, startIndex, midIndex);
		var rightResult = CountInversions(arr, midIndex + 1, endIndex);

		return leftResult + rightResult + _Conquer(arr, startIndex, midIndex, midIndex + 1, endIndex);
	}

	private int _Conquer(int[] arr, int start1, int end1, int start2, int end2) {
		// Sort on independent subarray after divided
		Array.Sort(arr, start2, end2 - start2 + 1);

		var smallerCount = 0;
		for (var index = start1; index <= end1; ++index) {
			var smallerIndex = _FindIndexOfSmaller(arr, arr[index], start2, end2);
			if (smallerIndex >= start2) {
				smallerCount += smallerIndex - start2 + 1;
			}
		}

		return smallerCount;
	}

	private int _FindIndexOfSmaller(int[] arr, int value, int _startIndex, int _endIndex) {
		var startIndex = _startIndex;
		var endIndex = _endIndex;
		var midIndex = 0;

		while (startIndex < endIndex) {
			midIndex = (startIndex + endIndex) >> 1;

			if (value <= arr[midIndex]) {
				endIndex = midIndex;
			}
			else {
				startIndex = midIndex + 1;
			}
		}

		if (startIndex != endIndex) {
			Console.WriteLine("---> Oops");
		}

		midIndex = (startIndex + endIndex) >> 1;

		if (midIndex + 1 <= _endIndex && value > arr[midIndex + 1]) {
			return midIndex + 1;
		}
		if (value > arr[midIndex]) {
			return midIndex;
		}
		if (midIndex - 1 >= _startIndex && value > arr[midIndex - 1]) {
			return midIndex - 1;
		}

		return -1;
	}
}
