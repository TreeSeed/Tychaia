//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using Tychaia.ProceduralGeneration;
using System.Linq;

namespace MakeMeAWorld
{
    public class ImageGenerator : BaseGenerator
    {
        private string GetCacheName(GenerationRequest request, HttpContext context)
        {
            var layer = request.LayerName.Replace("_", "");
            var folder = context.Server.MapPath("~/App_Data/cached_" + layer + "_" + request.Seed);
            var cache = context.Server.MapPath("~/App_Data/cached_" + layer + "_" + request.Seed +
                "/" + request.X + "_" + request.Y + "_" + request.Z +
                "_" + request.Size + ".png");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return cache;
        }

        protected override bool ProcessCache(GenerationRequest request, HttpContext context)
        {
            var cache = this.GetCacheName(request, context);
            if (File.Exists(cache))
            {
                context.Response.ContentType = "image/png";
                context.Response.TransmitFile(cache);
                return true;
            }
            return false;
        }

        protected override void ProcessGeneration(GenerationResult result, HttpContext context)
        {
            var bitmap = RenderPartial3D(result);
            try
            {
                context.Response.ContentType = "image/png";
                bitmap.Save(context.Response.OutputStream, ImageFormat.Png);
                var cache = this.GetCacheName(result.Request, context);
                if (cache != null)
                {
                    try
                    {
                        using (var stream = new FileStream(cache, FileMode.CreateNew))
                            bitmap.Save(stream, ImageFormat.Png);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            finally
            {
                bitmap.Dispose();
            }
        }

        #region Rendering
        
        #region 3D Rendering
        
        #region Cell Render Ordering
        
        private static int[][] m_CellRenderOrder = new int[4][]
        {
            null,
            null,
            null,
            null
        };
        private const int RenderToNE = 0;
        private const int RenderToNW = 1;
        private const int RenderToSE = 2;
        private const int RenderToSW = 3;
        
        private static int[] CalculateCellRenderOrder(int targetDir, int width, int height)
        {
            /*               North
             *        0  1  2  3  4  5  6 
             *        1  2  3  4  5  6  7 
             *        2  3  4  5  6  7  8
             *  East  3  4  5  6  7  8  9  West
             *        4  5  6  7  8  9  10
             *        5  6  7  8  9  10 11
             *        6  7  8  9  10 11 12
             *               South
             *  
             * Start value is always 0.
             * Last value is (MaxX + MaxY).
             * This is the AtkValue.
             * 
             * We attack from the left side of the render first
             * with (X: 0, Y: AtkValue) until Y would be less than
             * half of AtkValue.
             * 
             * We then attack from the right side of the render
             * with (X: AtkValue, Y: 0) until X would be less than
             * half of AtkValue - 1.
             * 
             * If we are attacking from the left, but Y is now
             * greater than MaxY, then we are over half-way and are
             * now starting at the bottom of the grid.
             * 
             * In this case, we start with (X: AtkValue - MaxY, Y: MaxY)
             * and continue until we reach the same conditions that
             * apply normally.  The same method applies to the right hand
             * side where we start with (X: MaxX, Y: AtkValue - MaxX).
             *
             */
            
            if (targetDir != RenderToNE)
                throw new InvalidOperationException();
            
            var result = new int[width * height];
            var count = 0;
            var start = 0;
            var maxx = width - 1;
            var maxy = height - 1;
            var last = maxx + maxy;
            int x, y;
            
            for (int atk = start; atk <= last; atk++)
            {
                // Attack from the left.
                if (atk < maxy)
                {
                    x = 0;
                    y = atk;
                }
                else
                {
                    x = atk - maxy;
                    y = maxy;
                }
                while (y > atk / 2)
                    result[count++] = y-- * width + x++;
                
                // Attack from the right.
                if (atk < maxx)
                {
                    x = atk;
                    y = 0;
                }
                else
                {
                    x = maxx;
                    y = atk - maxx;
                }
                while (y <= atk / 2)
                    result[count++] = y++ * width + x--;
            }
            
            return result;
        }
        
        private static int[] GetCellRenderOrder(int cameraDirection, int width, int height)
        {
            if (m_CellRenderOrder[cameraDirection] == null)
                m_CellRenderOrder[cameraDirection] = CalculateCellRenderOrder(cameraDirection, width, height);
            return m_CellRenderOrder[cameraDirection];
        }
        
        #endregion
        
        private static Bitmap RenderPartial3D(GenerationResult result)
        {
            var width = result.Request.Size;
            var height = result.Request.Size;
            var depth = result.Request.Size;
            var bitmap = new Bitmap(width * 2, height * 3);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                try
                {
                    var render = GetCellRenderOrder(RenderToNE, width, height);
                    var ztop = depth;
                    var zbottom = 0;
                    var parentLayer = StorageAccess.FromRuntime(result.Layer.GetInputs()[0]);
                    for (int z = zbottom; z < ztop; z++)
                    {
                        int rcx = width / 2 - 1 + 16;
                        int rcy = height / 2 - 15 + 32;
                        int rw = 2;
                        int rh = 1;
                        for (int i = 0; i < render.Length; i++)
                        {
                            // Calculate the X / Y of the tile in the grid.
                            int x = render[i] % width;
                            int y = render[i] / width;

                            // Calculate the render position on screen.
                            int rx = rcx + (int)((x - y) / 2.0 * rw);
                            int ry = rcy + (x + y) * rh - (rh / 2 * (width + height)) - (z - zbottom) * 1;
                            while (true)
                            {
                                try
                                {
                                    Color lc = result.Layer.Algorithm.GetColorForValue(
                                        parentLayer,
                                        result.Data[x + y * width + z * width * height]);
                                    var sb = new SolidBrush(Color.FromArgb(lc.A, lc.R, lc.G, lc.B));
                                    graphics.FillRectangle(sb, new Rectangle(rx, ry, rw, rh));
                                    break;
                                }
                                catch (InvalidOperationException)
                                {
                                    // Graphics can be in use elsewhere, but we don't care; just try again.
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return bitmap;
        }
        
        #endregion

        #endregion
    }
}