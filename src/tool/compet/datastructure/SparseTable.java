package tool.compet.datastructure;

import tool.compet.algorithm.DkMaths;

public class SparseTable {
	private int[] data;
	private int[][] T;

	public void build(int[] a) {
		data = a;
		int n = a.length, m = DkMaths.log2(n) + 1;
		T = new int[n][m];
		// T[i][j] holds data segment with length 2^j from data[i -> i+2^j-1]
		for (int i = 0; i < n; ++i) {
			// TODO: init data for first segment T[i][0]
			T[i][0] = i;
		}

		for (int j = 1; j < m; ++j) {
			for (int i = 0; i < n; ++i) {
				int next = i + (1 << (j - 1));
				if (next >= n) break;
				// TODO: init data for T[i][j]
				int dat1 = T[i][j - 1], dat2 = T[next][j - 1];
				T[i][j] = a[dat1] < a[dat2] ? dat1 : dat2;
			}
		}
	}

	public int query(int left, int right) {
		int res = left, length = right - left + 1;
		// assume left = 0, right = 12, then cnt = 13 (1101) = 2^3 + 2^2 + 2^0
		// then divide [0 -> 12] to [0 -> 7] + [8 -> 11] + [12 -> 12]
		while (length > 0) {
			int r = DkMaths.log2(length), id = T[left][r];

			// TODO: query on segment [left -> left + 2^r -1]
			if (data[id] < data[res]) res = id;

			left += (1 << r);
			length -= (1 << r);
		}
		return res;
	}

	public void test() {
		int a[] = {1, 3, 2, -1, 5, 4};
		build(a);
		System.out.println("min at index: " + query(1, 2));
		System.out.println("min at index: " + query(3, 5));
		System.out.println("min at index: " + query(2, 2));
		System.out.println("min at index: " + query(0, a.length - 1));
	}
}
