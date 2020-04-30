# Best Mask

This problem minimizes height of `box` with a `mask`. In here, `box` contains elements, and `box height` is maximum value
of elements.

>Source:  
>D. Dr. Evil Underscores  
>https://codeforces.com/problemset/problem/1285/D


## Problem

**Input:**

- Positive integer `N (1 <= N <= 10^5)`.
- Array `arr` contains `N` positive elements, each item `arr[i] in [0, 2^30 - 1]`.

**Output:**

For all positive integer `X`, find minimum possible value of `max(arr[i] ^ X)` over all `i in [0, N - 1]`.


## Observation

Suppose each integer has at max 32 bits.

- Start from left to right for each `arr[i]`, we can see that if all bit at index `i` is `1`,
then we can mark all bits at `i` with bit `1` to result a smaller value `max(arr[i] ^ X)`.
So bit at `i` of `X` is `1` in this case.

- Else if all bit at `i` is `0`, we can pass this case, so bit at `i` of `X` is `0`.

- Otherwise, we have 2 options, `XOR` all bit at `i` with `0` or `1`. For both options,
we can divide bit at `i` after `XOR` to 2 groups `zero` (for bit `0`) and `one` (for bit `1`).
In each group, we can calculate max-value by recursively calculate max-value at index `i + 1`.
Finally, just calculate `minumum value` from each option to result `maximum value`.

This is [DP (Dynamic Programming)](https://en.wikipedia.org/wiki/Dynamic_programming) approach.
We succeed to divide a set to sub-sets to handle on each child sub-set and report result
to its parent.


## Implementation

Java implementation. See full source code at [Codeforces Submission](https://codeforces.com/contest/1285/submission/68962540).

```java
   void solve() throws Exception {
      int N = ni();
      int[] arr = ni(N);

      List<Integer> indices = new ArrayList<>();
      
      for (int i = 0; i < N; ++i) {
         indices.add(i);
      }
      
      println(maxValueFrom(31, indices, arr));
   }

   int maxValueFrom(int colIndex, List<Integer> indices, int[] arr) {
      if (colIndex < 0) {
         return 0;
      }

      List<Integer> zeroIndices = new ArrayList<>();
      List<Integer> oneIndices = new ArrayList<>();

      // Check bit at colIndex is 0 or 1 to divide 2 index-groups
      for (int index : indices) {
         if ((arr[index] & (1 << colIndex)) > 0) {
            oneIndices.add(index);
         }
         else {
            zeroIndices.add(index);
         }
      }

      if (zeroIndices.size() == 0 || oneIndices.size() == 0) {
         return maxValueFrom(colIndex - 1, indices, arr);
      }

      int childZeroResult = maxValueFrom(colIndex - 1, zeroIndices, arr);
      int childOneResult = maxValueFrom(colIndex - 1, oneIndices, arr);

      // Have 2 cases here, xor at colIndex with 0 or 1 to get min of max-results.
      return Math.min((1 << colIndex) + childOneResult), (1 << colIndex) + childZeroResult)););
   }
```


## Discussion

- At begining time I faced to this problem, I have taken a lot of logic-mistakes while thinking it.
I tried with `for` loop from `left-to-right` and `right-to-left` but it does not `guarantee` the solution
is correct since many cases is omitted.

- By doing with `dp (dynamic programming)` approach, I met some mistakes for get `min of max` for
each `XOR` case at `colIndex` for numbers inside combined-group.

- Humh, after ~10 times to try, finally it was done. Time-Complexity of the algorithm is
about `N x log(32)`.
