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
    public class FeedController : Controller
    {
        private readonly IPhabricator m_Phabricator;
        private readonly IConduitClientProvider m_ConduitClientProvider;

        public FeedController(IPhabricator phabricator, IConduitClientProvider conduitClientProvider)
        {
            this.m_Phabricator = phabricator;
            this.m_ConduitClientProvider = conduitClientProvider;
        }

        public ActionResult Index()
        {
            var posts = this.m_Phabricator.GetBlogPosts(
                this.m_ConduitClientProvider.GetConduitClient());

            Response.ContentType = "application/rss+xml";

            return View(new BlogIndexViewModel { Posts = posts });
        }
    }
}
