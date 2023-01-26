/// Ref: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/operator-overloading

public struct ModInt {
	public const int MOD = 998244353;
	public int value;

	public ModInt(int value) {
		this.value = value;
	}

	public static ModInt operator +(ModInt a, ModInt b) {
		return new ModInt((a.value + b.value) % MOD);
	}

	public static ModInt operator -(ModInt a, ModInt b) {
		return new ModInt((a.value - b.value) % MOD);
	}

	public static ModInt operator *(ModInt a, ModInt b) {
		return new ModInt((a.value * b.value) % MOD);
	}

	public static ModInt operator /(ModInt a, ModInt b) {
		return new ModInt((a.value / b.value) % MOD);
	}
}

public struct ModLong {
	public const long MOD = 998244353;
	public long value;

	public ModLong(long value) {
		this.value = value;
	}

	public static ModLong operator +(ModLong a, ModLong b) {
		return new ModLong((a.value + b.value) % MOD);
	}

	public static ModLong operator -(ModLong a, ModLong b) {
		return new ModLong((a.value - b.value) % MOD);
	}

	public static ModLong operator *(ModLong a, ModLong b) {
		return new ModLong((a.value * b.value) % MOD);
	}

	public static ModLong operator /(ModLong a, ModLong b) {
		return new ModLong((a.value / b.value) % MOD);
	}
}
