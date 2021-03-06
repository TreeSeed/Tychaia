// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.IO;
using System.Reflection;
using System.Web;

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
            
            foreach (var f in new DirectoryInfo(context.Server.MapPath("~/_js")).GetFiles("*.js"))
            {
                context.Response.Write("/_js/" + f.Name + "\n");
            }
            
            foreach (var f in new DirectoryInfo(context.Server.MapPath("~/_css")).GetFiles("*.css"))
            {
                context.Response.Write("/_css/" + f.Name + "\n");
            }
            
            context.Response.Write("/\n");
            context.Response.Write("/Default.aspx\n");
            context.Response.Write("/explore\n");
            context.Response.Write("/gmap/index.htm\n");
            context.Response.Write("/gmap/default.css\n");
        }
    }
}
