#pragma warning disable IDE1006 // 命名スタイル
#pragma warning disable IDE0078 // パターン マッチングを使用します
#pragma warning disable IDE0058 // 式の値が使用されていません

/// <summary>
/// Ref:
/// https://users.cs.fiu.edu/~weiss/dsaa_c++/code/AATree.cpp
/// https://github.com/JuYanYan/AA-Tree/blob/master/aatree.cpp
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
			++node.count;
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

	private AATreeNode? Delete(AATreeNode? node, T value) {
		if (node is null) {
			return null;
		}

		var compare = value.CompareTo(node.value);
		if (compare < 0) {
			node.left = this.Delete(node.left, value);
		}
		else if (compare > 0) {
			node.right = this.Delete(node.right, value);
		}
		else {
			--node.count;
			if (node.left is null && node.right is null) {
				return node.count == 0 ? null : node;
			}

			if (node.left is null) {
				var suc = Successor(node);
				node.value = suc.value; // Exchange position.
				node.right = this.Delete(node.right, suc.value); // Delete successor node.
			}
			else {
				var pre = Predecessor(node);
				node.value = pre.value; // Similar to the above.
				node.left = this.Delete(node.left, pre.value);
			}
		}

		node = DecreaseLevel(node);
		node = this.Skew(node);
		var right = node.right;

		if (right != null) {
			node.right = this.Skew(right);

			if (right.right != null) {
				node.right.right = this.Skew(right.right);
			}
		}

		node = this.Split(node);
		if (right != null) {
			node.right = this.Split(right);
		}

		return node;
	}

	static AATreeNode DecreaseLevel(AATreeNode node) {
		int wdo;
		if (node.left != null && node.right != null) {
			wdo = Math.Min(node.left.level, node.right.level) + 1;
			if (wdo < node.level) {
				node.level = wdo;
				if (node.right != null && wdo < node.right.level) {
					node.right.level = wdo;
				}
			}
		}
		return node;
	}

	static AATreeNode Predecessor(AATreeNode curNode) {
		curNode = curNode.left!;
		while (curNode.right != null) {
			curNode = curNode.right;
		}
		return curNode;
	}

	static AATreeNode Successor(AATreeNode curNode) {
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
		return node;
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

	public T? Lowerbound(T value) {
		return this.Lowerbound(this.root, value);
	}

	private T? Lowerbound(AATreeNode? node, T value) {
		if (node is null) {
			throw new Exception("Tree empty");
		}
		var compare = value.CompareTo(node.value);

		// Node value > the value
		// -> Find at left subtree to get smaller value
		if (compare < 0) {
			var result = this.Lowerbound(node.left, value);
			if (result is null) {
				return node.value;
			}
			return result;
		}

		// Node value < the value
		// -> Find at right subtree to get bigger value
		if (compare > 0) {
			return this.Lowerbound(node.right, value);
		}

		//TODO find left node more
		return node.value;
	}

	private static string NodeToString(AATreeNode? node) {
		if (node != null) {
			var sb = new System.Text.StringBuilder();
			if (node != node.left) {
				if (sb.Length > 0) { sb.Append(' '); }
				sb.Append(NodeToString(node.left));
			}
			if (sb.Length > 0) { sb.Append(' '); }
			sb.Append(node.value);
			if (node != node.right) {
				if (sb.Length > 0) { sb.Append(' '); }
				sb.Append(NodeToString(node.right));
			}
			return sb.ToString();
		}
		return string.Empty;
	}

	public override string ToString() {
		return NodeToString(this.root);
	}

	internal class AATreeNode {
		// User data
		public T value;

		// Node internal data
		internal int level;
		internal AATreeNode? left;
		internal AATreeNode? right;
		/// <summary>
		/// Hold number of value that equals.
		/// </summary>
		internal int count;

		// constuctor for regular nodes (that all start life as leaf nodes)
		internal AATreeNode(T value, AATreeNode? left, AATreeNode? right) {
			this.left = left;
			this.right = right;
			this.value = value;
			this.level = 1;
			this.count = 1;
		}
	}
}

/// <summary>
/// This performs read/write on ASCII chars which be in range [0, 255] (see: https://www.asciitable.com/).
/// TechNotes:
/// - Use hyphen (_) to separate/group long number (but it does not work in mono).
/// - Use new keyword to override base method that does not declare with virtual keyword.
/// </summary>
public abstract class BaseSolution {
	protected virtual bool inputFromFile { get; set; }
	protected virtual bool outputToFile { get; set; }

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
	}

	protected void Start() {
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
		Console.Write(text);
	}
	protected void debugln(string text) {
		Console.WriteLine(text);
	}

	protected void assert(bool condition, string? message = null) {
		System.Diagnostics.Debug.Assert(condition, message);
	}
}

/// Run: dotnet run
public class Solution : BaseSolution {
	// protected override bool inputFromFile => true;
	// protected override bool outputToFile => true;

	public static void Main(params string[] args) {
		new Solution().Start();
	}

	protected override void Solve() {
		debugln("ans: " + string.Join(", ", LexicographicallySmallestArray(new int[] { 7, 73, 1, 97, 13, 55, 74, 29, 76, 19 }, 14)));
	}


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

		for (var index = 0; index < N; ++index) {
			var node = nodes[index];
			var swapNode = FindSwapNode(tr, node, limit);

			if (swapNode is null) {
				tr.Remove(node);
				debugln($"---> Do not swap for node {node.num}({node.index}), tr: {tr}");
				continue;
			}

			(nums[index], nums[swapNode.index]) = (nums[swapNode.index], nums[index]);
			(nodes[index], nodes[swapNode.index]) = (nodes[swapNode.index], nodes[index]);

			(nodes[swapNode.index].index, nodes[index].index) = (nodes[index].index, nodes[swapNode.index].index);

			tr.Remove(swapNode);

			debugln($"---> Swapped node {node.num}({node.index}) vs {swapNode.num}({swapNode.index}), nums: {string.Join(", ", nums)}, tr: {tr}");
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
			var resultNode = tr.Lowerbound(this.lowerMyNode);
			if (resultNode is null || resultNode.num >= curNode.num) {
				debugln($"1.{count} lowerbound of {curNode.num}-{limit} is {resultNode?.num}({resultNode?.index})");
				return bestNode;
			}
			debugln($"2.{count} lowerbound of {curNode.num}-{limit} is {resultNode.num}({resultNode.index})");
			bestNode = resultNode;
			curNode = resultNode;
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
