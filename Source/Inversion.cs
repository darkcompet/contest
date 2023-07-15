using System.Runtime.CompilerServices;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

/// For all 2 indices i < j such that a[i] > a[j], it counts all such pairs.
/// Approach: divide and conquer.
/// Time complexity: N * log(N)
public class Inversion {
	int CountInversions(int[] arr) {
		return CountInversions(arr, 0, arr.Length - 1);
	}

	int CountInversions(int[] arr, int startIndex, int endIndex) {
		if (startIndex >= endIndex) {
			return 0;
		}

		// Divide and Conquer (on independent subarray)
		var midIndex = (startIndex + endIndex) >> 1;
		var leftResult = CountInversions(arr, startIndex, midIndex);
		var rightResult = CountInversions(arr, midIndex + 1, endIndex);

		return leftResult + rightResult + binaryResult(arr, startIndex, midIndex, midIndex + 1, endIndex);
	}

	private int binaryResult(int[] arr, int start1, int end1, int start2, int end2) {
		Array.Sort(arr, start2, end2 - start2 + 1);

		var ans = 0;
		for (var index = start1; index <= end1; ++index) {
			var smallerIndex = findIndexOfSmaller(arr, arr[index], start2, end2);
			if (smallerIndex >= 0) {
				ans += smallerIndex - start2 + 1;
			}
		}

		return ans;
	}

	int findIndexOfSmaller(int[] arr, int value, int _startIndex, int _endIndex) {
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
