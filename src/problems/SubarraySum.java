package tool.compet.problem;

import tool.compet.algorithm.DkMaths;

public class SubarraySum {
	// take O(n)
	public static long maxSumOfSubarray(int[] a) {
		long maxsum = 0, seqsum = 0;
		for (int i = 0, N = a.length; i < N; ++i) {
			seqsum = Math.max(a[i], seqsum + a[i]);
			if (maxsum < seqsum) maxsum = seqsum;
		}
		return maxsum;
	}

	// max-sum subarray through an element of the array, take O(n)
	public static long maxSumOfSubarray_dp(int[] a) {
		final int n = a.length;
		long maxLeft[] = new long[n], maxRight[] = new long[n];

		maxLeft[0] = 0;
		for (int i = 1; i < n; ++i) {
			maxLeft[i] = DkMaths.max(0, a[i - 1], a[i - 1] + maxLeft[i - 1]);
		}

		maxRight[n - 1] = 0;
		for (int i = n - 2; i >= 0; --i) {
			maxRight[i] = DkMaths.max(0, a[i + 1], a[i + 1] + maxRight[i + 1]);
		}

		long max = Long.MIN_VALUE;
		for (int i = 0; i < n; ++i) {
			max = Math.max(max, a[i] + maxLeft[i] + maxRight[i]);
		}

		return max;
	}

	// take O(nlog(n))
	public static long maxSumOfSubarray_divideconquer(int[] a, int left, int right) {
		if (left > right) return 0;
		if (left == right) return a[left];
		if (left + 1 == right) return Math.max(a[left], a[right]);

		final int n = a.length, mid = (right - left) >> 1;
		long maxleft = 0, maxright = 0, leftsum = 0, rightsum = 0;
		for (int i = mid; i >= 0; --i) {
			leftsum += a[i];
			if (maxleft < leftsum) maxleft = leftsum;
		}
		for (int i = mid + 1; i < n; ++i) {
			rightsum += a[i];
			if (maxright < rightsum) maxright = rightsum;
		}

		long onMid = maxleft + maxright;
		long onLeft = maxSumOfSubarray_divideconquer(a, 0, mid);
		long onRight = maxSumOfSubarray_divideconquer(a, mid + 1, right);

		return Math.max(onMid, Math.max(onLeft, onRight));
	}

	public void test() {
		int a[] = {-1, -2, -3, 1, -13};
		System.out.println(maxSumOfSubarray(a));
		System.out.println(maxSumOfSubarray_divideconquer(a, 0, a.length - 1));
		System.out.println(maxSumOfSubarray_dp(a));
	}
}
