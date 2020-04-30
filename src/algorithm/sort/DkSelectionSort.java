package tool.compet.algorithm.sort;

import tool.compet.algorithm.DkArrays;

public class DkSelectionSort {
	public void selectionsort(int[] a, int left, int right) {
		for (int i = left; i <= right; ++i) {
			int minIndex = i;
			int min = a[i];

			for (int j = i + 1; j <= right; ++j) {
				if (min > a[j]) {
					min = a[j];
					minIndex = j;
				}
			}
			DkArrays.swap(a, i, minIndex);
		}
	}
}
