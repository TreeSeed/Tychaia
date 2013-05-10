using System.Web;
using System.IO;
using System.Reflection;

namespace Tychaia.Website
{
    public class TestHandler : BaseHandler, IHttpHandler
    {
        public override void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(context.Request.ApplicationPath + "\n");
            context.Response.Write(context.Request.Path + "\n");
            context.Response.Write(context.Request.PathInfo + "\n");
            context.Response.Write(context.Request.PhysicalApplicationPath + "\n");
            context.Response.Write(context.Request.PhysicalPath + "\n");
            
            context.Response.Write(HttpContext.Current + "\n");
            if (HttpContext.Current != null)
            {
                var req = HttpContext.Current.Request;
                context.Response.Write(req + "\n");
                if (req != null)
                {
                    // Reflection to get value as it's internal.
                    var type = req.GetType();
                    if (type != null)
                    {
                        var prop = type.GetProperty("BaseVirtualDir", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (prop != null)
                        {
                            var gett = prop.GetGetMethod(true);
                            if (gett != null)
                            {
                                var val = gett.Invoke(req, null) as string;
                                context.Response.Write("req base: " + val + "\n");
                            }
                            else
                            {
                                context.Response.Write("req get method not found.\n");
                            }
                        }
                        else
                        {
                            context.Response.Write("req prop not found.\n");
                        }
                    }
                    else
                    {
                        context.Response.Write("req type not found.\n");
                    }
                }
            }
            context.Response.Write(HttpRuntime.AppDomainAppVirtualPath);
        }
    }
}
