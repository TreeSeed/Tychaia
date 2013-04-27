using System.Web;
using System.IO;
using System.Reflection;

namespace MakeMeAWorld
{
    public class CacheManifestHandler : BaseHandler, IHttpHandler
    {
        public override void ProcessRequest(HttpContext context)
        {
            context.Response.Write("CACHE MANIFEST\n");
            context.Response.Write("# " + File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location) + "\n");
            context.Response.Write("NETWORK:\n");
            context.Response.Write("*\n");
            context.Response.Write("FALLBACK:\n");
            context.Response.Write("/ /Default.aspx\n");
            context.Response.Write("CACHE:\n");
            foreach (var f in new DirectoryInfo("_js").GetFiles("*.js"))
            {
                context.Response.Write("/_js/" + f.Name + "\n");
            }
            foreach (var f in new DirectoryInfo("_css").GetFiles("*.css"))
            {
                context.Response.Write("/_css/" + f.Name + "\n");
            }
        }
    }
}