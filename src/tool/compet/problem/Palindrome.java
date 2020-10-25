package tool.compet.problem;

public class Palindrome {
	public int longestPalindromicSubstring(char[] a, int left, int right) {
		if (left > right) return 0;
		if (left == right) return 1;
		if (right - left == 1) return a[left] == a[right] ? 2 : -1;
		int x = longestPalindromicSubstring(a, left + 1, right);
		int y = longestPalindromicSubstring(a, left, right - 1);
		int z = longestPalindromicSubstring(a, left + 1, right - 1);
		if (z >= 0 && a[left] == a[right]) z += 2;
		return Math.max(x, Math.max(y, z));
	}

	public String test() {
		String s = "abab";
		return longestPalindromicSubstring(s.toCharArray(), 0, s.length() - 1) + "";
	}
}
