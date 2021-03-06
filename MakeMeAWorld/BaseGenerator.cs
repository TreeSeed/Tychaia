// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Ninject;
using Tychaia.ProceduralGeneration;

namespace MakeMeAWorld
{
    public abstract class BaseGenerator : BaseHandler, IHttpHandler
    {
        [Inject]
        public IStorageAccess StorageAccess { protected get; set; }

        public override sealed void ProcessRequest(HttpContext context)
        {
            // Read in provided parameters, using either the fast or slow
            // API URLs.
            GenerationRequest request;
            if (context.Request.Url.AbsolutePath.StartsWith("/api-v1/", StringComparison.Ordinal))
            {
                // The format of this URL allows Nginx to automatically
                // serve the resource if it already exists, which the
                // query string version does not.  However it means that
                // we have to parse out the path components of the request
                // so that we have the information we want.
                var components = context.Request.Url.AbsolutePath.Substring("/api-v1/".Length).Split('/');
                if (components.Length != 7)
                    throw new HttpException(500, "Not enough URL components to determine request.");
                request = new GenerationRequest
                {
                    X = Convert.ToInt64(components[2]),
                    Y = Convert.ToInt64(components[3]),
                    Z = Convert.ToInt64(components[4]),
                    Size = Convert.ToInt32(components[5]),
                    Seed = Convert.ToInt64(components[1]),
                    LayerName = HttpUtility.UrlDecode(components[0]),
                    Packed = components[6].Contains("_packed"),
                    AsSquare = components[6].Contains("_square")
                };
                if (request.LayerName.Contains(".") || request.LayerName.Contains("/"))
                    throw new HttpException("Layer name is not valid.");
                var permittedNames = new[]
                {
                    "get.png",
                    "get_square.png",
                    "get.json",
                    "get_packed.json"
                };
                if (!permittedNames.Contains(components[6]))
                {
                    var message = 
                        "The final component of the URL must be one of {" +
                        permittedNames.Aggregate((a, b) => a + ", " + b) +
                        "} for fast caching to work.";
                    throw new HttpException(
                        500,
                        message);
                }
            }
            else
            {
                request = new GenerationRequest
                {
                    X = Convert.ToInt64(context.Request.QueryString["x"]),
                    Y = Convert.ToInt64(context.Request.QueryString["y"]),
                    Z = Convert.ToInt64(context.Request.QueryString["z"]),
                    Size = Convert.ToInt32(context.Request.QueryString["size"]),
                    Seed = Convert.ToInt64(context.Request.QueryString["seed"]),
                    LayerName = context.Request.QueryString["layer"],
                    Packed = Convert.ToBoolean(context.Request.QueryString["packed"]),
                    AsSquare = Convert.ToBoolean(context.Request.QueryString["as_square"])
                };
            }

            // Force the size to be 64x64x64.
            request.Size = Math.Max(0, request.Size);
            request.Size = Math.Min(request.Size, 128);

            // Load the configuration.
            var layer = this.CreateLayerFromConfig(context.Server.MapPath("~/bin/WorldConfig.xml"), request);
            if (layer == null)
                throw new HttpException(404, "The layer name was invalid");

            // Handle with cache if possible.
            if (this.ProcessCache(request, context))
                return;

            // Generate the requested data.
            int computations;
            layer.SetSeed(request.Seed);
            var start = DateTime.Now;
            var data = layer.GenerateData(
                request.X,
                request.Y,
                layer.Algorithm.Is2DOnly ? 0 : request.Z,
                request.Size,
                request.Size,
                layer.Algorithm.Is2DOnly ? 1 : request.Size,
                out computations);
            var end = DateTime.Now;

            // Store the result.
            var generation = new GenerationResult
            {
                Request = request,
                Layer = layer,
                Data = data,
                Computations = computations,
                TotalTime = end - start
            };

            // 3D layers can be optimized to empty results.
            if (!layer.Algorithm.Is2DOnly && DataIsAllSame(data))
            {
                this.ProcessEmpty(generation, context);
                return;
            }

            // Handle with generation processing.
            this.ProcessGeneration(generation, context);
        }

        public List<string> GetListOfAvailableLayers(HttpContext context)
        {
            return this.GetListOfAvailableLayers(context.Server.MapPath("~/bin/WorldConfig.xml"));
        }

        public string GetDefaultAvailableLayer(HttpContext context)
        {
            return this.GetDefaultAvailableLayer(context.Server.MapPath("~/bin/WorldConfig.xml"));
        }

        /// <summary>
        /// Processes a request by checking the cache and handling it if possible.  Returns
        /// true if the request has been handled by the cache.
        /// </summary>
        protected abstract bool ProcessCache(GenerationRequest request, HttpContext context);

        /// <summary>
        /// Processes an empty 3D result (when all of the values are the same).
        /// </summary>
        protected abstract void ProcessEmpty(GenerationResult result, HttpContext context);

        /// <summary>
        /// Processes a request using the generated data and layer information.
        /// </summary>
        protected abstract void ProcessGeneration(GenerationResult result, HttpContext context);
        
        protected string GetCacheName(GenerationRequest request, HttpContext context, string extension)
        {
            if (request.LayerName.Contains(".") || request.LayerName.Contains("/"))
                throw new HttpException("Layer name is not valid.");
            var cacheComponents = new string[]
            {
                "~",
                "App_Cache",
                "api-v1",
                request.LayerName,
                request.Seed.ToString(),
                request.X.ToString(),
                request.Y.ToString(),
                request.Z.ToString(),
                request.Size.ToString(),
                "get" + (request.AsSquare ? "_square" : string.Empty) +
                (request.Packed ? "_packed" : string.Empty) + "." + extension
            };
            
            for (var i = 0; i < cacheComponents.Length - 1; i++)
            {
                var path = context.Server.MapPath(cacheComponents.Where((x, id) => id <= i).Aggregate((a, b) => a + "/" + b));
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            
            return context.Server.MapPath(cacheComponents.Aggregate((a, b) => a + "/" + b));
        }

        private static bool DataIsAllSame(dynamic data)
        {
            for (var i = 0; i < data.Length; i++)
                if (data[i] != data[0])
                    return false;
            return true;
        }

        #region Data Loading

        private RuntimeLayer CreateLayerFromConfig(string path, GenerationRequest request)
        {
            // Use StorageAccess to load reference to world generation.
            StorageLayer[] layers;
            using (var reader = new StreamReader(path))
                layers = this.StorageAccess.LoadStorage(reader);
            foreach (var layer in layers)
                if ((layer.Algorithm is AlgorithmResult) &&
                    (layer.Algorithm as AlgorithmResult).Name == request.LayerName &&
                    ((layer.Algorithm as AlgorithmResult).ShowInMakeMeAWorld ||
                    (layer.Algorithm as AlgorithmResult).PermitInMakeMeAWorld))
                    return this.StorageAccess.ToRuntime(layer);
            return null;
        }

        private List<string> GetListOfAvailableLayers(string path)
        {
            var result = new List<string>();

            // Use StorageAccess to load reference to world generation.
            StorageLayer[] layers;
            using (var reader = new StreamReader(path))
                layers = this.StorageAccess.LoadStorage(reader);
            foreach (var layer in layers)
                if (layer.Algorithm is AlgorithmResult)
                if ((layer.Algorithm as AlgorithmResult).ShowInMakeMeAWorld)
                    result.Add(
                        (layer.Algorithm.Is2DOnly ? "2D," : "3D,") +
                        (layer.Algorithm as AlgorithmResult).Name);

            return result;
        }

        private string GetDefaultAvailableLayer(string path)
        {
            // Use StorageAccess to load reference to world generation.
            StorageLayer[] layers;
            using (var reader = new StreamReader(path))
                layers = this.StorageAccess.LoadStorage(reader);
            foreach (var layer in layers)
                if (layer.Algorithm is AlgorithmResult)
                if ((layer.Algorithm as AlgorithmResult).DefaultForMakeMeAWorld)
                    return (layer.Algorithm.Is2DOnly ? "2D," : "3D,") +
                        (layer.Algorithm as AlgorithmResult).Name;

            return null;
        }

        #endregion
        
        protected class GenerationRequest
        {
            public long X { get; set; }
            public long Y { get; set; }
            public long Z { get; set; }
            public int Size { get; set; }
            public long Seed { get; set; }
            public string LayerName { get; set; }
            public bool Packed { get; set; }
            public bool AsSquare { get; set; }
        }

        protected class GenerationResult
        {
            public GenerationRequest Request { get; set; }
            public RuntimeLayer Layer { get; set; }
            public int[] Data { get; set; }
            public int Computations { get; set; }
            public TimeSpan TotalTime { get; set; }
        }
    }
}
