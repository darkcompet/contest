/// Ref: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/operator-overloading
public struct DkModInt {
	private static int MOD;
	public int value;

	public DkModInt(int mod = 998244353) {
		MOD = mod;
	}

	public DkModInt(int value, int mod = 998244353) {
		this.value = value;
		MOD = mod;
	}

	public static DkModInt operator +(DkModInt a, DkModInt b) {
		return new DkModInt((a.value + b.value) % MOD);
	}

	public static DkModInt operator -(DkModInt a, DkModInt b) {
		return new DkModInt((a.value - b.value) % MOD);
	}

	public static DkModInt operator *(DkModInt a, DkModInt b) {
		return new DkModInt((a.value * b.value) % MOD);
	}

	public static DkModInt operator /(DkModInt a, DkModInt b) {
		return new DkModInt((a.value / b.value) % MOD);
	}
}

public struct DkModLong {
	private static long MOD;
	public long value;

	public DkModLong(long mod = 998244353) {
		MOD = mod;
	}

	public DkModLong(long value, long mod = 998244353) {
		this.value = value;
		MOD = mod;
	}

	public static DkModLong operator +(DkModLong a, DkModLong b) {
		return new DkModLong((a.value + b.value) % MOD);
	}

	public static DkModLong operator -(DkModLong a, DkModLong b) {
		return new DkModLong((a.value - b.value) % MOD);
	}

	public static DkModLong operator *(DkModLong a, DkModLong b) {
		return new DkModLong((a.value * b.value) % MOD);
	}

	public static DkModLong operator /(DkModLong a, DkModLong b) {
		return new DkModLong((a.value / b.value) % MOD);
	}
}
