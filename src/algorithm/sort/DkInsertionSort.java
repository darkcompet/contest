package tool.compet.algorithm.sort;

public class DkInsertionSort {
	public static void insertionsort(int[] a, int left, int right) {
		for (int i = left + 1; i <= right; ++i) {
			int j = i - 1, ai = a[i];
			while (j >= left && a[j] > ai) {
				a[j + 1] = a[j--];
			}
			a[j + 1] = ai;
		}
	}
}
