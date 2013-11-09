// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Linq;
using System.Web.Mvc;
using Tychaia.Website.Cachable;
using Tychaia.Website.ViewModels;

namespace Tychaia.Website.Controllers
{
    public class BlogController : Controller
    {
        private readonly IPhabricator m_Phabricator;
        private readonly IConduitClientProvider m_ConduitClientProvider;

        public BlogController(IPhabricator phabricator, IConduitClientProvider conduitClientProvider)
        {
            this.m_Phabricator = phabricator;
            this.m_ConduitClientProvider = conduitClientProvider;
        }

        public ActionResult Index()
        {
            var posts = this.m_Phabricator.GetBlogPosts(
                this.m_ConduitClientProvider.GetConduitClient());

            return View(new BlogIndexViewModel { Posts = posts });
        }

        public ActionResult Read(int issue)
        {
            var posts = this.m_Phabricator.GetBlogPosts(
                this.m_ConduitClientProvider.GetConduitClient());

            return View(new BlogReadViewModel { Post = posts.FirstOrDefault(x => x.ID == issue) });
        }
    }
}