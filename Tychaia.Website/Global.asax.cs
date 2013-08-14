// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using System.Web.WebPages.Scope;
using Ninject;
using RazorGenerator.Mvc;

namespace Tychaia.Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private void RegisterIoC(IKernel kernel)
        {
            kernel.Load<TychaiaWebsiteIoCModule>();
        }
    
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Home", "", new { controller = "Home", action = "Index" });
            routes.MapRoute("Download", "download", new { controller = "Download", action = "Index" });
            routes.MapRoute("Tuesday", "tuesday/{issue}", new { controller = "Tuesday", action = "Index", issue = 0 });
            routes.MapRoute("Wiki", "w/{*slug}",
                new { controller = "Wiki", action = "Index", slug = UrlParameter.Optional });
            routes.MapRoute("Cache", "clear-cache", new { controller = "Cache", action = "Index" });
        }

        protected void Application_Start()
        {
            var engine = new PrecompiledMvcEngine(typeof(MvcApplication).Assembly) {
                UsePhysicalViewsIfNewer = false,
                PreemptPhysicalFiles = true
            };

            ViewEngines.Engines.Insert(0, engine);

            // StartPage lookups are done by WebPages. 
            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
            
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            
            var kernel = new StandardKernel();
            this.RegisterIoC(kernel);

            ControllerBuilder.Current.SetControllerFactory(new DependencyControllerFactory(kernel));
        }

        protected void Application_BeginRequest()
        {
            // http://www.codinghub.net/2012/12/storage-scopes-cannot-be-created-when.html
            var ob = typeof(
                AspNetRequestScopeStorageProvider).Assembly.GetType(
                "System.Web.WebPages.WebPageHttpModule").GetProperty
                ("AppStartExecuteCompleted",
            BindingFlags.NonPublic | BindingFlags.Static);
            ob.SetValue(null, true, null);
        }
    }
}
