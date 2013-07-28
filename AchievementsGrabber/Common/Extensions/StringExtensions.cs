using System.Globalization;

namespace AchievementsGrabber.Common.Extensions
{
	public static class StringExtensions
	{
		public static string Safe(this string s)
		{
			return s ?? string.Empty;
		}

		public static string Either(this string left, string right)
		{
			var l = left.Safe();
			var r = right.Safe();

			return string.IsNullOrWhiteSpace(l) ? r : l;
		}

		public static string Capitalize(this string s)
		{
			var str = (s ?? string.Empty).Trim().ToLower();

			return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str);
		}
	}
}