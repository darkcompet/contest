namespace Compet.Contest;

public class RadixSort {
	/// <summary>
	/// Radix sort on the range of given array.
	/// </summary>
	/// <param name="arr"></param>
	/// <param name="startIndex">Inclusive</param>
	/// <param name="endIndex">Inclusive</param>
	public void Sort(int[] arr, int startIndex, int endIndex) {
		if (startIndex >= endIndex) {
			return;
		}
		const int maxDigitCount = 10;
		var maxElm = arr.Max();
		var elmCount = endIndex - startIndex + 1;
		var workArr = new int[elmCount];
		var digitCount = new int[maxDigitCount];
		for (var k = 1; k <= maxElm; k *= 10) {
			Array.Fill(digitCount, 0);
			for (var index = startIndex; index <= endIndex; ++index) {
				var lastDigit = arr[index] / k % 10;
				++digitCount[lastDigit];
			}
			// Counting sort
			for (var index = 1; index < maxDigitCount; ++index) {
				digitCount[index] += digitCount[index - 1];
			}
			// Must use inversed loop
			for (var index = endIndex; index >= startIndex; --index) {
				var lastDigit = arr[index] / k % 10;
				workArr[--digitCount[lastDigit]] = arr[index];
			}
			Array.Copy(workArr, 0, arr, startIndex, elmCount);
		}
	}
}
