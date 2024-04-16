namespace Compet.Algorithm;

public class Prime {
	/// <summary>
	/// Generate all prime numbers that less than or equal to given N.
	/// </summary>
	/// <param name="N">Maximum number that each prime number should not greater than.</param>
	/// <returns></returns>
	public static List<int> SieveOfEratosthenes(int N) {
		var notPrime = new bool[N + 1];
		for (var p = 2; p * p <= N; p++) {
			if (!notPrime[p]) {
				// Update all multiples of p
				for (var mp = p * p; mp <= N; mp += p) {
					notPrime[mp] = true;
				}
			}
		}

		// Get all prime numbers
		var primes = new List<int>(N >> 1);
		for (var p = 2; p <= N; p++) {
			if (!notPrime[p]) {
				primes.Add(p);
			}
		}

		return primes;
	}
}
