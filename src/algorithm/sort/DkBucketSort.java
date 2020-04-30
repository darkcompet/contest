package tool.compet.algorithm.sort;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class DkBucketSort {
	public void bucketsort(float[] a, int left, int right) {
		int n = a.length;
		List<Float> bucket[] = new List[n];
		for (int i = left; i <= right; ++i) {
			int id = (int) (n * a[i]);
			if (bucket[id] == null) bucket[id] = new ArrayList<>();
			bucket[id].add(a[i]);
		}
		int cur = 0;
		for (List<Float> list : bucket) {
			if (list == null) continue;
			Collections.sort(list);
			for (Float f : list) {
				a[cur++] = f;
			}
		}
	}
}
