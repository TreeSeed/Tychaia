using System.Web.Mvc;
using Argotic.Syndication;
using System;
using System.Runtime.Caching;

namespace Tychaia.Website.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            AtomFeed feed = Phabricator.BlogCache.Get("summary-feed") as AtomFeed;
            if (feed == null)
            {
                feed = AtomFeed.Create(new System.Uri("http://code.redpointsoftware.com.au/phame/blog/feed/1/"));
                Phabricator.BlogCache.Add(
                    new CacheItem("summary-feed", feed),
                    new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15) }
                );
            }

            return View(new { Feed = feed });
        }
    }
}
