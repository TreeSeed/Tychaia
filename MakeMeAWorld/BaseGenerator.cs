//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

using System;
using System.Web;
using Tychaia.ProceduralGeneration;
using System.IO;
using System.Collections.Generic;

namespace MakeMeAWorld
{
    public abstract class BaseGenerator : BaseHandler, IHttpHandler
    {
        protected class GenerationRequest
        {
            public long X;
            public long Y;
            public long Z;
            public int Size;
            public long Seed;
            public string LayerName;
            public bool Packed;
        }

        protected class GenerationResult
        {
            public GenerationRequest Request;
            public RuntimeLayer Layer;
            public int[] Data;
            public int Computations;
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

        public override sealed void ProcessRequest(HttpContext context)
        {
            // Read in provided parameters.
            var request = new GenerationRequest
            {
                X = Convert.ToInt64(context.Request.QueryString["x"]),
                Y = Convert.ToInt64(context.Request.QueryString["y"]),
                Z = Convert.ToInt64(context.Request.QueryString["z"]),
                Size = Convert.ToInt32(context.Request.QueryString["size"]),
                Seed = Convert.ToInt64(context.Request.QueryString["seed"]),
                LayerName = context.Request.QueryString["layer"],
                Packed = Convert.ToBoolean(context.Request.QueryString["packed"])
            };

            // Force the size to be 64x64x64.
            request.Size = Math.Max(0, request.Size);
            request.Size = Math.Min(request.Size, 128);

            // Load the configuration.
            var layer = CreateLayerFromConfig(context.Server.MapPath("~/bin/WorldConfig.xml"), request);
            if (layer == null)
                throw new HttpException(404, "The layer name was invalid");

            // Handle with cache if possible.
            if (this.ProcessCache(request, context))
                return;

            // Generate the requested data.
            int computations;
            layer.Seed = request.Seed;
            var data = layer.GenerateData(
                request.X, 
                request.Y, 
                layer.Algorithm.Is2DOnly ? 0 : request.Z,
                request.Size,
                request.Size,
                layer.Algorithm.Is2DOnly ? 1 : request.Size,
                out computations);
            
            // Store the result.
            var generation = new GenerationResult
            {
                Request = request,
                Layer = layer,
                Data = data,
                Computations = computations
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

        private static bool DataIsAllSame(dynamic data)
        {
            for (var i = 0; i < data.Length; i++)
                if (data[i] != data[0])
                    return false;
            return true;
        }
        
        #region Data Loading
        
        private static RuntimeLayer CreateLayerFromConfig(string path, GenerationRequest request)
        {
            // Use StorageAccess to load reference to world generation.
            StorageLayer[] layers;
            using (var reader = new StreamReader(path))
                layers = StorageAccess.LoadStorage(reader);
            foreach (var layer in layers)
                if ((layer.Algorithm is AlgorithmResult) &&
                    (layer.Algorithm as AlgorithmResult).Name == request.LayerName &&
                    (layer.Algorithm as AlgorithmResult).ShowInMakeMeAWorld &&
                    !(layer.Algorithm as AlgorithmResult).DefaultForMakeMeAWorld)
                    return StorageAccess.ToRuntime(layer);
            foreach (var layer in layers)
                if ((layer.Algorithm is AlgorithmResult) &&
                    (layer.Algorithm as AlgorithmResult).ShowInMakeMeAWorld &&
                    (layer.Algorithm as AlgorithmResult).DefaultForMakeMeAWorld)
                {
                    request.LayerName = (layer.Algorithm as AlgorithmResult).Name;
                    return StorageAccess.ToRuntime(layer);
                }
            return null;
        }
        
        public static List<string> GetListOfAvailableLayers(HttpContext context)
        {
            return GetListOfAvailableLayers(context.Server.MapPath("~/bin/WorldConfig.xml"));
        }

        public static string GetDefaultAvailableLayer(HttpContext context)
        {
            return GetDefaultAvailableLayer(context.Server.MapPath("~/bin/WorldConfig.xml"));
        }

        private static List<string> GetListOfAvailableLayers(string path)
        {
            var result = new List<string>();
            
            // Use StorageAccess to load reference to world generation.
            StorageLayer[] layers;
            using (var reader = new StreamReader(path))
                layers = StorageAccess.LoadStorage(reader);
            foreach (var layer in layers)
                if (layer.Algorithm is AlgorithmResult)
                if ((layer.Algorithm as AlgorithmResult).ShowInMakeMeAWorld)
                    result.Add(
                        (layer.Algorithm.Is2DOnly ? "2D," : "3D,") +
                        (layer.Algorithm as AlgorithmResult).Name);
            
            return result;
        }
        
        private static string GetDefaultAvailableLayer(string path)
        {
            // Use StorageAccess to load reference to world generation.
            StorageLayer[] layers;
            using (var reader = new StreamReader(path))
                layers = StorageAccess.LoadStorage(reader);
            foreach (var layer in layers)
                if (layer.Algorithm is AlgorithmResult)
                if ((layer.Algorithm as AlgorithmResult).DefaultForMakeMeAWorld)
                    return (layer.Algorithm.Is2DOnly ? "2D," : "3D,") +
                        (layer.Algorithm as AlgorithmResult).Name;
            
            return null;
        }
        
        #endregion
    }
}

