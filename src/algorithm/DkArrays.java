package tool.compet.algorithm;

public class DkArrays {
	public static int[] unique(int[] arr) {
		if (arr.length == 0) return new int[0];

		int len = 1;

		for (int i = 0, lastIndex = arr.length - 1; i < lastIndex; ++i) {
			if (arr[i] != arr[i + 1]) {
				++len; // increment since found new different element
			}
		}

		int[] res = new int[len];
		int cursor = 0;
		res[0] = arr[0];

		for (int i = 0, lastIndex = arr.length - 1; i < lastIndex; ++i) {
			if (arr[i] != arr[i + 1]) {
				res[++cursor] = arr[i + 1];
			}
		}

		return res;
	}

	public static void swap(int[] a, int i, int j) {
		if (i != j) {
			int tmp = a[i];
			a[i] = a[j];
			a[j] = tmp;
		}
	}
}
