package tool.compet.algorithm.sort;

import tool.compet.algorithm.DkArrays;

public class DkBubbleSort {
	public void bubblesort(int[] a, int left, int right) {
		for (int d = 0; d < right - left; ++d) {
			for (int i = left; i < right - d; ++i) {
				if (a[i] > a[i + 1]) {
					DkArrays.swap(a, i, i + 1);
				}
			}
		}
	}
}
