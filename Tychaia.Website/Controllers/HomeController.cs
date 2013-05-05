using System.Web.Mvc;
using Argotic.Syndication;
using System;

namespace Tychaia.Website.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var feed = AtomFeed.Create(new System.Uri("http://code.redpointsoftware.com.au/phame/blog/feed/1/"));

            return View(new { Feed = feed });
        }
    }
}
