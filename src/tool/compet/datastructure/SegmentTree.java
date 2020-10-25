package tool.compet.datastructure;

public class SegmentTree {
	private Node T[];

	public class Node {
		int L, R;
		double dat;

		Node(int left, int right) {
			L = left;
			R = right;
		}
	}

	public void build(int[] a) {
		int n = a.length;
		T = new Node[n << 2];
		build(a, 1, 0, n - 1);
	}

	private void build(int[] a, int id, int L, int R) {
		if (T[id] == null) T[id] = new Node(L, R);
		int mid = (L + R) >> 1;
		if (L == R) {
			// TODO: setup dat for leaf node
			T[id].dat = a[mid];
		} else {
			int idLeft = (id << 1), idRight = idLeft | 1;
			build(a, idLeft, L, mid);
			build(a, idRight, mid + 1, R);
			// TODO: setup dat for node id
			T[id].dat = T[idLeft].dat + T[idRight].dat;
		}
	}

	public double query(int id, int left, int right) {
		int L = T[id].L, R = T[id].R;
		if (left > right || left > R || right < L) return Double.MIN_VALUE;

		if (left == L && right == R) {
			return T[id].dat;
		}

		int mid = (L + R) >> 1;
		int idLeft = (id << 1), idRight = idLeft | 1;
		if (left > mid) return query(idRight, left, right);
		if (right <= mid) return query(idLeft, left, right);
		double datLeft = query(idLeft, left, mid);
		double datRight = query(idRight, mid + 1, right);
		// TODO: calculate dat over [left -> right]
		return datLeft + datRight;
	}

	public void test() {
		int a[] = {1, 3, 2, 4, 16, 5};
		build(a);
		System.out.println(query(1, 2, 5));
	}
}
