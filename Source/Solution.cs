public class HashMap<TKey, TValue> : Dictionary<TKey, TValue?> where TKey : notnull {
	public new TValue? this[TKey key] {
		get => this.GetValueOrDefault(key, default);
		set => base[key] = value;
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

	protected virtual void debug(string text) {
		Console.Write(text);
	}

	protected virtual void debugln(string text) {
		Console.WriteLine(text);
	}

	protected void assert(bool condition, string? message = null) {
		System.Diagnostics.Debug.Assert(condition, message);
	}

	protected int min(params int[] arr) {
		var ans = arr[0];
		for (var i = arr.Length - 1; i > 0; --i) {
			if (ans < arr[i]) {
				ans = arr[i];
			}
		}
		return ans;
	}

	protected int max(params int[] arr) {
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
