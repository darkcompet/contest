package tool.compet.algorithm;

import java.util.ArrayList;
import java.util.List;

public class DkPrimes {
	// Sieve prime number from [1, n]. Eratosthenes algorithm: O(N.loglog(N))
	public boolean[] sievePrimes(int n) {
		final int N = n + 1;
		boolean not[] = new boolean[N];
		not[0] = not[1] = true;
		for (int p = 2; p < N; ++p) {
			if (not[p]) continue;
			for (int v = p * p; v < N; v += p) {
				not[v] = true;
			}
		}
		return not;
	}

	// Sieve prime number from [1, n]. Modified Eratosthenes algorithm: O(N)
	public boolean[] sievePrimesFast(int n) {
		final int N = n + 1;
		boolean not[] = new boolean[N];
		int smallest_prime_factor[] = new int[N];
		List<Integer> primes = new ArrayList<>(N);
		not[0] = not[1] = true;
		for (int A = 2; A < N; ++A) {
			if (!not[A]) {
				primes.add(A);
				smallest_prime_factor[A] = A;
			}
			// for Main < B, assume B = p.Main with p = smallest_prime_factor[B]
			// of course p <= smallest_prime_factor[Main]
			// when we meet Main, we should add B as not prime number
			// by set not[Main.x] = true with x is prime number in [2, smallest_prime_factor[Main]]
			for (int x : primes) {
				if (x > smallest_prime_factor[A]) break;
				int B = A * x;
				if (B >= N) break;
				not[B] = true;
				smallest_prime_factor[B] = x;
			}
		}
		return not;
	}

	// O(N.log(N))
	public List<Integer> sievePrimes(int from, int to) {
		List<Integer> res = new ArrayList<>();
		if (from <= 2) from = 2;
		if (from > to) return res;

		// pre-processing
		int MAX = DkMaths.sqrt(Integer.MAX_VALUE) + 1;
		boolean not[] = new boolean[MAX];
		for (int i = 2; i < MAX; ++i) for (int j = i << 1; j < MAX; j += i) not[j] = true;
		List<Integer> primes = new ArrayList<>();
		for (int i = 2; i < MAX; ++i) if (!not[i]) primes.add(i);

		// query for range
		boolean no[] = new boolean[to - from + 1];
		for (int p : primes) {
			for (int i = from / p; ; ++i) {
				int cur = i * p;
				if (cur <= p || cur < from) continue;
				if (cur > to) break;
				no[cur - from] = true;
			}
		}
		for (int i = 0; i < no.length; ++i) if (!no[i]) res.add(i + from);

		return res;
	}

	// Wilson theorem: p is a prime number iff (p-1)! ≡ (p-1) (modulo p)
}
