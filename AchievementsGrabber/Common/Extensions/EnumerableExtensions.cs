using System.Collections.Generic;
using System.Linq;

namespace AchievementsGrabber.Common.Extensions
{
	/// <summary>
	/// Extensions for <see cref="IEnumerable{T}" />
	/// </summary>
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Converts single element to collection
		/// </summary>
		public static IEnumerable<T> Enumerate<T>(this T item)
		{
			yield return item;
		}

		/// <summary>
		/// Checks if provided collection is null and returns empty collection. Use to avoid NRE
		/// </summary>
		public static IEnumerable<T> Safe<T>(this IEnumerable<T> enumerable)
		{
			return enumerable ?? Enumerable.Empty<T>();
		}

		/// <summary>
		/// Straight forward implementation of FirstOrDefault with explicit default value
		/// </summary>
		public static T FirstOr<T>(this IEnumerable<T> enumerable, T @default)
		{
			using (var enumerator = enumerable.Safe().GetEnumerator())
			{
				if (enumerator.MoveNext()) return enumerator.Current;
			}
			return @default;
		}
	}
}