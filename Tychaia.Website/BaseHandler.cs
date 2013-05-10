using System.Web;
using System.Web.Routing;

namespace Tychaia.Website
{
    public abstract class BaseHandler
    {
        public bool IsReusable { get { return false; } }
        protected RequestContext RequestContext { get; set; }

        public BaseHandler() : base()
        {
        }

        public BaseHandler(RequestContext requestContext)
        {
            this.RequestContext = requestContext;
        }

        public abstract void ProcessRequest(HttpContext context);
    }
}
