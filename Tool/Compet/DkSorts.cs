// namespace Tool.Compet.Core {
	public class DkSorts {
		/// @return (minValue, maxValue, minIndex, maxIndex)
		public static (int, int, int, int) FindMinMaxWithIndices(int[] arr) {
			var minIndex = 0;
			var maxIndex = 0;
			var minValue = arr[minIndex];
			var maxValue = arr[maxIndex];

			for (var index = arr.Length - 1; index >= 0; --index) {
				var value = arr[index];
				if (minValue > value) {
					minValue = value;
					minIndex = index;
				}
				if (maxValue < value) {
					maxValue = value;
					maxIndex = index;
				}
			}

			return (minValue, maxValue, minIndex, maxIndex);
		}
	}
// }
