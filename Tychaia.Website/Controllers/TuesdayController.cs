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
    public class TuesdayController : Controller
    {
        private IPhabricator m_Phabricator;
        private IConduitClientProvider m_ConduitClientProvider;

        public TuesdayController(IPhabricator phabricator, IConduitClientProvider conduitClientProvider)
        {
            this.m_Phabricator = phabricator;
            this.m_ConduitClientProvider = conduitClientProvider;
        }

        public ActionResult Index(int issue)
        {
            var content = this.m_Phabricator.GetTychaiaTuesdayIssue(
                this.m_ConduitClientProvider.GetConduitClient(),
                issue);

            return View(
                new TychaiaTuesdayViewModel
                {
                    Issue = content
                });
        }
    }
}
