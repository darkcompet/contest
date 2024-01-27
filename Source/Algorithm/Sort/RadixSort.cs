namespace Compet.Contest;

public class RadixSort {
	public void Sort(int[] arr) {
		var N = arr.Length;
		const int M = 10;
		var maxElm = arr.Max();
		var workArr = new int[N];
		var count = new int[M];
		for (var k = 1; k <= maxElm; k *= 10) {
			Array.Fill(count, 0);
			for (var index = 0; index < N; ++index) {
				++count[arr[index] / k % 10];
			}
			// Counting sort
			for (var index = 1; index < M; ++index) {
				count[index] += count[index - 1];
			}
			// Must use inversed loop
			for (var index = N - 1; index >= 0; --index) {
				workArr[--count[arr[index] / k % 10]] = arr[index];
			}
			Array.Copy(workArr, arr, N);
		}
	}
}
