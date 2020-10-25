package tool.compet.algorithm.sort;

import tool.compet.algorithm.DkArrays;

public class DkHeapSort {
	public void heapsort(int[] a) {
		makeheap(a);

		int len = a.length;

		while (len > 0) {
			DkArrays.swap(a, 0, --len);
			downheap(a, 0, len);
		}
	}

	private void makeheap(int[] a) {
		for (int i = a.length >> 1; i >= 0; --i) {
			downheap(a, i, a.length);
		}
	}

	private void downheap(int[] a, int id, int len) {
		int idLeft = (id << 1) | 1, idRight = idLeft + 1;

		if (idLeft >= len) {
			return;
		}

		int idBig = idLeft;

		if (idRight < len && a[idRight] > a[idLeft]) {
			idBig = idRight;
		}
		if (a[id] < a[idBig]) {
			DkArrays.swap(a, id, idBig);
		}

		downheap(a, idBig, len);
	}
}
