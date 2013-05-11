using System.Web.Mvc;
using Tychaia.Website.Models;
using Phabricator.Conduit;
using System.Configuration;

namespace Tychaia.Website.Controllers
{
    public class WikiController : Controller
    {
        public ActionResult Index(string slug)
        {
            if (slug == null)
                slug = "";
            if (!string.IsNullOrWhiteSpace(slug))
                slug = "/" + slug;
            slug = "tychaia" + slug;
            return View(GetContentFor(slug));
        }

        private WikiPageModel GetContentFor(string slug)
        {
            var client = new ConduitClient("http://code.redpointsoftware.com.au/api");
            client.Certificate = ConfigurationManager.AppSettings["ConduitCertificate"];
            client.User = ConfigurationManager.AppSettings["ConduitUser"];
            var infoResult = client.Do("phriction.info", new {
                slug = slug
            });
            var processResult = client.Do("remarkup.process", new {
                context = "phriction",
                content = infoResult.content
            });
            return new WikiPageModel
            {
                Title = infoResult.title,
                Content = new MvcHtmlString(processResult.content)
            };
        }
    }
}
