namespace Tool.Compet.Core {
	public class DkBitwise {
		/// @param index: From right -> left. At rightmost, it is 0.
		public static bool HasBitAt(int value, int index) {
			return ((value >> index) & 1) == 1;
		}
	}
}
