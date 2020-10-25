package tool.compet.problem;

import java.util.Arrays;

// inversion of an array is invariant parameter which be used to check
// the array is sorted or not. In detail, inversion is number of pair
// (ai, aj) such that: ai > aj with i < j.
// If it is sorted, then inversion(array) == 0.
public class Inversion {
	public long inversion_divideconquer(int[] a) {
		int n = a.length, work[] = new int[n];
		return inversion_divideconquer(a, work, 0, n - 1);
	}

	private long inversion_divideconquer(int[] a, int[] work, int left, int right) {
		int LENGTH = right - left + 1;
		if (LENGTH <= 1) return 0;
		long res = 0;
		int mid = (left + right) >> 1;
		long inv1 = inversion_divideconquer(a, work, left, mid);
		long inv2 = inversion_divideconquer(a, work, mid + 1, right);
		// merge
		for (int i = left, j = mid + 1, cur = left; cur <= right; ++cur) {
			if (i <= mid && j <= right && a[i] > a[j]) {
				work[cur] = a[i++];
				res += right - j + 1;
			} else if (i <= mid && j > right) {
				work[cur] = a[i++];
			} else if (j <= right) {
				work[cur] = a[j++];
			}
		}
		System.arraycopy(work, left, a, left, LENGTH);
		return res + inv1 + inv2;
	}

	public long inversion_fenwicktree(int[] a) {
		// convert a[] to indexed array
		int N = a.length;
		int b[] = new int[N];
		System.arraycopy(a, 0, b, 0, N);
		Arrays.sort(b);
		for (int i = 0; i < N; ++i) a[i] = Arrays.binarySearch(b, a[i]);

		long inv = 0;
		int mark[] = new int[N];
		FenwickTree fw = new FenwickTree();
		fw.build(mark);
		for (int i = N - 1; i >= 0; --i) {
			int id = a[i];
			inv += fw.query(id - 1);
			fw.add(id, 1);
		}
		return inv;
	}

	private class FenwickTree {
		private int[] T;

		public void build(int[] a) {
			T = new int[a.length + 1];
			for (int i = 0; i < a.length; ++i) {
				add(i, a[i]);
			}
		}

		public void add(int i, int val) {
			// add val to T[i] and its ancestors
			for (++i; i < T.length; i += (i & -i)) {
				T[i] += val;
			}
		}

		public long query(int i) {
			long sum = 0;
			for (++i; i > 0; i -= (i & -i)) {
				sum += T[i];
			}
			return sum;
		}
	}

	public long inversion_avltree(int[] a) {
		long inv = 0;
		return inv;
	}

	public void test() {
		int a[] = {8, 4, 2, 1};
//		System.out.println(inversion_divideconquer(a));
		System.out.println(inversion_fenwicktree(a));
//		System.out.println(inversion_avltree(a));
	}
}
