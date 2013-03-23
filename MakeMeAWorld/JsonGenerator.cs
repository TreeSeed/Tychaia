//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.IO;
using System.Web;
using System.Text;

namespace MakeMeAWorld
{
    public class JsonGenerator : BaseGenerator
    {
        private string GetCacheName(GenerationRequest request, HttpContext context)
        {
            var layer = request.LayerName.Replace("_", "");
            var folder = context.Server.MapPath("~/App_Data/cached_" + layer + "_" + request.Seed);
            var cache = context.Server.MapPath("~/App_Data/cached_" + layer + "_" + request.Seed +
                "/" + request.X + "_" + request.Y + "_" + request.Z +
                "_" + request.Size + ".json");
            try
            {
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
            }
            catch (Exception)
            {
            }
            return cache;
        }
        
        protected override bool ProcessCache(GenerationRequest request, HttpContext context)
        {
            var cache = this.GetCacheName(request, context);
            if (File.Exists(cache))
            {
                context.Response.ContentType = "application/json";
                context.Response.TransmitFile(cache);
                return true;
            }
            return false;
        }
        
        protected override void ProcessGeneration(GenerationResult result, HttpContext context)
        {
            var cache = this.GetCacheName(result.Request, context);
            context.Response.ContentType = "application/json";
            using (var cacheWriter = new StreamWriter(cache))
            {
                using (var webWriter = new StreamWriter(context.Response.OutputStream))
                {
                    cacheWriter.Write("[");
                    webWriter.Write("[");
                    for (var i = 0; i < result.Data.Length; i++)
                    {
                        if (i != 0)
                        {
                            cacheWriter.Write(", ");
                            webWriter.Write(", ");
                        }
                        cacheWriter.Write(i);
                        webWriter.Write(i);
                    }
                    cacheWriter.Write("]");
                    webWriter.Write("]");
                }
            }
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
        }
    }
}

