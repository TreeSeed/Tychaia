using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// A generic color container used so that HTML5 can access the information
    /// (System.Drawing is not implemented in JSIL).
    /// </summary>
    public struct LayerColor
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public LayerColor(int r, int g, int b, int a)
        {
            this.R = (byte)r;
            this.G = (byte)g;
            this.B = (byte)b;
            this.A = (byte)a;
        }

        public LayerColor(int r, int g, int b)
        {
            this.R = (byte)r;
            this.G = (byte)g;
            this.B = (byte)b;
            this.A = 255;
        }

        public LayerColor(int hex)
        {
            this.R = (byte)((hex & 0xFF0000) >> 16);
            this.G = (byte)((hex & 0x00FF00) >> 8);
            this.B = (byte)((hex & 0x0000FF) >> 0);
            this.A = 255;
        }

        public LayerColor(int hex, int alpha)
        {
            this.R = (byte)((hex & 0xFF0000) >> 16);
            this.G = (byte)((hex & 0x00FF00) >> 8);
            this.B = (byte)((hex & 0x0000FF) >> 0);
            this.A = (byte)alpha;
        }

        #region Color Definitions

        // Sourced from: http://en.wikipedia.org/wiki/List_of_colors

        public static LayerColor Transparent = new LayerColor(0x000000, 0x00);
        public static LayerColor Black = new LayerColor(0x000000);
        public static LayerColor Yellow = new LayerColor(0xFFFF00);
        public static LayerColor Red = new LayerColor(0xFF0000);
        public static LayerColor Beige = new LayerColor(0xF5F5DC);
        public static LayerColor FloralWhite = new LayerColor(0xFFFAF0);
        public static LayerColor Gold = new LayerColor(0xFFD700);
        public static LayerColor Blue = new LayerColor(0x0000FF);
        public static LayerColor Green = new LayerColor(0x008000);
        public static LayerColor Brown = new LayerColor(0x6B4423);
        public static LayerColor DarkGreen = new LayerColor(0x013220);

        #endregion

        [Obsolete]
        public static LayerColor FromArgb(int r, int g, int b)
        {
            return new LayerColor(r, g, b);
        }
    }
}
