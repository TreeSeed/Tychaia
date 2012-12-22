using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Tychaia.ProceduralGeneration;
using System.Threading.Tasks;
using System.Threading;

namespace TychaiaWorldGenViewer.Flow
{
    public static class LayerFlowImageGeneration
    {
        private static SolidBrush m_UnknownAssociation = new SolidBrush(Color.FromArgb(255, 0, 0));

        public static int X
        {
            get;
            set;
        }

        public static int Y
        {
            get;
            set;
        }

        public static int Z
        {
            get;
            set;
        }

        public static Bitmap RegenerateImageForLayer(Layer l, int width, int height)
        {
            if (l is Layer2D)
                return Regenerate2DImageForLayer(l as Layer2D, width, height);
            else if (l is Layer3D)
                return Regenerate3DImageForLayer(l as Layer3D, width, height);
            else
                return null;
        }

        #region 3D Rendering

        #region Cell Render Ordering

        private static int[][] CellRenderOrder = new int[4][] { null, null, null, null };
        private const int RenderToNE = 0;
        private const int RenderToNW = 1;
        private const int RenderToSE = 2;
        private const int RenderToSW = 3;
        private const int RenderWidth = 64;
        private const int RenderHeight = 64;
        private const int RenderDepth = 64;

        private static int[] CalculateCellRenderOrder(int targetDir)
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

            int[] result = new int[RenderWidth * RenderHeight];
            int count = 0;
            int start = 0;
            int maxx = RenderWidth - 1;
            int maxy = RenderHeight - 1;
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
                    result[count++] = y-- * RenderWidth + x++;

                // Attack from the right.
                if (atk < maxx)
                { x = atk; y = 0; }
                else
                { x = maxx; y = atk - maxx; }
                while (y <= atk / 2)
                    result[count++] = y++ * RenderWidth + x--;
            }

            return result;
        }

        private static int[] GetCellRenderOrder(int cameraDirection)
        {
            if (CellRenderOrder[cameraDirection] == null)
                CellRenderOrder[cameraDirection] = CalculateCellRenderOrder(cameraDirection);
            return CellRenderOrder[cameraDirection];
        }

        #endregion

        private static Bitmap Regenerate3DImageForLayer(Layer3D l, int width, int height)
        {
            Bitmap b = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(b);
            g.Clear(Color.White);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            Dictionary<int, LayerColor> colors = l.GetLayerColors();
            int[] data = l.GenerateData(LayerFlowImageGeneration.X, LayerFlowImageGeneration.Y, LayerFlowImageGeneration.Z, RenderWidth, RenderHeight, RenderDepth);

            /* Our world is laid out in memory in terms of X / Y, but
             * we are rendering isometric, which means that the rendering
             * order for tiles must be like so:
             * 
             *               North
             *        1  3  5  9  13 19 25
             *        2  6  10 14 20 26 32
             *        4  8  15 21 27 33 37
             *  East  7  12 18 28 34 38 42  West
             *        11 17 24 31 39 43 45
             *        16 23 30 36 41 46 48
             *        22 29 35 40 44 47 49
             *               South
             *  
             * We also need to account for situations where the user rotates
             * the isometric view.
             */

            /*
             *                      North
             *         0    0.5  1     1.5  2    2.5  3
             *        -0.5  0    0.5   1    1.5  2    2.5
             *        -1   -0.5  0     0.5  1    1.5  2
             *  East  -1.5 -1   -0.5   0    0.5  1    1.5  West
             *        -2   -1.5 -1    -0.5  0    0.5  1
             *        -2.5 -2   -1.5  -1   -0.5  0    0.5
             *        -3   -2.5 -2    -1.5 -1   -0.5  0
             *                      South
             *                      
             *  v = (x - y) / 2.0
             */

            int[] render = GetCellRenderOrder(RenderToNE);
            int ztop = RenderDepth;
            int zbottom = 0;
            for (int z = zbottom; z < ztop; z++)
            {
                int rcx = width / 2 - 1;
                int rcy = height / 2 - 31;
                int rw = 2;
                int rh = 1;
                for (int i = 0; i < render.Length; i++)
                {
                    // Calculate the X / Y of the tile in the grid.
                    int x = render[i] % RenderWidth;
                    int y = render[i] / RenderWidth;

                    // Calculate the render position on screen.
                    int rx = rcx + (int)((x - y) / 2.0 * rw);// (int)(x / ((RenderWidth + 1) / 2.0) * rw);
                    int ry = rcy + (x + y) * rh - (rh / 2 * (RenderWidth + RenderHeight)) - (z - zbottom) * 1;
                    
                    while (true)
                    {
                        try
                        {
                            if (l.IsLayerColorsFlags())
                            {
                                Color accum = Color.FromArgb(0, 0, 0, 0);
                                foreach (KeyValuePair<int, LayerColor> kv in colors)
                                {
                                    LayerColor lc = kv.Value;
                                    SolidBrush sb = new SolidBrush(Color.FromArgb(lc.A, lc.R, lc.G, lc.B));
                                    if ((data[x + y * RenderWidth + z * RenderWidth * RenderHeight] & kv.Key) != 0)
                                    {
                                        accum = Color.FromArgb(
                                            Math.Min(255, accum.A + sb.Color.A),
                                            Math.Min((byte)255, (byte)(accum.R + sb.Color.R * (sb.Color.A / 255.0) / colors.Count)),
                                            Math.Min((byte)255, (byte)(accum.G + sb.Color.G * (sb.Color.A / 255.0) / colors.Count)),
                                            Math.Min((byte)255, (byte)(accum.B + sb.Color.B * (sb.Color.A / 255.0) / colors.Count))
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
                                if (colors != null && colors.ContainsKey(data[x + y * RenderWidth + z * RenderWidth * RenderHeight]))
                                {
                                    LayerColor lc = colors[data[x + y * RenderWidth + z * RenderWidth * RenderHeight]];
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

            return b;
        }

        #endregion

        #region 2D Rendering

        private static Bitmap Regenerate2DImageForLayer(Layer2D l, int width, int height)
        {
            Bitmap b = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(b);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            Dictionary<int, LayerColor> colors = l.GetLayerColors();
            int[] data = l.GenerateData(LayerFlowImageGeneration.X, LayerFlowImageGeneration.Y, width, height);
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    while (true)
                    {
                        try
                        {
                            if (colors != null && colors.ContainsKey(data[x + y * width]))
                            {
                                LayerColor lc = colors[data[x + y * (width)]];
                                SolidBrush sb = new SolidBrush(Color.FromArgb(lc.A, lc.R, lc.G, lc.B));
                                g.FillRectangle(
                                    sb,
                                    new Rectangle(x, y, 1, 1)
                                    );
                            }
                            else
                                g.FillRectangle(
                                    m_UnknownAssociation,
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
            return b;
        }

        #endregion
    }
}
