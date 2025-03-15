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

/// <summary>
/// Without main method.
/// </summary>
public class Solution : BaseSolution {
	public int[] ResultsArray(int[][] queries, int k) {
		var maxHeap = new PriorityQueue<int, int>();
		var dists = new int[queries.Length];
		for (var i = 0; i < queries.Length; ++i) {
			var x = queries[i][0];
			var y = queries[i][1];
			var d = Math.Abs(x) + Math.Abs(y);
			maxHeap.Enqueue(d, -d);
			var removeCount = maxHeap.Count - k;
			while (removeCount-- > 0) {
				maxHeap.Dequeue();
			}

			if (i + 1 < k) {
				dists[i] = -1;
			}
			else {
				dists[i] = maxHeap.Peek();
			}
		}
		return dists;
	}
}
