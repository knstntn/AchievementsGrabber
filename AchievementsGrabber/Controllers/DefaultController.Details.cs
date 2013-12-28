using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using AchievementsGrabber.Common;
using AchievementsGrabber.Common.Extensions;
using AchievementsGrabber.Model.Default;
using HtmlAgilityPack;

namespace AchievementsGrabber.Controllers
{
	public sealed partial class DefaultController
	{
		[HttpGet]
		public ActionResult Details(string url)
		{
			var root = BaseUri.Combine(url + "guide").Download().Document().Root();
			if (root == null) return RedirectToAction("Index");

			return View(new DetailsModel
			{
				Name = root.Title(this),
				Overview = root.Overview(this, BaseUri),
				Achievements = root.Achievements(this, BaseUri)
			});
		}
	}

	public static class GameExtensions
	{
		public static string Title(this HtmlNode game, IXpathBuilderProvider provider)
		{
			var xpath = provider.Builder()
			                    .Any("div").WithAttribute("id", "cont")
			                    .Any("div").WithAttribute("class", "bl_la_top")
			                    .Child("div").WithAttribute("class", "tt");

			return game.Single(xpath.ToString(), false).Get(x => x.InnerText);
		}

		public static OverviewModel Overview(this HtmlNode game, IXpathBuilderProvider provider, Uri baseuri)
		{
			var xpath = provider.Builder()
								.Any("div").WithAttribute("id", "cont")
								.Child("div").WithAttribute("id", "col_r")
								.Child("div").WithAttribute("class", "blr_main")
								.Child("div").WithAttribute("class", "divtext")
								.Child("div").WithAttribute("class", "men_h_content")
								.Child("table").Child("tr").ToString();

			var row = game.Single(xpath);
			if (row != null)
			{
				var cells = row.Select("td").ToArray();

				Debug.Assert(cells.Length == 2);

				var img = cells[0].Attributes("img", "src").FirstOrDefault().Get(x => x.Value);
				return new OverviewModel
				{
					Description = cells[1].InnerHtml,
					Image = img != null ? baseuri.Combine(img).AbsoluteUri : ""
				};
			}
			return new OverviewModel();
		}

		public static IEnumerable<AchievementModel> Achievements(this HtmlNode game, IXpathBuilderProvider provider, Uri baseuri)
		{
			// TODO: implement this correctly
			var xpath = provider.Builder()
			                    .Any("div").WithAttribute("id", "cont")
			                    .Any("table").WithAttribute("id", "dataTable")
			                    .Child("tr");

			var list = game.Select(xpath.ToString()).Safe().Skip(3).ToList()
			               .Select((x, i) => new {Index = i, Row = x})
			               .GroupBy(x => x.Index/3)
			               .Select(x => new {First = x.First().Row, Second = x.Skip(1).Take(1).First().Row, Third = x.Last().Row}).ToList();
			return (from item in list
			        let image = item.First.Attributes("td[@class='ac1']/a/img", "src").Select(x => x.Value).FirstOrDefault()
			        select new AchievementModel
			        {
				        Id = Guid.NewGuid(),
				        Image = string.IsNullOrEmpty(image) ? "" : baseuri.Combine(image).AbsoluteUri,
				        Title = item.First.Single("td[@class='ac2']/a/b").Get(x => x.InnerText),
				        Points = item.First.Single("td[@class='ac4']/strong").Get(x => x.InnerText).AsInt(),
				        Description = item.Second.Single("td[@class='ac3']").Get(x => x.InnerText),
				        IsSecret = item.First.Attributes["class"].Get(x => x.Value).Safe().Contains("secret"),
				        Guide = item.Third.Single("td[@class='ac6']").Get(x => x.InnerHtml),
			        }).Where(x => !string.IsNullOrWhiteSpace(x.Title)).ToList();
		}
	}
}