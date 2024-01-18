namespace Compet.Algorithm;

/// <summary>
/// Knuth–Morris–Pratt algorithm to find all pattern (M) in given text (N).
/// Time: O(N + M).
/// Memory: O(M).
/// Ref: https://en.wikipedia.org/wiki/Knuth%E2%80%93Morris%E2%80%93Pratt_algorithm
/// Impl: https://www.geeksforgeeks.org/kmp-algorithm-for-pattern-searching/
/// </summary>
public class KMP {
	/// <summary>
	/// </summary>
	/// <param name="text"></param>
	/// <param name="pattern"></param>
	/// <returns>Indices in the text that the pattern appears.</returns>
	public IList<int> Search(string text, string pattern) {
		var indices = new List<int>();
		var M = pattern.Length;
		var N = text.Length;

		// Create lps[] that will hold the longest prefix suffix values for pattern
		// Preprocess the pattern (calculate lps[] array)
		var lps = new int[M];
		this.ComputeLPS(pattern, lps);

		// Index for text and pattern.
		var index = 0;
		var patIndex = 0;
		while (index < N) {
			if (pattern[patIndex] == text[index]) {
				++index;
				++patIndex;
			}

			if (patIndex == M) {
				indices.Add(index - patIndex);
				patIndex = lps[patIndex - 1];
			}
			// Mismatch after j matches
			else if (index < N && pattern[patIndex] != text[index]) {
				// Do not match lps[0..lps[j-1]] characters, they will match anyway
				if (patIndex != 0) {
					patIndex = lps[patIndex - 1];
				}
				else {
					++index;
				}
			}
		}
		return indices;
	}

	private void ComputeLPS(string pattern, int[] lps) {
		var M = pattern.Length;
		// Length of the previous longest prefix suffix
		var len = 0;
		var index = 1;

		lps[0] = 0;

		// The loop calculates lps[i] for i = 1 to M-1
		while (index < M) {
			if (pattern[index] == pattern[len]) {
				len++;
				lps[index] = len;
				index++;
			}
			else {
				// This is tricky. Consider the example.
				// AAACAAAA and i = 7. The idea is similar to search step.
				if (len != 0) {
					len = lps[len - 1];
				}
				else {
					lps[index] = len;
					index++;
				}
			}
		}
	}
}
