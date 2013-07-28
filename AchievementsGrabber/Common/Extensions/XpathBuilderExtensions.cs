using AchievementsGrabber.Model;

namespace AchievementsGrabber.Common.Extensions
{
	public static class XpathBuilderExtensions
	{
		public static XpathBuilder Builder(this IXpathBuilderProvider provider)
		{
			return new XpathBuilder();
		}
	}
}