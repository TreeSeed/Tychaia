// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Drawing;
using System.Drawing.Text;

namespace Tychaia.ProceduralGeneration.Flow
{
    public class AlgorithmFlowImageGeneration : IAlgorithmFlowImageGeneration
    {
        private const int RenderWidth = 64;
        private const int RenderHeight = 64;
        private const int RenderDepth = 64;

        private readonly IStorageAccess m_StorageAccess;
        private readonly IIsometricBitmapRenderer m_IsometricBitmapRenderer;

        public AlgorithmFlowImageGeneration(
            IStorageAccess storageAccess,
            IIsometricBitmapRenderer isometricBitmapRenderer)
        {
            this.m_StorageAccess = storageAccess;
            this.m_IsometricBitmapRenderer = isometricBitmapRenderer;
        }

        public Bitmap RegenerateImageForLayer(
            StorageLayer layer,
            long seed,
            long ox, long oy, long oz,
            int width, int height, int depth,
            bool compiled = false)
        {
            try
            {
                var runtime = this.m_StorageAccess.ToRuntime(layer);
                runtime.SetSeed(seed);
                if (compiled)
                {
                    try
                    {
                        return this.m_IsometricBitmapRenderer.GenerateImage(
                            runtime,
                            x => runtime.Algorithm.GetColorForValue(this.m_StorageAccess.FromRuntime(runtime), x),
                            ox, oy, oz,
                            width, height, runtime.Algorithm.Is2DOnly ? 1 : depth);
                        /*return Regenerate3DImageForLayer(
                            runtime,
                            ox, oy, oz,
                            width, height, depth,
                            this.m_StorageAccess.ToCompiled(runtime));*/
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                return this.m_IsometricBitmapRenderer.GenerateImage(
                    runtime,
                    x => runtime.Algorithm.GetColorForValue(this.m_StorageAccess.FromRuntime(runtime), x),
                    ox, oy, oz,
                    width, height, runtime.Algorithm.Is2DOnly ? 1 : depth);
                //return Regenerate3DImageForLayer(runtime, ox, oy, oz, width, height, depth);
            }
            catch (Exception)
            {
                return null;
            }
        }

/*
        private Bitmap Regenerate3DImageForLayer(
            RuntimeLayer runtimeLayer,
            long ox, long oy, long oz,
            int width, int height, int depth,
            IGenerator compiledLayer = null)
        {
            var owidth = width;
            var oheight = height;
            width = 128;
            height = 192; // this affects bitmaps and rendering and stuff :(
            depth = 128;

            // ARGHGHG FIXME
            var b = new Bitmap(width, height);
            var g = Graphics.FromImage(b);
            g.Clear(Color.White);
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            var computations = 0;
            dynamic data;
            if (compiledLayer != null)
                data = compiledLayer.GenerateData(ox, oy, oz, RenderWidth, RenderHeight, RenderDepth, out computations);
            else
                data = runtimeLayer.GenerateData(ox, oy, oz, RenderWidth, RenderHeight, RenderDepth, out computations);

            var storageLayer = this.m_StorageAccess.FromRuntime(runtimeLayer);
            int[] render = this.m_CellOrderCalculator.CalculateCellRenderOrder(RenderWidth, RenderHeight);
            int ztop = runtimeLayer.Algorithm.Is2DOnly ? 1 : RenderDepth;
            var zbottom = 0;
            for (var z = zbottom; z < ztop; z++)
            {
                var rcx = width / 2 - 1;
                var rcy = height / 2 - 31;
                var rw = 2;
                var rh = 1;
                for (var i = 0; i < render.Length; i++)
                {
                    // Calculate the X / Y of the tile in the grid.
                    var x = render[i] % RenderWidth;
                    var y = render[i] / RenderWidth;

                    // Calculate the render position on screen.
                    var rx = rcx + (int) ((x - y) / 2.0 * rw); // (int)(x / ((RenderWidth + 1) / 2.0) * rw);
                    var ry = rcy + (x + y) * rh - (rh / 2 * (RenderWidth + RenderHeight)) - (z - zbottom) * 1;

                    while (true)
                    {
                        try
                        {
                            Color lc = runtimeLayer.Algorithm.GetColorForValue(
                                storageLayer,
                                data[x + y * owidth + z * owidth * oheight]);
                            var sb = new SolidBrush(Color.FromArgb(lc.A, lc.R, lc.G, lc.B));
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
        }*/
    }
}
