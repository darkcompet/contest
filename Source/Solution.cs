public class DkHashMap<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull {
	/// <summary>
	/// Null/Default value means the key may not exist.
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public new TValue? this[TKey key] {
		get => this.GetValueOrDefault(key, default);
		set => base[key] = value!;
	}

	public TValue GetOrSet(TKey key, TValue initValue) {
		if (this.TryGetValue(key, out var value)) {
			return value;
		}
		base[key] = initValue;
		return initValue;
	}
}

public class BaseSolution {
	protected bool isDebug;

	/// <summary>
	/// Utc epoch time in millis.
	/// </summary>
	/// <returns>Elapsed UTC-time from Epoch in milliseconds</returns>
	protected static long Now() {
		return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}

	protected virtual void Debug(string text) {
		Console.Write(text);
	}

	protected virtual void Debugln(string text) {
		Console.WriteLine(text);
	}

	protected void Assert(bool condition, string? message = null) {
		System.Diagnostics.Debug.Assert(condition, message);
	}

	protected int Min(params int[] arr) {
		var ans = arr[0];
		for (var i = arr.Length - 1; i > 0; --i) {
			if (ans < arr[i]) {
				ans = arr[i];
			}
		}
		return ans;
	}

	protected long Min(params long[] arr) {
		var ans = arr[0];
		for (var i = arr.Length - 1; i > 0; --i) {
			if (ans < arr[i]) {
				ans = arr[i];
			}
		}
		return ans;
	}

	protected int Max(params int[] arr) {
		var ans = arr[0];
		for (var i = arr.Length - 1; i > 0; --i) {
			if (ans > arr[i]) {
				ans = arr[i];
			}
		}
		return ans;
	}

	protected long Max(params long[] arr) {
		var ans = arr[0];
		for (var i = arr.Length - 1; i > 0; --i) {
			if (ans > arr[i]) {
				ans = arr[i];
			}
		}
		return ans;
	}
}

public class Solution : BaseSolution {
}
