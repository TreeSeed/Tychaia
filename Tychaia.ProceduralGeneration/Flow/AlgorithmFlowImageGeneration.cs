using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Tychaia.ProceduralGeneration;
using System.Threading.Tasks;
using System.Threading;

namespace Tychaia.ProceduralGeneration.Flow
{
    public static class AlgorithmFlowImageGeneration
    {
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

        public static Bitmap RegenerateImageForLayer(StorageLayer layer, long ox, long oy, long oz, int width, int height, int depth)
        {
            var runtime = StorageAccess.ToRuntime(layer);
            return Regenerate3DImageForLayer(runtime, ox, oy, oz, width, height, depth);
        }

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

        private static int[] GetCellRenderOrder(int cameraDirection, int width, int height)
        {
            if (CellRenderOrder[cameraDirection] == null)
                CellRenderOrder[cameraDirection] = CalculateCellRenderOrder(cameraDirection, width, height);
            return CellRenderOrder[cameraDirection];
        }

        #endregion

        #if BROKEN

        private static Bitmap Regenerate3DImageForLayer(RuntimeLayer layer, long ox, long oy, long oz, int width, int height, int depth)
        {
            var bitmap = new Bitmap(width, height);
            var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            var data = layer.GenerateData(ox, oy, oz, width, height, depth);

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
            
            StorageLayer parent;
            if (layer.GetInputs().Length == 0)
                parent = null;
            else
                parent = StorageAccess.FromRuntime(layer.GetInputs()[0]);

            int[] render = GetCellRenderOrder(RenderToNE, width, height);
            int ztop = depth;
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
                                parent,
                                data[x + y * width + z * width * height]);
                            var sb = new SolidBrush(Color.FromArgb(lc.A, lc.R, lc.G, lc.B));
                            graphics.FillRectangle(
                                sb,
                                new Rectangle(rx, ry, rw, rh)
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

            return bitmap;
        }

#else
        
        private const int RenderWidth = 64;
        private const int RenderHeight = 64;
        private const int RenderDepth = 64;
        
        private static Bitmap Regenerate3DImageForLayer(RuntimeLayer l, long ox, long oy, long oz, int width, int height, int depth)
        {
            int owidth = width;
            int oheight = height;
            //int odepth = depth;
            width = 128;
            height = 192; // this affects bitmaps and rendering and stuff :(
            depth = 128;

            // ARGHGHG FIXME
            Bitmap b = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(b);
            g.Clear(Color.White);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            dynamic data = l.GenerateData(ox, oy, oz, RenderWidth, RenderHeight, RenderDepth);
            
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
            
            StorageLayer parent;
            if (l.GetInputs().Length == 0)
                parent = null;
            else
                parent = StorageAccess.FromRuntime(l.GetInputs()[0]);

            int[] render = GetCellRenderOrder(RenderToNE, RenderWidth, RenderHeight);
            int ztop = l.Algorithm.Is2DOnly ? 1 : RenderDepth;
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
                            Color lc = l.Algorithm.GetColorForValue(
                                parent,
                                data[x + y * owidth + z * owidth * oheight]);
                            SolidBrush sb = new SolidBrush(Color.FromArgb(lc.A, lc.R, lc.G, lc.B));
                            g.FillRectangle(
                                sb,
                                new Rectangle(rx, ry, rw, rh)
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
            
            return b;
        }

        #endif
    }
}
