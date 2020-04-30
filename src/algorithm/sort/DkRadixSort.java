package tool.compet.algorithm.sort;

public class DkRadixSort {
	// O(n.log10(maxElement))
	public void radixsort(int[] a, int left, int right) {
		int max = Integer.MIN_VALUE, digitCnt = 1;
		for (int i = left, ai; i <= right; ++i) {
			ai = a[i];
			if (ai < 0) ai = -ai;
			if (ai > max) max = ai;
		}
		while ((max /= 10) > 0) ++digitCnt;

		// counting sort
		int LENGTH = right - left + 1, work[] = new int[LENGTH];
		for (int i = 0, exp = 1; i < digitCnt; ++i, exp *= 10) {
			int count[] = new int[10];
			for (int j = left; j <= right; ++j) count[digit(a[j], exp)]++;

			int index[] = new int[10];
			for (int j = 1; j < 10; ++j) index[j] = index[j - 1] + count[j - 1];
			for (int j = left; j <= right; ++j) {
				int ai = a[j], id = digit(ai, exp);
				work[index[id]++] = ai;
			}

			System.arraycopy(work, left, a, left, LENGTH);
		}

		// resort if array a contains a negative element
		int l = left, r = right, neg = 0;
		for (int i = right, ai; i >= left; --i) {
			ai = a[i];
			if (ai < 0) {
				neg = 1;
				work[l++] = ai;
			} else {
				work[r--] = ai;
			}
		}
		if (neg == 1) System.arraycopy(work, left, a, left, LENGTH);
	}

	private int digit(int x, int exp) {
		x /= exp;
		if (x < 0) x = -x;
		return x % 10;
	}
}
