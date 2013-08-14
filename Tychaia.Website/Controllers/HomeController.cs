// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Web.Mvc;
using Tychaia.Website.Cachable;
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
                    Feed = this.m_Phabricator.GetFeed("1"),
                    PostID = null
                });
        }
    }
}
