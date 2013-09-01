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
    public class AlgorithmTraceImageGeneration : IAlgorithmTraceImageGeneration
    {
        private const int TraceScale = 3;
        private const int TraceRenderWidth = 64 * TraceScale;
        private const int TraceRenderHeight = 64 * TraceScale;
        private const int TraceRenderDepth = 64 * TraceScale;
        
        private readonly IStorageAccess m_StorageAccess;
        private readonly IIsometricBitmapRenderer m_IsometricBitmapRenderer;
        
        public AlgorithmTraceImageGeneration(
            IStorageAccess storageAccess,
            IIsometricBitmapRenderer isometricBitmapRenderer)
        {
            this.m_StorageAccess = storageAccess;
            this.m_IsometricBitmapRenderer = isometricBitmapRenderer;
        }

        public Bitmap RenderTraceResult(
            RuntimeLayer layer,
            dynamic data,
            int width,
            int height,
            int depth)
        {
            Func<dynamic, Color> getColor;
            getColor = x => layer.Algorithm.GetColorForValue(
                this.m_StorageAccess.FromRuntime(layer),
                x);
            return this.m_IsometricBitmapRenderer.GenerateImage(
                data,
                getColor,
                width,
                height,
                depth);
        }

        /*
        public Bitmap RenderTraceResult(
            RuntimeLayer layer,
            dynamic data,
            int width,
            int height,
            int depth)
        {
            var owidth = width;
            var oheight = height;
            width = 128 * TraceScale;
            height = 128 * TraceScale;
            depth = 128 * TraceScale;

            var b = new Bitmap(width, height);
            var g = Graphics.FromImage(b);
            g.Clear(Color.White);
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            StorageLayer parent;
            if (layer.GetInputs().Length == 0)
                parent = null;
            else
                parent = this.m_StorageAccess.FromRuntime(layer.GetInputs()[0]);

            int[] render = this.m_CellOrderCalculator.CalculateCellRenderOrder(TraceRenderWidth, TraceRenderHeight);
            var ztop = layer.Algorithm.Is2DOnly ? 1 : 128;
            var zbottom = 0;
            for (var z = zbottom; z < ztop; z++)
            {
                var rcx = width / 2 - 1;
                var rcy = height / 2 - (height / 2 - 1);
                var rw = 2;
                var rh = 1;
                for (var i = 0; i < render.Length; i++)
                {
                    // Calculate the X / Y of the tile in the grid.
                    var x = render[i] % TraceRenderWidth;
                    var y = render[i] / TraceRenderWidth;

                    // Calculate the render position on screen.
                    var rx = rcx + (int) ((x - y) / 2.0 * rw); // (int)(x / ((RenderWidth + 1) / 2.0) * rw);
                    var ry = rcy + (x + y) * rh - (rh / 2 * (TraceRenderWidth + TraceRenderHeight)) - (z - zbottom) * 1;

                    while (true)
                    {
                        try
                        {
                            Color lc = layer.Algorithm.GetColorForValue(
                                parent,
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
