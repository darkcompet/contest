#pragma warning disable IDE1006 // 命名スタイル
#pragma warning disable IDE0078 // パターン マッチングを使用します

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

	// public static void Main(params string[] args) {
	// 	new Solution().Start();
	// }

	// protected override void Solve() {
	// 	debugln("ans: " + string.Join(", ", LexicographicallySmallestArray(new int[] { 4, 52, 38, 59, 71, 27, 31, 83, 88, 10 }, 14)));
	// }

	public int[] LexicographicallySmallestArray(int[] nums, int limit) {
		var N = nums.Length;

		var num2node = new AATree<int, MyNode>();
		var index2node = new Dictionary<int, MyNode>();
		for (var index = 0; index < N; ++index) {
			var node = new MyNode() {
				num = nums[index],
				numIndex = index
			};
			num2node.Add(nums[index], node);
			index2node[index] = node;
		}

		for (var index = 0; index < N; ++index) {
			var curNode = index2node[index];
			var swapNode = FindSwapNode(num2node, nums[index], limit);

			if (swapNode is null) {
				num2node.Remove(curNode.num);
				continue;
			}

			(nums[index], nums[swapNode.numIndex]) = (nums[swapNode.numIndex], nums[index]);
			index2node[index] = swapNode;
			index2node[swapNode.numIndex] = curNode;

			curNode.numIndex = swapNode.numIndex;
			num2node.Remove(swapNode.num);
		}

		return nums;
	}

	private MyNode? FindSwapNode(AATree<int, MyNode> num2node, int num, int limit) {
		MyNode? result = null;
		while (true) {
			// num - target <= limit
			var node = num2node.Lowerbound(num - limit);
			if (node is null || node.num >= num) {
				return result;
			}
			result = node;
			num = result.num;
		}
	}

	public class MyNode {
		public int num;
		public int numIndex;
	}

	public class AATree<TKey, TValue> where TKey : IComparable<TKey> {
		private class Node {
			// Node internal data
			internal int level;
			internal Node left;
			internal Node right;

			// User data
			internal TKey key;
			internal TValue value;

			// Constuctor for the sentinel node
			internal Node() {
				this.level = 0;
				this.left = this;
				this.right = this;
			}

			// Constuctor for regular nodes (that all start life as leaf nodes)
			internal Node(TKey key, TValue value, Node sentinel) {
				this.level = 1;
				this.left = sentinel;
				this.right = sentinel;
				this.key = key;
				this.value = value;
			}
		}

		Node root;
		Node sentinel;
		Node? deleted;

		public AATree() {
			this.root = this.sentinel = new Node();
			this.deleted = null;
		}

		private void Skew(ref Node node) {
			if (node.level == node.left.level) {
				// rotate right
				var left = node.left;
				node.left = left.right;
				left.right = node;
				node = left;
			}
		}

		private void Split(ref Node node) {
			if (node.right.right.level == node.level) {
				// rotate left
				var right = node.right;
				node.right = right.left;
				right.left = node;
				node = right;
				node.level++;
			}
		}

		private bool Insert(ref Node node, TKey key, TValue value) {
			if (node == this.sentinel) {
				node = new Node(key, value, this.sentinel);
				return true;
			}

			var compare = key.CompareTo(node.key);
			if (compare < 0) {
				if (!this.Insert(ref node.left, key, value)) {
					return false;
				}
			}
			else if (compare > 0) {
				if (!this.Insert(ref node.right, key, value)) {
					return false;
				}
			}
			else {
				return false;
			}

			this.Skew(ref node);
			this.Split(ref node);

			return true;
		}

		private bool Delete(ref Node node, TKey key) {
			if (node == this.sentinel) {
				return this.deleted != null;
			}

			var compare = key.CompareTo(node.key);
			if (compare < 0) {
				if (!this.Delete(ref node.left, key)) {
					return false;
				}
			}
			else {
				if (compare == 0) {
					this.deleted = node;
				}
				if (!this.Delete(ref node.right, key)) {
					return false;
				}
			}

			var del = this.deleted;
			if (del != null) {
				del.key = node.key;
				del.value = node.value;
				this.deleted = null;
				node = node.right;
			}
			else if (node.left.level < node.level - 1 || node.right.level < node.level - 1) {
				--node.level;
				if (node.right.level > node.level) {
					node.right.level = node.level;
				}
				this.Skew(ref node);
				this.Skew(ref node.right);
				this.Skew(ref node.right.right);
				this.Split(ref node);
				this.Split(ref node.right);
			}

			return true;
		}

		private Node? Search(Node node, TKey key) {
			if (node == this.sentinel) {
				return null;
			}

			var compare = key.CompareTo(node.key);
			if (compare < 0) {
				return Search(node.left, key);
			}
			else if (compare > 0) {
				return Search(node.right, key);
			}
			else {
				return node;
			}
		}

		public bool Add(TKey key, TValue value) {
			return this.Insert(ref this.root, key, value);
		}

		public bool Remove(TKey key) {
			return this.Delete(ref this.root, key);
		}

		public TValue this[TKey key] {
			get {
				var node = this.Search(this.root, key);
				return node is null ? default : node.value;
			}
			set {
				var node = this.Search(root, key);
				if (node == null) {
					this.Add(key, value);
				}
				else {
					node.value = value;
				}
			}
		}

		public TValue? Lowerbound(TKey key) {
			var node = this.Lowerbound(this.root, key);
			return node is null ? default : node.value;
		}

		private Node? Lowerbound(Node node, TKey key) {
			if (node == this.sentinel) {
				return null;
			}

			var compare = key.CompareTo(node.key);
			if (compare < 0) {
				if (node.left is null || node.left.key.CompareTo(key) <= 0) {
					return node.left;
				}
				return this.Lowerbound(node.left, key);
			}

			if (compare > 0) {
				if (node.right is null || node.right.key.CompareTo(key) >= 0) {
					return node.right;
				}
				return this.Lowerbound(node.right, key);
			}

			return node;
		}
	}

	public IList<IList<int>> CombinationSum(int[] candidates, int target) {
		return dp(candidates, target, new List<int>());
	}

	private List<IList<int>> dp(int[] candidates, int target, List<int> removedIndices) {
		// 1. Contains candidates[i]
		// 2. Not contain candidates[i]
		var ans = new List<IList<int>>();
		if (candidates.Length - removedIndices.Count == 1) {
			var remainIndex = -1;
			for (var index = candidates.Length - 1; index >= 0; --index) {
				if (removedIndices.Contains(index)) {
					remainIndex = index;
					break;
				}
			}
			if (remainIndex >= 0 && candidates[remainIndex] == target) {
				ans.Add(new List<int> { candidates[remainIndex] });
			}
			return ans;
		}

		for (var index = 0; index < candidates.Length; ++index) {
			if (removedIndices.Contains(index)) {
				continue;
			}
			var indices1 = new List<int> { index };
			indices1.AddRange(removedIndices);
			ans.AddRange(dp(candidates, target, indices1));

			var indices2 = new List<int> { index };
			indices2.AddRange(removedIndices);
			ans.AddRange(dp(candidates, target - candidates[index], indices2));
		}
		return ans;
	}
}
