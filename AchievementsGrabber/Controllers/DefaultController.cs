using System.Web.Mvc;

namespace AchievementsGrabber.Controllers
{
	public class DefaultController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}
	}
}