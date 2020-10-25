package tool.compet.algorithm;

import java.util.Arrays;

// MO algorithms (Query square root decomposition)
public class DkSortQuery {
	public class Query {
		private int index;
		private int left, right;

		public Query(int id, int l, int r) {
			index = id;
			left = l;
			right = r;
		}
	}

	public int[] processQueries(int[] a, Query[] queries) {
		int size = queries.length, res[] = new int[size];

		final int blockSize = (int) Math.sqrt(size);
		Arrays.sort(queries, (q1, q2) -> {
			int l1 = q1.left / blockSize, l2 = q2.left / blockSize;
			if (l1 != l2) return l1 - l2;
			return q1.right - q2.right;
		});

		int curLeft = 0, curRight = 0, curRes = a[0];
		for (Query query : queries) {
			int left = query.left, right = query.right;
			while (curLeft < left) {
				// TODO: 2017/07/13 change here
				curRes -= a[curLeft++];
			}

			while (curLeft > left) {
				// TODO: 2017/07/13 change here
				curRes += a[--curLeft];
			}

			while (curRight < right) {
				// TODO: 2017/07/13 change here
				curRes += a[++curRight];
			}

			while (curRight > right) {
				// TODO: 2017/07/13 change here
				curRes -= a[curRight--];
			}
			res[query.index] = curRes;
		}
		return res;
	}

	public void test() {
		int a[] = {1, 2, 4, 3};
		Query q[] = new Query[]{new Query(0, 2, 3), new Query(1, 1, 2), new Query(2, 0, 3), new Query(3, 0, 1)};
		int res[] = processQueries(a, q);
		System.out.println(Arrays.toString(res));
	}
}
