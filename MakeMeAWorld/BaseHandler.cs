// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Web;
using System.Web.Routing;

namespace MakeMeAWorld
{
    public abstract class BaseHandler
    {
        protected BaseHandler()
        {
            ((Global)HttpContext.Current.ApplicationInstance).Inject(this);
        }

        protected BaseHandler(RequestContext requestContext)
        {
            this.RequestContext = requestContext;
            ((Global)HttpContext.Current.ApplicationInstance).Inject(this);
        }
        
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        
        protected RequestContext RequestContext { get; set; }

        public abstract void ProcessRequest(HttpContext context);
    }
}
