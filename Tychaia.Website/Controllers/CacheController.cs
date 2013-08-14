// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Web.Mvc;
using Tychaia.Website.Cachable;

namespace Tychaia.Website.Controllers
{
    public class CacheController : Controller
    {
        private IPhabricator m_Phabricator;

        public CacheController(IPhabricator phabricator)
        {
            this.m_Phabricator = phabricator;
        }

        public ActionResult Index()
        {
            this.m_Phabricator.ClearCache();
            return RedirectToAction("Index", "Home");
        }
    }
}
