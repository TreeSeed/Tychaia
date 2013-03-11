using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Tychaia.Globals;
using Tychaia.ProceduralGeneration;

namespace MakeMeAWorld
{
    public class ImageGenerator : BaseHandler, IHttpHandler
    {
        public override void ProcessRequest(HttpContext context)
        {
            long x = Convert.ToInt64(context.Request.QueryString["x"]);
            long y = Convert.ToInt64(context.Request.QueryString["y"]);
            long z = Convert.ToInt64(context.Request.QueryString["z"]);
            int size = Convert.ToInt32(context.Request.QueryString["size"]);
            long seed = Convert.ToInt64(context.Request.QueryString["seed"]);
            string layer = context.Request.QueryString["layer"];
            size = 64;
            if (string.IsNullOrEmpty(layer))
                layer = "Game World";
            layer = layer.Replace("_", "");

            string cache = null;
            try
            {
                string folder = context.Server.MapPath("~/App_Data/cached_" + layer + "_" + seed);
                cache = context.Server.MapPath("~/App_Data/cached_" + layer + "_" + seed + "/" + x + "_" + y + "_" + z + "_" + size + ".png");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                if (File.Exists(cache))
                {
                    context.Response.ContentType = "image/png";
                    context.Response.TransmitFile(cache);
                    return;
                }
            } catch (Exception)
            {
            }

            var l = CreateLayerFromConfig(context.Server.MapPath("~/bin/WorldConfig.xml"), layer);
            if (l == null)
                throw new HttpException(404, "The layer name was invalid");
            l.Seed = seed;
            Bitmap b = RegenerateImageForLayer(l, x, y, z, size, size, size);
            context.Response.ContentType = "image/png";
            b.Save(context.Response.OutputStream, ImageFormat.Png);
            if (cache != null)
            {
                try
                {
                    using (var stream = new FileStream(cache, FileMode.CreateNew))
                        b.Save(stream, ImageFormat.Png);
                } catch (Exception)
                {
                }
            }
        }

        #region Data Loading

        private static RuntimeLayer CreateLayerFromConfig(string path, string name)
        {
            // Use StorageAccess to load reference to world generation.
            StorageLayer[] layers;
            using (var reader = new StreamReader(path))
                layers = StorageAccess.LoadStorage(reader);
            foreach (var layer in layers)
                if (layer.Algorithm is AlgorithmResult)
                if ((layer.Algorithm as AlgorithmResult).Name == name)
                    return StorageAccess.ToRuntime(layer);
            return null;
        }

        public static List<string> GetListOfAvailableLayers(HttpContext context)
        {
            return GetListOfAvailableLayers(context.Server.MapPath("~/bin/WorldConfig.xml"));
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
                    result.Add((layer.Algorithm as AlgorithmResult).Name);

            return result;
        }

        #endregion

        #region Rendering

        private static SolidBrush m_UnknownAssociation2D = new SolidBrush(Color.FromArgb(63, 63, 63));

        public static Bitmap RegenerateImageForLayer(RuntimeLayer l, long x, long y, long z, int width, int height, int depth)
        {
            return RenderPartial3D(l, x, y, z, width, height, depth);
        }
        
        
        #region 3D Rendering
        
        #region Cell Render Ordering
        
        private static int[][] CellRenderOrder = new int[4][]
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
        
        private static int[] CalculateCellRenderOrder(int targetDir, int width, int height, int depth)
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
            
            int[] result = new int[width * height];
            int count = 0;
            int start = 0;
            int maxx = width - 1;
            int maxy = height - 1;
            int last = maxx + maxy;
            int x, y;
            
            for (int atk = start; atk <= last; atk++)
            {
                // Attack from the left.
                if (atk < maxy)
                {
                    x = 0;
                    y = atk;
                } else
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
                } else
                {
                    x = maxx;
                    y = atk - maxx;
                }
                while (y <= atk / 2)
                    result[count++] = y++ * width + x--;
            }
            
            return result;
        }
        
        private static int[] GetCellRenderOrder(int cameraDirection, int width, int height, int depth)
        {
            if (CellRenderOrder[cameraDirection] == null)
                CellRenderOrder[cameraDirection] = CalculateCellRenderOrder(cameraDirection, width, height, depth);
            return CellRenderOrder[cameraDirection];
        }
        
        #endregion
        
        private static Bitmap RenderPartial3D(RuntimeLayer layer, long sx, long sy, long sz, int width, int height, int depth)
        {
            Bitmap bitmap = new Bitmap(width * 2, height * 3);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            int[] data = null;
            try
            {
                int computations = 0;
                data = layer.GenerateData(sx, sy, sz, width, height, depth, out computations);
                
                int[] render = GetCellRenderOrder(RenderToNE, width, height, depth);
                int ztop = depth;
                int zbottom = 0;
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
                        int rx = rcx + (int)((x - y) / 2.0 * rw);// (int)(x / ((RenderWidth + 1) / 2.0) * rw);
                        int ry = rcy + (x + y) * rh - (rh / 2 * (width + height)) - (z - zbottom) * 1;
                        
                        while (true)
                        {
                            try
                            {
                                Color lc = layer.Algorithm.GetColorForValue(
                                    StorageAccess.FromRuntime(layer.GetInputs()[0]),
                                    data[x + y * width + z * width * height]);
                                var sb = new SolidBrush(Color.FromArgb(lc.A, lc.R, lc.G, lc.B));
                                graphics.FillRectangle(
                                    sb,
                                    new Rectangle(rx, ry, rw, rh)
                                );
                                break;
                            } catch (InvalidOperationException)
                            {
                                // Graphics can be in use elsewhere, but we don't care; just try again.
                            }
                        }
                    }
                }
            } catch (Exception)
            {
            }
            
            return bitmap;
        }
        
        #endregion

        #endregion
    }
}