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
