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
    public class DownloadController : Controller
    {
        private IBuildServer m_BuildServer;

        public DownloadController(IBuildServer buildServer)
        {
            this.m_BuildServer = buildServer;
        }

        public ActionResult Index()
        {
            return View(new DownloadViewModel {
                BuildServerOnline = this.m_BuildServer.IsBuildServerOnline()
            });
        }
    }
}
