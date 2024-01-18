#pragma warning disable IDE1006 // 命名スタイル
#pragma warning disable IDE0078 // パターン マッチングを使用します
#pragma warning disable IDE0058 // 式の値が使用されていません

/// <summary>
/// This performs read/write on ASCII chars which be in range [0, 255] (see: https://www.asciitable.com/).
/// TechNotes:
/// - Use hyphen (_) to separate/group long number (but it does not work in mono).
/// - Use new keyword to override base method that does not declare with virtual keyword.
/// </summary>
public abstract class BaseSolution {
	protected readonly bool isDebug;
	protected bool inputFromFile;
	protected bool outputToFile;

	/// <summary>
	/// Subclass can override this method to give the solution.
	/// </summary>
	protected virtual void Solve() { }

	/// White space chars: space, tab, linefeed
	private const int WHITE_SPACE_CODE = 32;
	private const int IN_BUFFER_SIZE = 1 << 13;
	private const int OUT_BUFFER_SIZE = 1 << 13;

	private Stream inStream;
	private Stream outStream;

	private byte[] inBuffer;
	private int nextReadByteIndex;
	private int readByteCount;

	private byte[] outChars;
	private int nextWriteByteIndex;

	/// To store bytes of int, long values when write to out-buffer
	private readonly byte[] scratchBytes = new byte[32];

	public BaseSolution() {
		// Before dotnet 6, we should use Path.GetFileName() instead.
		// From dotnet 7, we can use Path.Exists() to test the file.
		this.isDebug = System.IO.Path.GetFileName("local.proof") != null;
	}

	private long startTimeMillis;

	protected void Start() {
		if (this.isDebug) {
			this.startTimeMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		}

		// Init IO
		this.inStream = this.inputFromFile ?
			new FileStream(Path.GetFullPath("Data/in.txt"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite) :
			Console.OpenStandardInput();

		this.outStream = this.outputToFile ?
			new FileStream(Path.GetFullPath("Data/out.txt"), FileMode.Open, FileAccess.Write, FileShare.ReadWrite) :
			Console.OpenStandardOutput();

		this.inBuffer = new byte[IN_BUFFER_SIZE];
		this.outChars = new byte[OUT_BUFFER_SIZE];

		this.Solve();

		// Flush out buffer
		this.FlushOutBuffer();

		// Close IO stream
		this.inStream.Close();
		this.outStream.Close();

		if (this.isDebug) {
			Console.WriteLine($"Elapsed: {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - this.startTimeMillis} ms");
		}
	}

	public int ni() {
		var num = 0;

		var nextChar = this._ReadNextByteSkipWhitespace();

		// Check negative value
		var isNegative = nextChar == '-';
		if (isNegative) {
			_ = this._TryReadNextByte(out nextChar);
		}

		// Assert digit
		if (nextChar < '0' || nextChar > '9') {
			throw new Exception("Digit expected");
		}

		while (true) {
			num = (num << 1) + (num << 3) + nextChar - '0';
			if (!this._TryReadNextByte(out nextChar)) {
				break;
			}
			// Unread if we have read non-digit char
			if (nextChar < '0' || nextChar > '9') {
				this._UnreadNextByte();
				break;
			}
		}

		return isNegative ? -num : num;
	}

	public long nl() {
		var num = 0L;

		var nextChar = this._ReadNextByteSkipWhitespace();

		// Check negative value
		var isNegative = nextChar == '-';
		if (isNegative) {
			_ = this._TryReadNextByte(out nextChar);
		}

		// Assert digit
		if (nextChar < '0' || nextChar > '9') {
			throw new Exception("Digit expected");
		}

		while (true) {
			num = (num << 1) + (num << 3) + nextChar - '0';
			if (!this._TryReadNextByte(out nextChar)) {
				break;
			}
			// Unread if we have read non-digit char
			if (nextChar < '0' || nextChar > '9') {
				this._UnreadNextByte();
				break;
			}
		}

		return isNegative ? -num : num;
	}

	public float nf() {
		var pre = 0.0f;
		var suf = 0.0f;

		var nextChar = this._ReadNextByteSkipWhitespace();

		// Check negative value
		var isNegative = nextChar == '-';
		if (isNegative) {
			_ = this._TryReadNextByte(out nextChar);
		}

		// Assert digit
		if (nextChar < '0' || nextChar > '9') {
			throw new Exception("Digit expected");
		}

		var endOfStream = false;
		while (true) {
			pre = (10 * pre) + (nextChar - '0');
			if (!this._TryReadNextByte(out nextChar)) {
				endOfStream = true;
				break;
			}
			if (nextChar < '0' || nextChar > '9') {
				break;
			}
		}

		if (nextChar == '.') {
			var div = 1.0f;
			while (this._TryReadNextByte(out nextChar)) {
				if (nextChar < '0' || nextChar > '9') {
					break;
				}
				suf += (nextChar - '0') / (div *= 10);
			}
		}
		// Unread if we have read some `non-digit` char, and not `dot` char.
		else if (!endOfStream) {
			this._UnreadNextByte();
		}

		return isNegative ? -(pre + suf) : (pre + suf);
	}

	public double nd() {
		var pre = 0.0;
		var suf = 0.0;

		var nextChar = this._ReadNextByteSkipWhitespace();

		// Check negative value
		var isNegative = nextChar == '-';
		if (isNegative) {
			_ = this._TryReadNextByte(out nextChar);
		}

		// Assert digit
		if (nextChar < '0' || nextChar > '9') {
			throw new Exception("Digit expected");
		}

		var endOfStream = false;
		while (true) {
			pre = (10 * pre) + (nextChar - '0');
			if (!this._TryReadNextByte(out nextChar)) {
				endOfStream = true;
				break;
			}
			if (nextChar < '0' || nextChar > '9') {
				break;
			}
		}

		if (nextChar == '.') {
			var div = 1.0;
			while (this._TryReadNextByte(out nextChar)) {
				if (nextChar < '0' || nextChar > '9') {
					break;
				}
				suf += (nextChar - '0') / (div *= 10);
			}
		}
		// Unread if we have read some `non-digit` char, and not `dot` char.
		else if (!endOfStream) {
			this._UnreadNextByte();
		}

		return isNegative ? -(pre + suf) : (pre + suf);
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public char nc() {
		return (char)this._ReadNextByteSkipWhitespace();
	}

	public string ns() {
		var nextByte = this._ReadNextByteSkipWhitespace();

		var sb = new System.Text.StringBuilder();
		while (true) {
			_ = sb.Append((char)nextByte);

			if (!this._TryReadNextByte(out nextByte) || nextByte <= WHITE_SPACE_CODE) {
				break;
			}
		}

		return sb.ToString();
	}

	public int[] ni(int count) {
		var res = new int[count];
		for (var index = 0; index < count; ++index) {
			res[index] = this.ni();
		}
		return res;
	}

	public long[] nl(int count) {
		var res = new long[count];
		for (var index = 0; index < count; ++index) {
			res[index] = this.nl();
		}
		return res;
	}

	public float[] nf(int count) {
		var res = new float[count];
		for (var index = 0; index < count; ++index) {
			res[index] = this.nf();
		}
		return res;
	}

	public double[] nd(int count) {
		var res = new double[count];
		for (var index = 0; index < count; ++index) {
			res[index] = this.nd();
		}
		return res;
	}

	public char[] nc(int count) {
		var res = new char[count];
		for (var index = 0; index < count; ++index) {
			res[index] = this.nc();
		}
		return res;
	}

	public string[] ns(int count) {
		var res = new string[count];
		for (var index = 0; index < count; ++index) {
			res[index] = this.ns();
		}
		return res;
	}

	public int[][] ni(int rowCount, int colCount) {
		var res = new int[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = this.ni(colCount);
		}
		return res;
	}

	public long[][] nl(int rowCount, int colCount) {
		var res = new long[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = this.nl(colCount);
		}
		return res;
	}

	public float[][] nf(int rowCount, int colCount) {
		var res = new float[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = this.nf(colCount);
		}
		return res;
	}

	public double[][] nd(int rowCount, int colCount) {
		var res = new double[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = this.nd(colCount);
		}
		return res;
	}

	public char[][] nc(int rowCount, int colCount) {
		var res = new char[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = this.nc(colCount);
		}
		return res;
	}

	public string[][] ns(int rowCount, int colCount) {
		var res = new string[rowCount][];
		for (var index = 0; index < rowCount; ++index) {
			res[index] = this.ns(colCount);
		}
		return res;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void print(char ch) {
		this.print((byte)ch);
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void print(byte ch) {
		if (this.nextWriteByteIndex >= OUT_BUFFER_SIZE) {
			this.FlushOutBuffer();
		}
		this.outChars[this.nextWriteByteIndex++] = ch;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void print(byte[] arr, int fromIndex, int count) {
		if (this.nextWriteByteIndex + count >= OUT_BUFFER_SIZE) {
			this.FlushOutBuffer();
		}
		Array.Copy(arr, fromIndex, this.outChars, this.nextWriteByteIndex, count);
		this.nextWriteByteIndex += count;
	}

	public void print(int num) {
		var buffer = this.scratchBytes;
		var curIndex = buffer.Length;
		var isNegative = num < 0;

		do {
			buffer[--curIndex] = (byte)((isNegative ? -(num % 10) : (num % 10)) + '0');
			num /= 10;
		}
		while (num != 0);

		// Write to buffer
		if (isNegative) {
			this.print((byte)'-');
		}
		this.print(buffer, curIndex, buffer.Length - curIndex);
	}

	public void print(long num) {
		var buffer = this.scratchBytes;
		var curIndex = buffer.Length;
		var isNegative = num < 0;

		do {
			buffer[--curIndex] = (byte)((isNegative ? -(num % 10) : (num % 10)) + '0');
			num /= 10;
		}
		while (num != 0);

		// Write to buffer
		if (isNegative) {
			this.print((byte)'-');
		}
		this.print(buffer, curIndex, buffer.Length - curIndex);
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void print(string text) {
		var arr = System.Text.Encoding.ASCII.GetBytes(text);
		this.print(arr, 0, arr.Length);
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void println() {
		this.print((byte)'\n');
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void println(int num) {
		this.print(num);
		this.print((byte)'\n');
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void println(long num) {
		this.print(num);
		this.print((byte)'\n');
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void println(string text) {
		this.print(text);
		this.print((byte)'\n');
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	protected void FlushOutBuffer() {
		this.outStream.Write(this.outChars, 0, this.nextWriteByteIndex);
		this.outStream.Flush();
		this.nextWriteByteIndex = 0;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	private bool _TryReadNextByte(out byte result) {
		if (this.nextReadByteIndex >= this.readByteCount) {
			this.readByteCount = this.inStream.Read(this.inBuffer, 0, IN_BUFFER_SIZE);
			this.nextReadByteIndex = 0;

			if (this.readByteCount <= 0) {
				result = 0;
				return false;
			}
		}
		result = this.inBuffer[this.nextReadByteIndex++];
		return true;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	private byte _ReadNextByteSkipWhitespace() {
		while (this._TryReadNextByte(out var nextByte)) {
			if (nextByte > WHITE_SPACE_CODE) {
				return nextByte;
			}
		}
		throw new Exception("Cannot read more");
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	private void _UnreadNextByte() {
		if (this.nextReadByteIndex-- <= 0) {
			throw new Exception("Cannot unread more");
		}
	}

	/// <summary>
	/// Utc epoch time in millis.
	/// </summary>
	/// <returns>Elapsed UTC-time from Epoch in milliseconds</returns>
	protected static long Now() {
		return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}

	protected void debug(string text) {
		if (this.isDebug) {
			Console.Write(text);
		}
	}
	protected void debugln(string text) {
		if (this.isDebug) {
			Console.WriteLine(text);
		}
	}

	protected void assert(bool condition, string? message = null) {
		System.Diagnostics.Debug.Assert(condition, message);
	}
}

/// Run: dotnet run
public class Solution : BaseSolution {
	// public static void Main(params string[] args) {
	// 	var sol = new Solution();
	// 	sol.inputFromFile = sol.isDebug;
	// 	sol.Start();
	// }

	// protected override void Solve() {
	// 	this.debugln("ans: " + string.Join(", ", this.LexicographicallySmallestArray(new int[] { 1,7,6,18,2,1 }, 3)));
	// }

	public int[] LexicographicallySmallestArray(int[] nums, int limit) {
		var N = nums.Length;

		var tr = new AATree<MyNode>();
		var nodes = new MyNode[N];
		for (var index = 0; index < N; ++index) {
			var node = nodes[index] = new MyNode() {
				num = nums[index],
				index = index
			};
			tr.Add(node);
		}
		// this.debugln($"tr: {tr}");

		for (var index = 0; index < N; ++index) {
			var node = nodes[index];
			var swapNode = this.FindSwapNode(tr, node, limit);

			if (swapNode is null) {
				tr.Remove(node);
				// this.debugln($"---> Do not swap for node {node.num}({node.index}), tr: {tr}");
				continue;
			}

			(nums[index], nums[swapNode.index]) = (nums[swapNode.index], nums[index]);

			(nodes[index], nodes[swapNode.index]) = (nodes[swapNode.index], nodes[index]);
			(nodes[swapNode.index].index, nodes[index].index) = (nodes[index].index, nodes[swapNode.index].index);

			tr.Remove(swapNode);

			// this.debugln($"---> Swapped node {node.num}({node.index}) vs {swapNode.num}({swapNode.index}), nums: {string.Join(", ", nums)}, tr: {tr}");
		}

		return nums;
	}

	private readonly MyNode lowerMyNode = new();
	private MyNode? FindSwapNode(AATree<MyNode> tr, MyNode node, int limit) {
		MyNode? bestNode = null;
		var curNode = node;
		var count = 0;
		while (true) {
			++count;
			// num - target <= limit
			// target >= num - limit
			this.lowerMyNode.num = curNode.num - limit;
			if (!tr.Lowerbound(this.lowerMyNode, out var resultNode) || resultNode!.num >= curNode.num) {
				this.debugln($"---> count: {count}");
				return bestNode;
			}
			// this.debugln($"2.{count} lowerbound of {curNode.num}-{limit} is {resultNode.num}({resultNode.index})");
			bestNode = resultNode;
			curNode = resultNode;
		}
	}
}


/// <summary>
/// Ref:
/// https://github.com/JuYanYan/AA-Tree/blob/master/aatree.cpp
/// https://users.cs.fiu.edu/~weiss/dsaa_c++/code/AATree.cpp
/// https://www.nayuki.io/res/aa-tree-set/BasicAaTreeSet.java
/// https://en.wikipedia.org/wiki/AA_tree
/// </summary>
/// <typeparam name="T"></typeparam>
public class AATree<T> where T : IComparable<T> {
	private AATreeNode? root;

	public AATree() {
	}

	public void Add(T value) {
		this.root = this.Insert(this.root, value);
	}

	private AATreeNode Insert(AATreeNode? node, T value) {
		if (node is null) {
			return new AATreeNode(value, null, null);
		}

		var compare = value.CompareTo(node.value);
		if (compare == 0) {
			// Add to bucket for value that equals to this node's value but different instance.
			if (!value.Equals(node.value)) {
				node.bucket ??= new();
				node.bucket.Add(value);
			}
			return node;
		}

		if (compare < 0) {
			node.left = this.Insert(node.left, value);
		}
		else {
			node.right = this.Insert(node.right, value);
		}

		node = this.Skew(node);
		node = this.Split(node);

		return node;
	}

	public bool Remove(T value) {
		this.root = this.Delete(this.root, value);
		return this.root != null;
	}

	/// <summary>
	/// https://en.wikipedia.org/wiki/AA_tree
	/// </summary>
	/// <param name="node"></param>
	/// <param name="value"></param>
	/// <param name="forceDeleteLeaf"></param>
	/// <returns></returns>
	private AATreeNode? Delete(AATreeNode? node, T value, bool forceDeleteLeaf = false) {
		if (node is null) {
			return null;
		}

		var compare = value.CompareTo(node.value);
		if (compare < 0) {
			// Delete left node (we will rebalancing tree later)
			node.left = this.Delete(node.left, value, forceDeleteLeaf);
		}
		else if (compare > 0) {
			// Delete right node (we will rebalancing tree later)
			node.right = this.Delete(node.right, value, forceDeleteLeaf);
		}
		else {
			if (!forceDeleteLeaf) {
				// Try remove from bucket first.
				if (node.bucket != null && node.bucket.Remove(value)) {
					return node;
				}
				// Try remove value from node itself.
				if (!node.value.Equals(value)) {
					return node;
				}

				// Remove node value. Then bring one of bucket value to node's value.
				if (node.bucket?.Count > 0) {
					node.value = node.bucket.First();
					node.bucket.Remove(node.value);
					return node;
				}
			}

			// Remove leaf node.
			if (node.left is null && node.right is null) {
				return null;
			}

			// -> Remove successor node
			if (node.left is null) {
				// Successor is leaf node. Find it by go one right, then turn left until meet leaf node.
				var successorNode = this.Successor(node);
				// Console.WriteLine($"-------> 011. successor of node {node.value} is {successorNode.value}, tr: {this}");
				// Remove the node by overwrite successor data to it
				node.value = successorNode.value;
				node.bucket = successorNode.bucket;
				node.right = this.Delete(node.right, successorNode.value, true);
				// Console.WriteLine($"-------> 012. node: {node.value}, tr: {this}");
			}
			// -> Remove predecessor node
			else {
				// Predecessor is leaf node. Find it by go one left, then turn right until meet leaf node.
				var predecessorNode = this.Predecessor(node);
				// Console.WriteLine($"-------> 021. predecessor of node {node.value} is {predecessorNode.value}, tr: {this}");
				// Remove the node by overwrite predecessor data to it
				node.value = predecessorNode.value;
				node.bucket = predecessorNode.bucket;
				node.left = this.Delete(node.left, predecessorNode.value, true);
				// Console.WriteLine($"-------> 022. node: {node.value}, tr: {this}");
			}
		}

		// Console.WriteLine($"-------> 1. tr: {this}");
		node = this.DecreaseLevel(node);
		// Console.WriteLine($"-------> 2. tr: {this}");
		node = this.Skew(node);
		// Console.WriteLine($"-------> 3. tr: {this}");

		if (node.right != null) {
			node.right = this.Skew(node.right);
			// Console.WriteLine($"-------> 4. tr: {this}");

			if (node.right.right != null) {
				node.right.right = this.Skew(node.right.right);
				// Console.WriteLine($"-------> 5. tr: {this}");
			}
		}

		node = this.Split(node);
		// Console.WriteLine($"-------> 6. tr: {this}");
		if (node.right != null) {
			node.right = this.Split(node.right);
			// Console.WriteLine($"-------> 7. tr: {this}");
		}

		return node;
	}

	private AATreeNode DecreaseLevel(AATreeNode node) {
		if (node.left != null && node.right != null) {
			var shouldBeLevel = Math.Min(node.left.level, node.right.level) + 1;
			if (node.level > shouldBeLevel) {
				node.level = shouldBeLevel;
				if (node.right != null && node.right.level > shouldBeLevel) {
					node.right.level = shouldBeLevel;
				}
			}
		}
		return node;
	}

	private AATreeNode Predecessor(AATreeNode curNode) {
		curNode = curNode.left!;
		while (curNode.right != null) {
			curNode = curNode.right;
		}
		return curNode;
	}

	private AATreeNode Successor(AATreeNode curNode) {
		curNode = curNode.right!;
		while (curNode.left != null) {
			curNode = curNode.left;
		}
		return curNode;
	}

	private AATreeNode Skew(AATreeNode node) {
		if (node.left != null) {
			// Rotate right
			if (node.level == node.left.level) {
				var left = node.left;
				node.left = left.right;
				left.right = node;

				return left;
			}
		}
		return node;
	}

	private AATreeNode Split(AATreeNode node) {
		if (node.right?.right != null) {
			// Rotate left
			if (node.right.right.level == node.level) {
				var right = node.right;
				node.right = right.left;
				right.left = node;
				++right.level;

				return right;
			}
		}
		return node;
	}

	private AATreeNode? Search(AATreeNode? node, T value) {
		if (node is null) {
			return null;
		}
		var compare = value.CompareTo(node.value);
		if (compare < 0) {
			return this.Search(node.left, value);
		}
		if (compare > 0) {
			return this.Search(node.right, value);
		}

		return (node.value.Equals(value) || (node.bucket != null && node.bucket.Contains(value))) ? node : null;
	}

	public T FindMin() {
		if (this.root is null) {
			throw new Exception("Empty tree");
		}
		//TODO impl
		return default;
	}

	public T FindMax() {
		if (this.root is null) {
			throw new Exception("Empty tree");
		}
		//TODO impl
		return default;
	}

	public bool IsEmpty() {
		return this.root is null;
	}

	public bool Lowerbound(T value, out T? result) {
		return Lowerbound(this.root, value, out result);
	}

	private static bool Lowerbound(AATreeNode? node, T value, out T? result) {
		if (node is null) {
			result = default;
			return false;
		}

		var compare = value.CompareTo(node.value);

		// Node value > the value
		// -> Find at left subtree to get smaller value
		if (compare < 0) {
			if (Lowerbound(node.left, value, out result)) {
				return true;
			}
			result = node.value;
			return true;
		}

		// Node value < the value
		// -> Find at right subtree to get bigger value
		if (compare > 0) {
			if (Lowerbound(node.right, value, out result)) {
				return true;
			}
			result = default;
			return false;
		}

		result = node.value;
		return true;
	}

	private string NodeToString(AATreeNode? node) {
		if (node is null) {
			return string.Empty;
		}

		var sb = new System.Text.StringBuilder();

		if (node != node.left) {
			var leftExp = this.NodeToString(node.left);
			if (leftExp.Length > 0) {
				sb.Append('{').Append(leftExp).Append('}').Append(" <- ");
			}
		}

		sb.Append(node.value).Append(node == this.root ? "(R)" : string.Empty);
		if (node.bucket?.Count > 0) {
			sb.Append('*').Append(node.bucket.Count);
		}

		if (node != node.right) {
			var rightExp = this.NodeToString(node.right);
			if (rightExp.Length > 0) {
				sb.Append(" -> ").Append('{').Append(rightExp).Append('}');
			}
		}

		return sb.ToString();
	}

	public override string ToString() {
		return this.NodeToString(this.root);
	}

	internal class AATreeNode {
		// User data
		public T value;

		// Node internal data
		internal int level;
		internal AATreeNode? left;
		internal AATreeNode? right;

		internal HashSet<T>? bucket;

		// constuctor for regular nodes (that all start life as leaf nodes)
		internal AATreeNode(T value, AATreeNode? left, AATreeNode? right) {
			this.left = left;
			this.right = right;
			this.value = value;
			this.level = 1;
		}
	}
}

public class MyNode : IComparable<MyNode> {
	public int num;
	public int index;

	public int CompareTo(MyNode that) {
		return this.num - that.num;
	}

	public override string ToString() {
		return this.num + string.Empty;
	}
}
