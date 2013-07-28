using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using AchievementsGrabber.Common;
using AchievementsGrabber.Common.Extensions;
using AchievementsGrabber.Model.Default;

namespace AchievementsGrabber.Controllers
{
	public sealed partial class DefaultController : Controller, IXpathBuilderProvider
	{
		private static readonly Uri BaseUri = new Uri("http://www.xbox360achievements.org/");

		[HttpGet]
		public ActionResult Index(SearchModel model)
		{
			ViewBag.Search = model;

			return View(GetSearchResult(model));
		}

		private ResultModel GetSearchResult(SearchModel request)
		{
			var search = request.Text.Safe();
			if (string.IsNullOrWhiteSpace(search)) return new ResultModel();

			try
			{
				var html = BaseUri.Combine("search.php").Upload(new NameValueCollection {{"search", search}});
				return new ResultModel {Games = GetGames(html)};
			}
			catch (Exception e)
			{
				return new ResultModel {Error = e.Message};
			}
		}

		private IEnumerable<GameModel> GetGames(string html)
		{
			var xpath = this.Builder()
			                .Any("div").WithAttribute("id", "cont")
			                .Any("div").WithAttribute("id", "col_l")
			                .Child("div").WithAttribute("class", "bl_la_main")
			                .Child("div").WithAttribute("class", "divtext")
			                .Child("table")
			                .Any("tr").WithAttributes(Tuple.Create("class", "trA1"), Tuple.Create("class", "trA2")).ToString();

			var rows = html.Document().Root().Select(xpath).Safe().ToList();
			if (rows.Count == 0) throw new Exception("Could not find any games. Please redefine your search");

			return rows.Select(x => new {x, link = x.Single("td//a[@class='linkT']")}).Select(x => new GameModel
			{
				Image = BaseUri.Combine(x.x.Attributes("td//img", "src").First().Value).AbsoluteUri,
				Text = x.link.InnerText,
				Url = x.link.Attributes["href"].Value.Replace("/overview/", "/"),
			}).GroupBy(x => x.Text).Select(x => new {x, first = x.First()}).Select(@t => new GameModel
			{
				Image = @t.first.Image,
				Text = @t.first.Text,
				Url = @t.first.Url,
				Siblings = @t.x,
				Id = Guid.NewGuid()
			}).ToList();
		}
	}
}