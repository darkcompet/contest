public class HashMap<TKey, TValue> : Dictionary<TKey, TValue?> where TKey : notnull {
	public new TValue? this[TKey key] {
		get => this.GetValueOrDefault(key, default);
		set => base[key] = value;
	}
}

public class Solution {
}
