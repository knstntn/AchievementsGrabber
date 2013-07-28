using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace AchievementsGrabber.Common.Extensions
{
	// Extensions for HTML agility pack, to make parsing cleaner and remove redundant checked
	public static class HtmlParsingExtensions
	{
		public static HtmlDocument Document(this string html)
		{
			if (string.IsNullOrWhiteSpace(html)) return null;

			try
			{
				var document = new HtmlDocument();
				document.OptionFixNestedTags = true;
				document.OptionAutoCloseOnEnd = true;
				document.LoadHtml(html);
				return document;
			}
			catch { }

			return null;
		}

		public static HtmlNode Root(this HtmlDocument document)
		{
			return document != null ? document.DocumentNode : null;
		}

		public static IEnumerable<HtmlNode> Select(this HtmlNode node, string xpath)
		{
			if (node == null) return Enumerable.Empty<HtmlNode>();
			if (string.IsNullOrEmpty(xpath)) return Enumerable.Empty<HtmlNode>();

			return node.SelectNodes(xpath);
		}

		public static HtmlNode Single(this HtmlNode node, string xpath, bool required = true)
		{
			var nodes = node.Select(xpath).Safe().ToList();
			if (!nodes.Any()) return null;

			return required ? nodes.First() : nodes.FirstOrDefault();
		}

		public static IEnumerable<HtmlAttribute> Attributes(this HtmlNode node, string xpath, string name = null)
		{
			var attr = node.Select(xpath).Safe().SelectMany(x => x.Attributes).Safe();
			return string.IsNullOrEmpty(name) ? attr : attr.Where(x => x.Name == name);
		}

		public static IEnumerable<string> GetMetaAttributes(this HtmlNode root, params string[] names)
		{
			return from name in names
				   let xpath = new XpathBuilder().Any("meta").WithAttribute("name", name)
				   from attribute in root.Attributes(xpath.ToString())
				   where "content".Equals(attribute.Name, StringComparison.InvariantCultureIgnoreCase)
				   select attribute.Value.Safe();
		}
	}
}