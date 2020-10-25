package tool.compet.algorithm.sort;

public class DkCountingSort {
	public void countingsort(char[] a, int left, int right) {
		int cnt[] = new int[1 << 16];
		for (int i = left; i <= right; ++i) cnt[a[i]]++;
		int cur = 0;
		for (int ch = 0, n = cnt.length; ch < n; ++ch) {
			while (cnt[ch]-- > 0) {
				a[cur++] = (char) ch;
			}
		}
	}
}
