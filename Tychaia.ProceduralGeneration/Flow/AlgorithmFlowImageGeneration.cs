//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Drawing;
using Tychaia.ProceduralGeneration;

namespace Tychaia.ProceduralGeneration.Flow
{
    public static class AlgorithmFlowImageGeneration
    {
        public static Bitmap RegenerateImageForLayer(
            StorageLayer layer,
            long seed,
            long ox, long oy, long oz,
            int width, int height, int depth,
            bool compiled = false)
        {
            try
            {
                var runtime = StorageAccess.ToRuntime(layer);
                runtime.SetSeed(seed);
                if (compiled)
                {
                    try
                    {
                        return Regenerate3DImageForLayer(
                            runtime,
                            ox, oy, oz,
                            width, height, depth,
                            StorageAccess.ToCompiled(runtime));
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                return Regenerate3DImageForLayer(runtime, ox, oy, oz, width, height, depth);
            }
            catch (Exception)
            {
                return null;
            }
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

        private const int RenderWidth = 64;
        private const int RenderHeight = 64;
        private const int RenderDepth = 64;

        private static Bitmap Regenerate3DImageForLayer(RuntimeLayer runtimeLayer, long ox, long oy, long oz, int width, int height, int depth, IGenerator compiledLayer = null)
        {
            int owidth = width;
            int oheight = height;
            width = 128;
            height = 192; // this affects bitmaps and rendering and stuff :(
            depth = 128;

            // ARGHGHG FIXME
            Bitmap b = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(b);
            g.Clear(Color.White);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            int computations = 0;
            dynamic data;
            if (compiledLayer != null)
                data = compiledLayer.GenerateData(ox, oy, oz, RenderWidth, RenderHeight, RenderDepth, out computations);
            else
                data = runtimeLayer.GenerateData(ox, oy, oz, RenderWidth, RenderHeight, RenderDepth, out computations);

            var storageLayer = StorageAccess.FromRuntime(runtimeLayer);
            int[] render = GetCellRenderOrder(RenderToNE, RenderWidth, RenderHeight);
            int ztop = runtimeLayer.Algorithm.Is2DOnly ? 1 : RenderDepth;
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
                            Color lc = runtimeLayer.Algorithm.GetColorForValue(
                                storageLayer,
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
    }
}
