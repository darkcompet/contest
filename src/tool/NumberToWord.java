package tool;

public class NumberToWord {
	private String a[] = {"Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine"};
	private String b[] = {"Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"};
	private String c[] = {"Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"};
	private String d[] = {"Hundred", "Thousand", "Million", "Billion", "Trillion"};

	public String convert(String number) {
		int i = 0;
		for (int CNT = number.length(); i < CNT; ++i) {if (number.charAt(i) != '0') break;}
		String s = number.substring(i, number.length());
		final int N = s.length();
		if (N > 12) {
			String s12 = convert(s.substring(N - 12, N));
			if (s12.length() > 0) s12 = " " + s12;
			return convert(s.substring(0, N - 12)) + " " + d[4] + s12;
		}
		if (N > 9) {
			String s9 = convert(s.substring(N - 9, N));
			if (s9.length() > 0) s9 = " " + s9;
			return convert(s.substring(0, N - 9)) + " " + d[3] + s9;
		}
		if (N > 6) {
			String s6 = convert(s.substring(N - 6, N));
			if (s6.length() > 0) s6 = " " + s6;
			return convert(s.substring(0, N - 6)) + " " + d[2] + s6;
		}
		if (N > 3) {
			String s3 = convert(s.substring(N - 3, N));
			if (s3.length() > 0) s3 = " " + s3;
			return convert(s.substring(0, N - 3)) + " " + d[1] + s3;
		}
		if (N == 3) {
			String s2 = convert(s.substring(1, 3));
			if (s2.length() > 0) s2 = " " + s2;
			return a[s.charAt(0) - '0'] + " " + d[0] + s2;
		}
		if (N == 2 && s.charAt(0) < '2') {
			return b[Integer.parseInt(s) - 10];
		}
		if (N == 2 && s.charAt(0) > '1') {
			String s1 = convert(s.substring(1, 2));
			if (s1.length() > 0) s1 = " " + s1;
			return c[s.charAt(0) - '0' - 2] + s1;
		}
		if (N == 1) {
			return a[Integer.parseInt(s)];
		}
		return "";
	}

	public String test() {
		return convert("90090");
	}
}
