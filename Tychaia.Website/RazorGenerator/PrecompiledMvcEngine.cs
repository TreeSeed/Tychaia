/** 
 * This is based on PrecompiledViews written by Chris van de Steeg. 
 * Code and discussions for Chris's changes are available at (http://www.chrisvandesteeg.nl/2010/11/22/embedding-pre-compiled-razor-views-in-your-dll/)
 **/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.WebPages;

namespace RazorGenerator.Mvc
{
    public class PrecompiledMvcEngine : VirtualPathProviderViewEngine, IVirtualPathFactory
    {
        private readonly IDictionary<string, Type> _mappings;
        private readonly string _baseVirtualPath;
        private readonly Lazy<DateTime> _assemblyLastWriteTime;
        private readonly IViewPageActivator _viewPageActivator;

        public PrecompiledMvcEngine(Assembly assembly)
            : this(assembly, null)
        {
        }

        public PrecompiledMvcEngine(Assembly assembly, string baseVirtualPath)
            : this(assembly, baseVirtualPath, null)
        {
        }

        public PrecompiledMvcEngine(Assembly assembly, string baseVirtualPath, IViewPageActivator viewPageActivator)
        {
            _assemblyLastWriteTime = new Lazy<DateTime>(() => assembly.GetLastWriteTimeUtc(fallback: DateTime.MaxValue));
            _baseVirtualPath = NormalizeBaseVirtualPath(baseVirtualPath);

            base.AreaViewLocationFormats = new[] {
                "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                "~/Areas/{2}/Views/Shared/{0}.cshtml", 
            };

            base.AreaMasterLocationFormats = new[] {
                "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                "~/Areas/{2}/Views/Shared/{0}.cshtml", 
            };

            base.AreaPartialViewLocationFormats = new[] {
                "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                "~/Areas/{2}/Views/Shared/{0}.cshtml", 
            };
            base.ViewLocationFormats = new[] {
                "~/Views/{1}/{0}.cshtml", 
                "~/Views/Shared/{0}.cshtml", 
            };
            base.MasterLocationFormats = new[] {
                "~/Views/{1}/{0}.cshtml", 
                "~/Views/Shared/{0}.cshtml", 
            };
            base.PartialViewLocationFormats = new[] {
                "~/Views/{1}/{0}.cshtml", 
                "~/Views/Shared/{0}.cshtml", 
            };
            base.FileExtensions = new[] {
                "cshtml", 
            };

            var results = (from type in assembly.GetTypes()
                         where typeof(WebPageRenderingBase).IsAssignableFrom(type)
                         let pageVirtualPath = type.GetCustomAttributes(inherit: false).OfType<PageVirtualPathAttribute>().FirstOrDefault()
                         where pageVirtualPath != null
                         select new KeyValuePair<string, Type>(CombineVirtualPaths(_baseVirtualPath, pageVirtualPath.VirtualPath), type)
                         );
            _mappings = new Dictionary<string, Type>();
            Console.WriteLine("Starting mapping..");
            foreach (var t in results)
            {
                Console.WriteLine("Creating mapping " + t.Key + ", " + t.Value);
                if (_mappings.Keys.Any(x => x.ToLower() == t.Key.ToLower()))
                {
                    Console.WriteLine("WARNING: Type " + t.Value + " has same " + 
                                      "key '" + t.Key + "' as type " +
                                      _mappings[t.Key] + ".  It will be ignored " +
                                      "for this key.");
                    continue;
                }
                _mappings.Add(t);
            }
            Console.WriteLine("Finished mapping..");
            this.ViewLocationCache = new PrecompiledViewLocationCache(assembly.FullName, this.ViewLocationCache);
            _viewPageActivator = viewPageActivator 
                ?? DependencyResolver.Current.GetService<IViewPageActivator>() /* For compatibility, remove this line within next version */
                ?? DefaultViewPageActivator.Current;
        }

        /// <summary>
        /// Determines if IVirtualPathFactory lookups returns files from assembly regardless of whether physical files are available for the virtual path.
        /// </summary>
        public bool PreemptPhysicalFiles
        {
            get; set;
        }

        /// <summary>
        /// Determines if the view engine uses views on disk if it's been changed since the view assembly was last compiled.
        /// </summary>
        /// <remarks>
        /// What an awful name!
        /// </remarks>
        public bool UsePhysicalViewsIfNewer
        {
            get; set;
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            Console.WriteLine("Precompiled view engine was asked if file exists.");
        
            if (UsePhysicalViewsIfNewer && IsPhysicalFileNewer(virtualPath))
            {
                // If the physical file on disk is newer and the user's opted in this behavior, serve it instead.
                return false;
            }
            var result = Exists(virtualPath);
            
            Console.WriteLine("Responded " + result + " for " + virtualPath);
            return result;
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return CreateViewInternal(partialPath, masterPath: null, runViewStartPages: false);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return CreateViewInternal(viewPath, masterPath, runViewStartPages: true);
        }

        private IView CreateViewInternal(string viewPath, string masterPath, bool runViewStartPages)
        {
            Type type;
            if (_mappings.TryGetValue(viewPath, out type))
            {
                return new PrecompiledMvcView(viewPath, masterPath, type, runViewStartPages, base.FileExtensions, _viewPageActivator);
            }
            return null;
        }

        public object CreateInstance(string virtualPath)
        {
            Type type;

            if (!PreemptPhysicalFiles && VirtualPathProvider.FileExists(virtualPath))
            {
                // If we aren't pre-empting physical files, use the BuildManager to create _ViewStart instances if the file exists on disk. 
                return BuildManager.CreateInstanceFromVirtualPath(virtualPath, typeof(WebPageRenderingBase));
            }

            if (UsePhysicalViewsIfNewer && IsPhysicalFileNewer(virtualPath))
            {
                // If the physical file on disk is newer and the user's opted in this behavior, serve it instead.
                return BuildManager.CreateInstanceFromVirtualPath(virtualPath, typeof(WebViewPage));
            }

            if (_mappings.TryGetValue(virtualPath, out type))
            {
                return _viewPageActivator.Create((ControllerContext)null, type);
            }
            return null;
        }

        public bool Exists(string virtualPath)
        {
            return _mappings.ContainsKey(virtualPath);
        }

        private bool IsPhysicalFileNewer(string virtualPath)
        {
            if (virtualPath.StartsWith(_baseVirtualPath ?? String.Empty, StringComparison.OrdinalIgnoreCase))
            {
                // If a base virtual path is specified, we should remove it as a prefix. Everything that follows should map to a view file on disk.
                if (!String.IsNullOrEmpty(_baseVirtualPath))
                {
                    virtualPath = '~' + virtualPath.Substring(_baseVirtualPath.Length);
                }

                string path = HttpContext.Current.Request.MapPath(virtualPath);
                return File.Exists(path) && File.GetLastWriteTimeUtc(path) > _assemblyLastWriteTime.Value;
            }
            return false;
        }

        private static string NormalizeBaseVirtualPath(string virtualPath)
        {
            if (!String.IsNullOrEmpty(virtualPath))
            {
                // For a virtual path to combine properly, it needs to start with a ~/ and end with a /.
                if (!virtualPath.StartsWith("~/", StringComparison.Ordinal))
                {
                    virtualPath = "~/" + virtualPath;
                }
                if (!virtualPath.EndsWith("/", StringComparison.Ordinal))
                {
                    virtualPath += "/";
                }
            }
            return virtualPath;
        }

        private static string CombineVirtualPaths(string baseVirtualPath, string virtualPath)
        {
            if (!String.IsNullOrEmpty(baseVirtualPath))
            {
                return VirtualPathUtility.Combine(baseVirtualPath, virtualPath.Substring(2));
            }
            return virtualPath;
        }
    }
}
