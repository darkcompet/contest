package tool.compet.datastructure;

// Binary Indexed Tree
public class FenwickTree {
	private int[] T;

	public void build(int[] a) {
		int N = a.length;
		T = new int[N + 1];
		for (int i = 0; i < N; ++i) {
			add(i, a[i]);
		}
	}

	public void add(int index, int val) {
		int N = T.length;
		// add val to T[i] and its ancestors
		for (++index; index < N; index += (index & -index)) {
			T[index] += val;
		}
	}

	public long query(int index) {
		long sum = 0;
		++index;
		for (int lastBit; index > 0; lastBit = index & -index, index -= lastBit) {
			sum += T[index];
		}
		return sum;
	}

	public void test() {
		int a[] = {1, 3, 2, 6, 1, 4};
		build(a);
		System.out.println(query(0));
		System.out.println(query(1));
		System.out.println(query(2));
	}
}
