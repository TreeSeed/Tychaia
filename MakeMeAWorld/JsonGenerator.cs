// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web;
using Tychaia.ProceduralGeneration;

namespace MakeMeAWorld
{
    public class JsonGenerator : BaseGenerator
    {
        protected override bool ProcessCache(GenerationRequest request, HttpContext context)
        {
            var cache = this.GetCacheName(request, context, "json");
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
            var cache = this.GetCacheName(result.Request, context, "json");
            context.Response.ContentType = "application/json";
            using (var cacheWriter = new StreamWriter(cache))
            {
                using (var webWriter = new StreamWriter(context.Response.OutputStream))
                {
                    if (result.Request.Packed)
                    {
                        cacheWriter.Write("{\"empty\":false,\"time\":\"" + result.TotalTime + "\",\"packed\":true,\"data\":[");
                        webWriter.Write("{\"empty\":false,\"time\":\"" + result.TotalTime + "\",\"packed\":true,\"data\":[");
                        {
                            var continuousValue = 0;
                            var continuousCount = 1;
                            var first = true;
                            var i = 0;
                            do
                            {
                                // Store the value into our continuity tracker.
                                if (i != result.Data.Length)
                                    continuousValue = result.Data[i];

                                // Increment to the next position.
                                i++;

                                if (i == result.Data.Length ||
                                    continuousValue != result.Data[i])
                                {
                                    // Output in the most efficient manner.
                                    if (("[" + continuousCount + "," + continuousValue + "]").Length >
                                        ((continuousValue.ToString().Length + 1) * continuousCount) - 1)
                                    {
                                        // Single value.
                                        for (var a = 0; a < continuousCount; a++)
                                        {
                                            if (!first)
                                            {
                                                cacheWriter.Write(",");
                                                webWriter.Write(",");
                                            }
                                            
                                            first = false;

                                            cacheWriter.Write(continuousValue);
                                            webWriter.Write(continuousValue);
                                        }
                                    }
                                    else
                                    {
                                        if (!first)
                                        {
                                            cacheWriter.Write(",");
                                            webWriter.Write(",");
                                        }
                                        
                                        first = false;

                                        // Multiple copies of the same
                                        // value in a row.
                                        cacheWriter.Write("[" + continuousCount + "," + continuousValue + "]");
                                        webWriter.Write("[" + continuousCount + "," + continuousValue + "]");
                                    }

                                    // Reset the continity count.
                                    continuousCount = 1;
                                }
                                else
                                    continuousCount++;
                            }
                            while (i < result.Data.Length);
                        }
                    }
                    else
                    {
                        cacheWriter.Write("{\"empty\":false,\"time\":\"" + result.TotalTime + "\",\"packed\":false,\"data\":[");
                        webWriter.Write("{\"empty\":false,\"time\":\"" + result.TotalTime + "\",\"packed\":false,\"data\":[");
                        {
                            var first = true;
                            for (var i = 0; i < result.Data.Length; i++)
                            {
                                if (!first)
                                {
                                    cacheWriter.Write(",");
                                    webWriter.Write(",");
                                }
                                
                                first = false;
                                cacheWriter.Write(result.Data[i]);
                                webWriter.Write(result.Data[i]);
                            }
                        }
                    }
                    
                    cacheWriter.Write("],\"mappings\":{");
                    webWriter.Write("],\"mappings\":{");
                    var mappings = new Dictionary<int, Color>();
                    var parentLayer = StorageAccess.FromRuntime(result.Layer.GetInputs()[0]);
                    
                    for (var i = 0; i < result.Data.Length; i++)
                    {
                        if (!mappings.ContainsKey(result.Data[i]))
                            mappings.Add(
                                result.Data[i],
                                result.Layer.Algorithm.GetColorForValue(parentLayer, result.Data[i]));
                    }
                    
                    {
                        var first = true;
                        foreach (var kv in mappings)
                        {
                            if (!first)
                            {
                                cacheWriter.Write(",");
                                webWriter.Write(",");
                            }
                            
                            first = false;

                            var colorString =
                            "[" + kv.Value.A +
                                "," + kv.Value.R +
                                "," + kv.Value.G +
                                "," + kv.Value.B + "]";
                            cacheWriter.Write("\"" + kv.Key + "\":" + colorString);
                            webWriter.Write("\"" + kv.Key + "\":" + colorString);
                        }
                    }
                    
                    cacheWriter.Write("}}");
                    webWriter.Write("}}");
                }
            }
        }

        protected override void ProcessEmpty(GenerationResult result, HttpContext context)
        {
            var cache = this.GetCacheName(result.Request, context, "json");
            context.Response.ContentType = "application/json";
            using (var cacheWriter = new StreamWriter(cache))
            {
                using (var webWriter = new StreamWriter(context.Response.OutputStream))
                {
                    cacheWriter.Write("{\"empty\":true}");
                    webWriter.Write("{\"empty\":true}");
                }
            }
        }
    }
}
