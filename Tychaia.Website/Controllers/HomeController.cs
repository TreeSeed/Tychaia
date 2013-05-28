using System.Web.Mvc;
using Tychaia.Website.ViewModels;

namespace Tychaia.Website.Controllers
{
    public class HomeController : Controller
    {
        private IPhabricator m_Phabricator;

        public HomeController(IPhabricator phabricator)
        {
            this.m_Phabricator = phabricator;
        }

        public ActionResult Index()
        {
            return View(
                new FeedViewModel
                {
                    Feed = this.m_Phabricator.GetFeed("1")
                });
        }
    }
}
