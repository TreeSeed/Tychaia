using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Tychaia.ProceduralGeneration;
using TychaiaWorldGenViewer.Flow;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Web.UI;

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
            //if (size <= 0 || size > 1000)
            size = 64;

            string cache = null;
            try
            {
                string folder = context.Server.MapPath("~/App_Data/cached_" + seed);
                cache = context.Server.MapPath("~/App_Data/cached_" + seed + "/" + x + "_" + y + "_" + z + "_" + size + ".png");
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

            Layer l = CreateLayerFromConfig(context.Server.MapPath("~/bin/WorldConfig.xml"));
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

        private static Layer CreateLayerFromConfig(string path)
        {
            // Dynamically generate a list of serializable types for the
            // data contract.
            List<Type> types = new List<Type> {
                // Flow system classes
                typeof(FlowConnector),
                typeof(FlowElement),
                typeof(LayerFlowConnector),
                typeof(LayerFlowElement),
            };
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type t in a.GetTypes())
                    if (typeof(Layer).IsAssignableFrom(t))
                        types.Add(t);
            Type[] serializableTypes = types.ToArray();

            // Load configuration.
            DataContractSerializer x = new DataContractSerializer(typeof(FlowInterfaceControl.ListFlowElement), serializableTypes);
            FlowInterfaceControl.ListFlowElement config = null;
            using (FileStream fstream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fstream, new XmlDictionaryReaderQuotas() { MaxDepth = 1000 }))
                config = x.ReadObject(reader, true) as FlowInterfaceControl.ListFlowElement;

            // Find the result layer.
            foreach (FlowElement fe in config)
            {
                if (fe is LayerFlowElement)
                    if ((fe as LayerFlowElement).Layer is Layer3DStoreResult)
                        return (fe as LayerFlowElement).Layer;
            }
            return null;
        }

        #endregion

        #region Rendering

        private static SolidBrush m_UnknownAssociation2D = new SolidBrush(Color.FromArgb(63, 63, 63));
        private static SolidBrush m_UnknownAssociation3D = new SolidBrush(Color.FromArgb(0, 0, 0, 0));

        public static Bitmap RegenerateImageForLayer(Layer l, long x, long y, long z, int width, int height, int depth)
        {
            if (l is Layer2D)
                return RenderPartial2D(l as Layer2D, x, y, width, height);
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
            int[] data = null;
            try
            {
                data = l.GenerateData(LayerFlowImageGeneration.X + sx, LayerFlowImageGeneration.Y + sy, width, height);
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                    {
                        while (true)
                        {
                            try
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
                                break;
                            }
                            catch (InvalidOperationException)
                            {
                                // Graphics can be in use elsewhere, but we don't care; just try again.
                            }
                        }
                    }
            }
            catch (Exception)
            {
            }
            return b;
        }

        #endregion

        #region 3D Rendering

        #region Cell Render Ordering

        private static int[][] CellRenderOrder = new int[4][] { null, null, null, null };
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
                { x = 0; y = atk; }
                else
                { x = atk - maxy; y = maxy; }
                while (y > atk / 2)
                    result[count++] = y-- * width + x++;

                // Attack from the right.
                if (atk < maxx)
                { x = atk; y = 0; }
                else
                { x = maxx; y = atk - maxx; }
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
            int[] data = null;
            //try
            //{
                data = l.GenerateData(sx, sy, sz, width, height, depth);
                if (data.All(v => v == data[0]))
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

                        while (true)
                        {
                            try
                            {
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
                                    break;
                                }
                                else
                                {
                                    if (brushes != null && brushes.ContainsKey(data[x + y * width + z * width * height]))
                                    {
                                        LayerColor lc = brushes[data[x + y * width + z * width * height]];
                                        SolidBrush sb = new SolidBrush(Color.FromArgb(lc.A, lc.R, lc.G, lc.B));
                                        //sb.Color = Color.FromArgb(255, sb.Color);
                                        g.FillRectangle(
                                            sb,
                                            new Rectangle(rx, ry, rw, rh)
                                            );
                                    }
                                    break;
                                }
                            }
                            catch (InvalidOperationException)
                            {
                                // Graphics can be in use elsewhere, but we don't care; just try again.
                            }
                        }
                    }
                }
            //}
            //catch (Exception)
           // {
            //}

            return b;
        }

        #endregion

        #endregion
    }
}
