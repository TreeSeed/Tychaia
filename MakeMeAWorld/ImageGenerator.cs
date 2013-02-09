using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Tychaia.ProceduralGeneration;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Web.UI;
using Tychaia.Globals;

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
            }
            catch (Exception)
            {
            }

            Layer l = CreateLayerFromConfig(context.Server.MapPath("~/bin/WorldConfig.xml"), layer);
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
                }
                catch (Exception)
                {
                }
            }
        }

        #region Data Loading

        private static Layer CreateLayerFromConfig(string path, string name)
        {
            // FIXME: Use StorageAccess to load reference
            // to world generation.
            throw new NotImplementedException();
        }

        public static List<string> GetListOfAvailableLayers(HttpContext context)
        {
            return GetListOfAvailableLayers(context.Server.MapPath("~/bin/WorldConfig.xml"));
        }

        private static List<string> GetListOfAvailableLayers(string path)
        {
            // FIXME: Use StorageAccess to load reference
            // to world generation.
            throw new NotImplementedException();
        }

        #endregion

        #region Rendering

        private static SolidBrush m_UnknownAssociation2D = new SolidBrush(Color.FromArgb(63, 63, 63));

        public static Bitmap RegenerateImageForLayer(Layer l, long x, long y, long z, int width, int height, int depth)
        {
            if (l is Layer2D)
                return RenderPartial3D(l as Layer2D, x, y, z, width, height, 1);
            else if (l is Layer3D)
                return RenderPartial3D(l as Layer3D, x, y, z, width, height, depth);
            else
                return null;
        }

        #region 2D Rendering

        private static Bitmap RenderPartial2D(Layer l, long sx, long sy, int width, int height)
        {
            Bitmap b = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(b);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            Dictionary<int, LayerColor> brushes = l.GetLayerColors();
            int[] data = l.GenerateData(TemporaryCrapBecauseIDidntReallyDesignThingsVeryWell.X + sx, TemporaryCrapBecauseIDidntReallyDesignThingsVeryWell.Y + sy, width, height);
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    if (brushes != null && brushes.ContainsKey(data[x + y * width]))
                        g.FillRectangle(
                            new SolidBrush(Color.FromArgb(
                                brushes[data[x + y * (width)]].A,
                                brushes[data[x + y * (width)]].R,
                                brushes[data[x + y * (width)]].G,
                                brushes[data[x + y * (width)]].B)),
                            new Rectangle(x, y, 1, 1)
                        );
                    else
                        g.FillRectangle(
                            m_UnknownAssociation2D,
                            new Rectangle(x, y, 1, 1)
                        );
                }
            return b;
        }

        #endregion

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

        private static int[] GetCellRenderOrder(int cameraDirection, int width, int height, int depth)
        {
            if (CellRenderOrder[cameraDirection] == null)
                CellRenderOrder[cameraDirection] = CalculateCellRenderOrder(cameraDirection, width, height, depth);
            return CellRenderOrder[cameraDirection];
        }

        #endregion

        private static Bitmap RenderPartial3D(Layer l, long sx, long sy, long sz, int width, int height, int depth)
        {
            Bitmap b = new Bitmap(width * 2, height * 3);
            Graphics g = Graphics.FromImage(b);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            Dictionary<int, LayerColor> brushes = l.GetLayerColors();
            int[] data = l.GenerateData(sx, sy, sz, width, height, depth);
            if (l is Layer3D && data.All(v => v == data[0]))
                return new Bitmap(1, 1);

            int[] render = GetCellRenderOrder(RenderToNE, width, height, depth);
            int ztop = depth;
            int zbottom = 0;
            for (int z = zbottom; z < ztop; z++)
            {
                int rcx = width / 2 - 1 + 16 + 16;
                int rcy = height / 2 - 15 + 32 + 16;
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

                    if (l.IsLayerColorsFlags())
                    {
                        Color accum = Color.FromArgb(0, 0, 0, 0);
                        foreach (KeyValuePair<int, LayerColor> kv in brushes)
                        {
                            SolidBrush sb = new SolidBrush(Color.FromArgb(
                                kv.Value.A,
                                kv.Value.R,
                                kv.Value.G,
                                kv.Value.B));
                            if ((data[x + y * width + z * width * height] & kv.Key) != 0)
                            {
                                accum = Color.FromArgb(
                                    Math.Min(255, accum.A + sb.Color.A),
                                    Math.Min((byte)255, (byte)(accum.R + sb.Color.R * (sb.Color.A / 255.0) / brushes.Count)),
                                    Math.Min((byte)255, (byte)(accum.G + sb.Color.G * (sb.Color.A / 255.0) / brushes.Count)),
                                    Math.Min((byte)255, (byte)(accum.B + sb.Color.B * (sb.Color.A / 255.0) / brushes.Count))
                                );
                            }
                        }
                        if (accum.R == 255 && accum.G == 255 && accum.B == 255)
                            accum = Color.FromArgb(63, 0, 0, 0);
                        g.FillRectangle(
                            new SolidBrush(accum),
                            new Rectangle(rx, ry, rw, rh)
                        );
                    }
                    else
                    {
                        if (brushes != null && brushes.ContainsKey(data[x + y * width + z * width * height]))
                        {
                            LayerColor lc = brushes[data[x + y * width + z * width * height]];
                            // Get neighbours if possible.
                            try
                            {
                                LayerColor n1 = brushes[data[(x + 1) + y * width + z * width * height]];
                                LayerColor n2 = brushes[data[x + (y + 1) * width + z * width * height]];
                                LayerColor n3 = brushes[data[x + y * width + (z + 1) * width * height]];
                                LayerColor n4 = brushes[data[(x + 1) + (y + 1) * width + (z + 1) * width * height]];
                                if (lc.A != 0 && (n1.A != 255 || n2.A != 255 || n3.A != 255 || n4.A != 255 ||
                                    (x + 1) >= width || (y + 1) >= height || (z + 1) >= depth))
                                {
                                    b.SetPixel(rx, ry, Color.FromArgb(lc.A, lc.R, lc.G, lc.B));
                                    b.SetPixel(rx + 1, ry, Color.FromArgb(lc.A, lc.R, lc.G, lc.B));
                                }
                            }
                            catch (Exception)
                            {
                                b.SetPixel(rx, ry, Color.FromArgb(lc.A, lc.R, lc.G, lc.B));
                                b.SetPixel(rx + 1, ry, Color.FromArgb(lc.A, lc.R, lc.G, lc.B));
                            }
                            /*
                            SolidBrush sb = new SolidBrush(Color.FromArgb(lc.A, lc.R, lc.G, lc.B));
                            //sb.Color = Color.FromArgb(255, sb.Color);
                            g.FillRectangle(
                                sb,
                                new Rectangle(rx, ry, rw, rh)
                            );*/
                        }
                    }
                }
            }

            return b;
        }

        #endregion

        #endregion
    }
}