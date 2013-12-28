using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using AchievementsGrabber.Model;

namespace AchievementsGrabber.Common.Extensions
{
	public static class UriExtensions
	{
		public static bool IsAbsoluteUrl(this string url)
		{
			if (string.IsNullOrWhiteSpace(url)) return false;

			Uri result;
			return Uri.TryCreate(url, UriKind.Absolute, out result);
		}

		// Joins absolute Uri and relative string into Uri
		public static Uri Combine(this Uri uri, string relative)
		{
			return new Uri(uri, relative);
		}

		// Appends relative url to existing url
		public static Uri Append(this Uri uri, string relative)
		{
			var abs = uri.AbsoluteUri.Safe().TrimEnd('/');
			var rel = relative.Safe().TrimStart('/');

			return new Uri(abs + "/" + rel);
		}

		// Downloads HTML from specified url
		public static string Download(this Uri uri)
		{
			return Download(uri, UriContext.Default);
		}

		public static string Download(this Uri uri, UriContext context)
		{
			return Download(uri, context, exception => { }); // suppressing exception by default
		}

		public static string Download(this Uri uri, UriContext context, Action<Exception> handler)
		{
			try
			{
				return uri.DownloadImpl(context, (client, u) => client.DownloadString(u));
			}
			catch (Exception e)
			{
				if (handler != null)
				{
					handler(e);
					return string.Empty;
				}

				throw;
			}
		}

		// Uploads values to the specified url and returns HTML response from the server
		public static string Upload(this Uri uri, NameValueCollection collection)
		{
			return Upload(uri, collection, UriContext.Default);
		}

		public static string Upload(this Uri uri, NameValueCollection collection, UriContext context)
		{
			return uri.DownloadImpl(context, (client, u) =>
			{
				var response = client.UploadValues(u, "POST", collection);
				return Encoding.UTF8.GetString(response);
			});
		}

		private static string DownloadImpl(this Uri uri, UriContext context, Func<WebClient, Uri, string> grab)
		{
			context = context ?? UriContext.Default;

			using (var client = new WebClient())
			{
				client.Encoding = context.Encoding ?? Encoding.UTF8;

				if (context.IncludeDefaultHeaders)
				{
					// TODO: pass those via context
					client.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml");
					client.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
					client.Headers.Add(HttpRequestHeader.Referer, "http://www.xboxachievements.com/search.php");
					client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
				}

				// headers given with context should overwrite default headers
				client.Headers.Add(context.Headers ?? new NameValueCollection());

				return grab(client, uri);
			}
		}
	}
}