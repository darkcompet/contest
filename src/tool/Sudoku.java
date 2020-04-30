package tool;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.util.ArrayList;
import java.util.Arrays;

// javac contest/Sudoku.java ; java -ea contest.Sudoku 4
// javac -cp . contest/Sudoku.java ; java -cp . -ea contest.Sudoku 4

public class Sudoku {
	public static final int NOT_AVAILABLE = -1;
	public static final String SPACE = " ";
	public static final int SMALL = 4;
	public static final int MEDIUM = 9;
	public static final int LARGE = 16;
	public static final String FILE_SMALL = "4.txt";
	public static final String FILE_MEDIUM = "9.txt";
	public static final String FILE_LARGE = "16.txt";

	private int N, mTable[][], rootN;
	private boolean mEditable[][];
	private String mFilename;
	private MyList mList[][];

	private void solution(int table[][], Cell cell) {
		MySudoku sudoku = new MySudoku(table);
		MySudoku ans = solve(sudoku);
		if (ans == null) {
			print("no solution\n");
			return;
		}
		showTable(ans.getBoard());
	}

	private MySudoku solve(MySudoku sudoku) {
		if (sudoku == null || sudoku.isSolved())
			return sudoku;
		int cellNo = sudoku.nextCell();
		if (cellNo == -1) {
			return null;
		}
		MyCell candidate = sudoku.getCell(cellNo);
		for (int num = 1; num <= N; ++num) {
			if (candidate.assignable(num)) {
				MySudoku sdk = new MySudoku(sudoku);
				if (sdk.assign(cellNo, num)) {
					MySudoku ans = solve(sdk);
					if (ans != null)
						return ans;
				}
			}
		}
		return null;
	}

	private void isValid(int table[][]) {
		int len = table.length;
		for (int col = 0; col < len; ++col) {
			boolean uniqueCol[] = new boolean[len + 1];
			for (int row = 0; row < len; ++row) {
				if (table[row][col] != 0) {
					if (uniqueCol[table[row][col]]) {
						print("invalid table! overlap in column %d, %d\n", row + 1, col + 1);
						System.exit(-1);
					} else {
						uniqueCol[table[row][col]] = true;
					}
				}
			}
		}

		for (int row = 0; row < len; ++row) {
			boolean uniqueRow[] = new boolean[len + 1];
			for (int col = 0; col < len; ++col) {
				if (table[row][col] != 0) {
					if (uniqueRow[table[row][col]]) {
						print("invalid table! overlap in row %d, %d\n", row + 1, col + 1);
						System.exit(-1);
					} else {
						uniqueRow[table[row][col]] = true;
					}
				}
			}
		}

		for (int i = 0; i < len; i += rootN) {
			for (int j = 0; j < len; j += rootN) {
				// check for each small box
				boolean unique[] = new boolean[len + 1];
				for (int k = i; k < i + rootN; ++k) {
					for (int l = j; l < j + rootN; ++l) {
						if (table[k][l] != 0) {
							if (unique[table[k][l]]) {
								print("invalid table! overlap in box start at %d, %d\n", i + 1, j + 1);
								System.exit(-1);
							} else {
								unique[table[k][l]] = true;
							}
						}
					}
				}
			}
		}
	}

	private boolean solve(int table[][], Cell cell) {
		if (!findUnassignedCell(table, cell)) {
			return true;
		}

		int nextRow = cell.row, nextCol = cell.col;
		mList[nextRow][nextCol] = getNumListForCell(table, nextRow, nextCol);
		for (int i = 0; i < mList[nextRow][nextCol].size(); ++i) {
			int num = mList[nextRow][nextCol].get(i);
			table[nextRow][nextCol] = num;
			if (solve(table, cell)) {
				return true;
			}
			table[nextRow][nextCol] = 0;
		}
		return false; // trigger backtracking
	}

	private boolean findUnassignedCell(int table[][], Cell cell) {
		for (int i = 0; i < table.length; ++i) {
			for (int j = 0; j < table.length; ++j) {
				if (table[i][j] == 0) {
					cell.row = i;
					cell.col = j;
					return true;
				}
			}
		}
		return false;
	}

	private MyList getNumListForCell(int table[][], int row, int col) {
		MyList list = new MyList();
		for (int num = 1; num <= N; ++num) {
			if (isPuttable(table, num, row, col)) {
				list.add(num);
			}
		}
		return list;
	}

	private boolean isPuttable(int table[][], int num, int row, int col) {
		for (int i = 0; i < table.length; ++i) {
			if (table[row][i] != 0) {
				if (table[row][i] == num || table[i][col] == num)
					return false;
			}
		}

		for (int i = row - row % rootN; i < row - row % rootN + rootN; ++i) {
			for (int j = col - col % rootN; j < col - col % rootN + rootN; ++j) {
				if (table[i][j] == num) {
					return false;
				}
			}
		}

		return true;
	}

	private void showTable(int table[][]) {
		for (int i = 0; i < table.length; ++i) {
			for (int j = 0; j < table.length; ++j) {
				print("%d ", table[i][j]);
			}
			print("\n");
		}
	}

	public Sudoku(String[] args) {
		if (args.length > 0) {
			N = Integer.parseInt(args[0]);
			myAssert((N == SMALL || N == MEDIUM || N == LARGE), String.format("Not supported table %dx%d\n", N, N));
			switch (N) {
				case SMALL:
					mFilename = FILE_SMALL;
					break;
				case MEDIUM:
					mFilename = FILE_MEDIUM;
					break;
				case LARGE:
					mFilename = FILE_LARGE;
					break;
			}
		} else {
			myAssert(false, "usage1: javac experiment/Sudoku.java ; java -ea experiment.Sudoku 4\n"
					+ "usage2: javac -cp . experiment/Sudoku.java ; java -cp . -ea experiment.Sudoku 4\n");
		}

		{
			rootN = (int) Math.sqrt(N + 0.5);
			mTable = new int[N][N];
			mEditable = new boolean[N][N];
			mList = new MyList[N][N];
		}

		// read data from file
		try {
			int currentRow = 0;
			String line;
			String[] elems;
			String file = new File("").getAbsolutePath();
			file += "/experiment/" + mFilename; // terminal
			// file += "/src/experiment/" + mFilename; // eclipse
			BufferedReader br = new BufferedReader(new FileReader(new File(file)));
			while ((line = br.readLine()) != null) {
				elems = line.split(SPACE);
				for (int i = 0; i < elems.length; ++i) {
					mTable[currentRow][i] = Integer.parseInt(elems[i]);
					if (mTable[currentRow][i] == 0) {
						mEditable[currentRow][i] = true;
					}
				}
				currentRow++;
			}

			br.close();
		} catch (Exception e) {
			e.printStackTrace();
		}

		solution(mTable, new Cell(0, 0));
	}

	public static void main(String[] args) {
		long st = System.currentTimeMillis();
		new Sudoku(args);
		print("Consumed: %.5f(s)\n", (System.currentTimeMillis() - st) / 1000.0);
	}

	public static void print(String format, Object... args) {
		System.out.print(String.format(format, args));
	}

	public static void debug(String format, Object... args) {
		System.out.print(String.format(format, args));
	}

	public static void myAssert(boolean require, String format, Object... args) {
		if (!require) {
			System.out.print(String.format(format, args));
			System.exit(-1);
		}
	}

	class Cell {
		int row, col;

		public Cell(int row, int col) {
			this.row = row;
			this.col = col;
		}
	}

	class MyList extends ArrayList<Integer> {
		private static final long serialVersionUID = 1L;
	}

	class MyCell {
		boolean[] number;

		public MyCell() {
			number = new boolean[N];
			Arrays.fill(number, true);
		}

		public MyCell(boolean[] num) {
			number = Arrays.copyOf(num, N);
		}

		boolean assignable(int value) {
			assert (value > 0);
			return number[value - 1];
		}

		void remove(int value) {
			assert (value > 0);
			number[value - 1] = false;
		}

		int count() {
			int ret = 0;
			for (int i = 0; i < N; ++i)
				if (number[i]) {
					ret++;
				}
			return ret;
		}

		int get() {
			for (int i = 0; i < N; ++i)
				if (number[i]) {
					return (i + 1);
				}
			return -1;
		}

		boolean[] getNumber() {
			return number;
		}
	}

	class MySudoku {
		MyCell[] cells;
		MyList[] groups, neighbors, memberOf;

		public MySudoku(int[][] board) {
			cells = new MyCell[N * N];
			groups = new MyList[3 * N];
			neighbors = new MyList[N * N];
			memberOf = new MyList[N * N];

			for (int i = 0; i < 3 * N; ++i) {
				groups[i] = new MyList();
			}
			for (int i = 0; i < N * N; ++i) {
				cells[i] = new MyCell();
				neighbors[i] = new MyList();
				memberOf[i] = new MyList();
			}
			for (int row = 0; row < N; ++row) {
				for (int col = 0; col < N; ++col) {
					int id = N * row + col;
					int[] groupId = {row, N + col, N * 2 + (row / rootN) * rootN + col / rootN};
					for (int i = 0; i < 3; ++i) {
						groups[groupId[i]].add(id);
						memberOf[id].add(groupId[i]);
					}
				}
			}
			for (int id = 0; id < N * N; ++id) {
				for (int g = 0; g < 3; ++g) {
					for (int i = 0; i < N; ++i) {
						int nei = groups[memberOf[id].get(g)].get(i);
						if (nei != id)
							neighbors[id].add(nei);
					}
				}
			}
			for (int row = 0; row < N; ++row) {
				for (int col = 0; col < N; ++col) {
					if (board[row][col] != 0) {
						if (!assign(N * row + col, board[row][col])) {
							print("invalid board, unable to set value at (%d, %d)\n", row + 1, col + 1);
							System.exit(0);
						}
					}
				}
			}
		}

		public MySudoku(MySudoku sudoku) {
			MyCell[] tmpCell = sudoku.getCells();
			MyList[] tmpGroup = sudoku.getGroups();
			MyList[] tmpNeighbors = sudoku.getNeighbors();
			MyList[] tmpMemberOf = sudoku.getMemberOf();

			cells = new MyCell[N * N];
			groups = new MyList[3 * N];
			neighbors = new MyList[N * N];
			memberOf = new MyList[N * N];
			for (int i = 0; i < 3 * N; ++i) {
				groups[i] = (MyList) tmpGroup[i].clone();
			}
			for (int i = 0; i < N * N; ++i) {
				cells[i] = new MyCell(tmpCell[i].getNumber());
				neighbors[i] = (MyList) tmpNeighbors[i].clone();
				memberOf[i] = (MyList) tmpMemberOf[i].clone();
			}
		}

		boolean remove(int id, int value) {
			if (!cells[id].assignable(value))
				return true;
			cells[id].remove(value);
			int numOfAssignable = cells[id].count();
			if (numOfAssignable == 0)
				return false;
			if (numOfAssignable == 1) {
				int num = cells[id].get();
				for (int i = 0; i < neighbors[id].size(); ++i) {
					if (!remove(neighbors[id].get(i), num)) {
						return false;
					}
				}
			}
			for (int g = 0; g < memberOf[id].size(); ++g) {
				int groupId = memberOf[id].get(g);
				int count = 0, newId = -1;
				for (int i = 0; i < groups[groupId].size(); ++i) {
					int cellNo = groups[groupId].get(i);
					if (cells[cellNo].assignable(value)) {
						count++;
						newId = cellNo;
					}
				}
				if (count == 0)
					return false;
				if (count == 1 && !assign(newId, value))
					return false;
			}
			return true;
		}

		boolean assign(int id, int value) {
			for (int num = 1; num <= N; ++num) {
				if (num != value && !remove(id, num))
					return false;
			}
			return true;
		}

		int nextCell() {
			int cellNo = -1, mini = Integer.MAX_VALUE;
			for (int i = 0; i < N * N; ++i) {
				int count = cells[i].count();
				if (count > 1 && count < mini) {
					mini = count;
					cellNo = i;
				}
			}
			return cellNo;
		}

		boolean isSolved() {
			for (int id = 0; id < N * N; ++id) {
				if (cells[id].count() != 1)
					return false;
			}
			return true;
		}

		MyCell getCell(int id) {
			return cells[id];
		}

		int[][] getBoard() {
			int[][] ans = new int[N][N];
			for (int i = 0; i < N; ++i) {
				for (int j = 0; j < N; ++j) {
					ans[i][j] = cells[N * i + j].get();
				}
			}
			return ans;
		}

		MyCell[] getCells() {
			return cells;
		}

		MyList[] getNeighbors() {
			return neighbors;
		}

		MyList[] getMemberOf() {
			return memberOf;
		}

		MyList[] getGroups() {
			return groups;
		}
	}
}
