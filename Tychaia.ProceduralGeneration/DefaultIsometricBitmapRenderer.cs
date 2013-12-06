// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Drawing;
using System.Drawing.Text;

namespace Tychaia.ProceduralGeneration
{
    public class DefaultIsometricBitmapRenderer : IIsometricBitmapRenderer
    {
        private ICellOrderCalculator m_CellOrderCalculator;

        public DefaultIsometricBitmapRenderer(
            ICellOrderCalculator cellOrderCalculator)
        {
            this.m_CellOrderCalculator = cellOrderCalculator;
        }

        public Bitmap GenerateImage(
            IGenerator generator,
            Func<dynamic, Color> getColor,
            long ox, 
            long oy, 
            long oz,
            int width, 
            int height,
            int depth)
        {
            var computations = 0;
            dynamic data = generator.GenerateData(ox, oy, oz, width, height, depth, out computations);
            return this.GenerateImage(
                data,
                getColor,
                width, 
                height, 
                depth);
        }

        public Bitmap GenerateImage(
            dynamic data,
            Func<dynamic, Color> getColor,
            int width, 
            int height, 
            int depth)
        {
            var rwidth = Math.Max(width, height) * 2;
            var rheight = Math.Max(width, height) * 3;

            // ARGHGHG FIXME
            var b = new Bitmap(rwidth, rheight);
            var g = Graphics.FromImage(b);
            g.Clear(Color.White);
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            var render = this.m_CellOrderCalculator.CalculateCellRenderOrder(width, height);
            for (var z = 0; z < depth; z++)
            {
                var rw = 2;
                var rh = 1;
                var rcx = (rwidth / 2) - 1;
                var rcy = rheight - (((width - 1) + ((height - 1)) * rh) - (((rh / 2) * (width + height))) - (z - 0) * 1) - 1;
                for (var i = 0; i < render.Length; i++)
                {
                    // Calculate the X / Y of the tile in the grid.
                    var x = render[i] % width;
                    var y = render[i] / width;

                    // Calculate the render position on screen.
                    var rx = rcx + (int)(((x - y) / 2.0) * rw);
                    var ry = rcy + ((x + y) * rh) - ((rh / 2) * (width + height)) - ((z - 0) * 1);

                    while (true)
                    {
                        try
                        {
                            var lc = getColor(data[x + (y * width) + (z * width * height)]);
                            var sb = new SolidBrush(Color.FromArgb(lc.A, lc.R, lc.G, lc.B));
                            g.FillRectangle(
                                sb,
                                new Rectangle(rx, ry, rw, rh));
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