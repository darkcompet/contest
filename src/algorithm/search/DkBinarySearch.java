package tool.compet.algorithm.search;

public class DkBinarySearch {
	/**
	 * Find arbitrary index of element which equals the key from sorted array.
	 *
	 * @param arr must be sorted in ascending order.
	 *
	 * @return arbitrary index of element which has value equals given key, or -1 if not found such key.
	 */
	public static int arbitrarySearch(int[] arr, int fromIndex, int toIndex, int key) {
		if (arr[fromIndex] > key || arr[toIndex] < key) return -1;

		int low = fromIndex;
		int high = toIndex;
		int mid, val;

		while (low <= high) {
			mid = (low + high) >>> 1;
			val = arr[mid];

			if (key > val) {
				low = mid + 1;
			}
			else if (key < val) {
				high = mid - 1;
			}
			else {
				return mid;
			}
		}

		return -1;
	}

	/**
	 * Find first element which equals the key from sorted array.
	 *
	 * @param arr must be sorted in ascending order.
	 * @param unique because arr maybe contains duplicated values, so you must give us unique array from arr, you can use #DkArrays.unique() to create it.
	 *
	 * @return first index of element which has value equals given key, or -1 if not found such key.
	 */
	public static int lowerSearch(int[] arr, int[] unique, int fromIndex, int toIndex, int key) {
		if (arr[fromIndex] > key || arr[toIndex] < key) return -1;

		int low = fromIndex;
		int high = toIndex;
		int mid, val;

		while (low <= high) {
			mid = (low + high) >>> 1;
			val = unique[mid];

			if (key > val) {
				low = mid + 1;
			}
			else if (key < val) {
				high = mid - 1;
			}
			else {
				return mid;
			}
		}

		return -1;
	}

	/**
	 * Find last element which equals the key from sorted array.
	 *
	 * @param arr must be sorted in ascending order.
	 * @param unique because arr maybe contains duplicated values, so you must give us unique array
	 *    from arr, you can use #DkArrays.unique() to create it.
	 *
	 * @return last index of element which has value equals given key, or -1 if not found such key.
	 */
	public static int upperSearch(boolean asc, int[] arr, int[] unique, int fromIndex, int toIndex, int key) {
		if (arr[fromIndex] > key || arr[toIndex] < key) return -1;

		int low = fromIndex;
		int high = toIndex;
		int mid, val;

		while (low <= high) {
			mid = (low + high) >>> 1;
			val = arr[mid];

			if (key > val) {
				low = mid + 1;
			}
			else if (key < val) {
				high = mid - 1;
			}
			else {
				return mid;
			}
		}

		return -1;
	}

	/**
	 * From sorted array which contains "duplicated elements", this method is binary search extension,
	 * find index of last element which smaller than given key, that is,
	 * last index such that: arr[index] < key <= arr[index + 1].
	 * <p></p>
	 * In other words, if we have an array a[1, n], and lowerBound() return index i >= 0,
	 * then we can say: all elements in a[1, i] < key and all elements in a[i+1, n] >= key.
	 * <p></p>
	 * For example, we have an array: [(1, 2, 3), 4, 4, 4, 4, (5, 6, 7, 8, 9)]
	 * then lowerBound() on whole array with key 4 will return index 2 (at element 3).
	 * <p></p>
	 * This method takes O(logN) time.
	 *
	 * @param arr must be sorted in ascending order.
	 *
	 * @return index of last element which smaller than given key, or -1 if key is out of such range.
	 */
	public static int lowerBound(int[] arr, int fromIndex, int toIndex, int key) {
		if (arr[fromIndex] >= key) return -1;
		if (arr[toIndex] < key) return toIndex;

		int low = fromIndex + 1;
		int high = toIndex;
		int mid;

		// this loop guarantee: arr[low - 1] < key <= arr[high].
		while (low < high) {
			mid = (low + high) >>> 1; // low <= mid < high

			if (key > arr[mid]) {
				low = mid + 1;
			}
			else {
				high = mid;
			}
		}

		return low - 1;
	}

	/**
	 * From sorted array which contains "duplicated elements", this method is binary search extension,
	 * find index of first element which smaller than given key, that is,
	 * last index such that: arr[index - 1] >= key > arr[index].
	 * <p></p>
	 * In other words, if we have an array a[1, n], and lowerBound() return index i >= 0,
	 * then we can say: all elements in a[1, i-1] >= key and all elements in a[i, n] < key.
	 * <p></p>
	 * For example, we have an array: [(9, 8, 7, 6, 5), 4, 4, 4, 4, (3, 2, 1)]
	 * then lowerBound() on whole array with key 4 will return index 9 (at element 3).
	 * <p></p>
	 * This method takes O(logN) time.
	 *
	 * @param arr must be sorted in descending order.
	 *
	 * @return index of first element which smaller than given key, or -1 if key is out of such range.
	 */
	public static int lowerBoundDsc(int[] arr, int fromIndex, int toIndex, int key) {
		if (arr[fromIndex] < key) return fromIndex;
		if (arr[toIndex] >= key) return -1;

		int low = fromIndex + 1;
		int high = toIndex;
		int mid;

		// this loop guarantee: arr[high] < key <= arr[low - 1].
		while (low < high) {
			mid = (low + high) >>> 1; // low <= mid < high

			if (key > arr[mid]) {
				high = mid;
			}
			else {
				low = mid + 1;
			}
		}

		return high;
	}

	/**
	 * From sorted array which contains "duplicated elements", this method is binary search extension,
	 * find index of first element which greater than given key,
	 * that is, first index such that: arr[index - 1] <= key < arr[index].
	 * <p></p>
	 * In other words, if we have an array a[1, n], and upperBound() return index i > 0,
	 * then we can say: all elements in a[1, i - 1] <= key and all elements in a[i, n] > key.
	 * <p></p>
	 * For example, we have an array: [(1, 2, 3), 4, 4, 4, 4, (5, 6, 7, 8, 9)]
	 * then lowerBound() on whole array with key 4 will return index 7 (at element 5).
	 * <p></p>
	 * This method takes O(logN) time.
	 *
	 * @param arr must be sorted in ascending order.
	 *
	 * @return index of first element which greater than given key, or -1 if key is out of such range.
	 */
	public static int upperBound(int[] arr, int fromIndex, int toIndex, int key) {
		if (arr[fromIndex] > key) return fromIndex;
		if (arr[toIndex] <= key) return -1;

		int low = fromIndex + 1;
		int high = toIndex;
		int mid;

		// this loop guarantee: arr[low - 1] <= key < arr[high]
		while (low < high) {
			mid = (low + high) >>> 1; // low <= mid < high

			if (key < arr[mid]) {
				high = mid;
			}
			else {
				low = mid + 1;
			}
		}

		return high;
	}

	/**
	 * From sorted array which contains "duplicated elements", this method is binary search extension,
	 * find index of last element which greater than given key,
	 * that is, last index such that: arr[index] > key >= arr[index + 1].
	 * <p></p>
	 * In other words, if we have an array a[1, n], and upperBound() return index i > 0,
	 * then we can say: all elements in a[1, i] > key and all elements in a[i + 1, n] <= key.
	 * <p></p>
	 * For example, we have an array: [(9, 8, 7, 6, 5), 4, 4, 4, 4, (3, 2, 1)]
	 * then lowerBound() on whole array with key 4 will return index 4 (at element 5).
	 * <p></p>
	 * This method takes O(logN) time.
	 *
	 * @param arr must be sorted in descending order.
	 *
	 * @return index of last element which greater than given key, or -1 if key is out of such range.
	 */
	public static int upperBoundDsc(int[] arr, int fromIndex, int toIndex, int key) {
		if (arr[fromIndex] <= key) return fromIndex;
		if (arr[toIndex] > key) return -1;

		int low = fromIndex + 1;
		int high = toIndex;
		int mid;

		// this loop guarantee: arr[high] <= key < arr[low - 1]
		while (low < high) {
			mid = (low + high) >>> 1; // low <= mid < high

			if (key < arr[mid]) {
				low = mid + 1;
			}
			else {
				high = mid;
			}
		}

		return low - 1;
	}
}
