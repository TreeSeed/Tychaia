using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tychaia.Website.Controllers
{
    public class CacheController : Controller
    {
        public ActionResult Index()
        {
            Phabricator.ClearCache();
            return RedirectToAction("Index", "Home");
        }
    }
}
