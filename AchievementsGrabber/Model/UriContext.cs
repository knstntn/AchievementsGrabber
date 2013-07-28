using System.Collections.Specialized;
using System.Text;
using AchievementsGrabber.Common.Extensions;

namespace AchievementsGrabber.Model
{
	/// Represents context for <see cref="UriExtensions"/>
	public sealed class UriContext
	{
		public static UriContext Default = new UriContext
		{
			Encoding = Encoding.UTF8,
			Headers = new NameValueCollection(),
			IncludeDefaultHeaders = true
		};

		public Encoding Encoding { get; set; }
		public NameValueCollection Headers { get; set; }
		public bool IncludeDefaultHeaders { get; set; }
	}
}