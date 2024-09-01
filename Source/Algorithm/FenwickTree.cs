namespace Compet.Algorithm;

/// <summary>
/// Fenwick BIT (Binary Indexed Tree) implementation.
/// Problems: https://leetcode.com/problem-list/5vezxjhm
/// BIT: https://www.topcoder.com/thrive/articles/Binary%20Indexed%20Trees
/// </summary>
public class FenwickTree(int N) {
	/// <summary>
	/// Initially all the values of Fenwick tree are 0
	/// </summary>
	private readonly List<int> list = new(new int[N + 1]);

	/// <summary>
	/// Add `more` value to element at given `index`
	/// </summary>
	/// <param name="index"></param>
	/// <param name="more"></param>
	public void Add(int index, int more) {
		while (index < this.list.Count) {
			this.list[index] += more;
			// Add rightMostSetBit to index
			index += index & (-index);
		}
	}

	/// <summary>
	/// Calculate current value in array at given `index`
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public int ValueAt(int index) {
		return this.SumTo(index) - this.SumTo(index - 1);
	}

	/// <summary>
	/// Calculate range sum from index `left` to `right`.
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <returns></returns>
	public int SumOn(int left, int right) {
		return this.SumTo(right) - this.SumTo(left - 1);
	}

	/// <summary>
	/// Method to calculate prefix sum till to `index`
	/// </summary>
	/// <param name="index">End index inclusive</param>
	/// <returns></returns>
	public int SumTo(int index) {
		var sum = 0;
		// Summing up all the partial sums
		while (index > 0) {
			sum += this.list[index];
			// Subtract rightMostSetBit from index
			index -= index & (-index);
		}
		return sum;
	}
}
