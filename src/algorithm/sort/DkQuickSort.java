package tool.compet.algorithm.sort;

import tool.compet.algorithm.DkArrays;

import java.util.LinkedList;

import static tool.compet.algorithm.DkArrays.swap;

public class DkQuickSort {
	// worst case takes O(n^2)
	public void quicksort(int[] a, int left, int right) {
		LinkedList<int[]> stack = new LinkedList<>();
		stack.addLast(new int[]{left, right});
		while (stack.size() > 0) {
			int[] range = stack.removeLast();
			int start = range[0], end = range[1];

			int N = right - left + 1;
			if (N <= 45) {
				DkInsertionSort.insertionsort(a, start, end);
				continue;
			}

			int pivot = partition(a, start, end);
			stack.addLast(new int[]{start, pivot - 1});
			stack.addLast(new int[]{pivot + 1, end});
		}
	}

	private int partition(int[] a, int left, int right) {
		int mid = (left + right) >> 1, key = a[mid];
		DkArrays.swap(a, left, mid);
		int cur = left; // a[cur] <= key
		for (int i = left + 1; i <= right; ++i) {
			if (a[i] < key) {
				swap(a, ++cur, i);
			}
		}
		swap(a, cur, left);
		return cur;
	}

	public void quicksort3way(int[] a, int left, int right) {
		int N = right - left + 1;
		if (N <= 45) {
			DkInsertionSort.insertionsort(a, left, right);
			return;
		}
		int key = a[(left + right) >> 1];
		int lt = left - 1, gt = right + 1;
		for (int i = lt + 1; i < gt; ) {
			if (a[i] < key) swap(a, ++lt, i++);
			else if (a[i] > key) swap(a, --gt, i);
			else ++i;
		}
		quicksort3way(a, left, lt);
		quicksort3way(a, gt, right);
	}
}
