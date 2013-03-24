using System.ComponentModel;
using System.Runtime.Serialization;
using System;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// "Zooms" in on the 1/4 center region of the parent layer, predicting the most likely
    /// values for the data that needs to be filled in (since resolution is being doubled).
    /// Only works in 2 dimensions.
    /// </summary>
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Zooming)]
    [FlowDesignerName("Zoom 2D Increment")]
    public class AlgorithmZoom2DIncrement : Algorithm<int, int>
    {
        [DataMember]
        [DefaultValue(ZoomType.Square)]
        [Description("The zooming algorithm to use.")]
        public ZoomType Mode
        {
            get;
            set;
        }        

        [DataMember]
        [DefaultValue(5)]
        [Description("Estimated Max value.")]
        public int EstimateMax
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(ColorScheme.Land)]
        [Description("The color scheme to use.")]
        public ColorScheme ColorSet
        {
            get;
            set;
        }
        
        public override int[] RequiredXBorder { get { return new int[] {3}; } }
        public override int[] RequiredYBorder { get { return new int[] {3}; } }
        public override int[] RequiredZBorder { get { return new int[] {0}; } }
        public override bool[] InputWidthAtHalfSize { get { return new bool[] {true}; } }
        public override bool[] InputHeightAtHalfSize { get { return new bool[] {true}; } }
        public override bool[] InputDepthAtHalfSize { get { return new bool[] {false}; } }
        
        public AlgorithmZoom2DIncrement()
        {
            this.Mode = ZoomType.Square;
            this.EstimateMax = 5;
            this.ColorSet = ColorScheme.Land;
        }

        public override string[] InputNames
        {
            get { return new string[] { "Input" }; }
        }
        
        public override bool Is2DOnly
        {
            get { return true; }
        }
        
        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            int ocx = ((x - i) % 2 == 0 ? 0 : (i % 2));
            int ocy = ((y - j) % 2 == 0 ? 0 : (j % 2));
            int ocz = 0;

            // Working out current cell value
            int v00 = input[(i / 2 + ox + ocx - 1) + (j / 2 + oy + ocy - 1) * width + (k + oz + ocz) * width * height];
            int v01 = input[(i / 2 + ox + ocx - 1) + (j / 2 + oy + ocy) * width + (k + oz + ocz) * width * height];
            int v02 = input[(i / 2 + ox + ocx - 1) + (j / 2 + oy + ocy + 1) * width + (k + oz + ocz) * width * height];
            int v10 = input[(i / 2 + ox + ocx) + (j / 2 + oy + ocy - 1) * width + (k + oz + ocz) * width * height];
            int v11 = input[(i / 2 + ox + ocx) + (j / 2 + oy + ocy) * width + (k + oz + ocz) * width * height];
            int v12 = input[(i / 2 + ox + ocx) + (j / 2 + oy + ocy + 1) * width + (k + oz + ocz) * width * height];
            int v20 = input[(i / 2 + ox + ocx + 1) + (j / 2 + oy + ocy - 1) * width + (k + oz + ocz) * width * height];
            int v21 = input[(i / 2 + ox + ocx + 1) + (j / 2 + oy + ocy) * width + (k + oz + ocz) * width * height];
            int v22 = input[(i / 2 + ox + ocx + 1) + (j / 2 + oy + ocy + 1) * width + (k + oz + ocz) * width * height];

            // v11 is the center value we're zooming in on.
            int mod = 0;

            if ((v00 < v11 || v20 < v11 || v02 < v11 || v22 < v11) && v11 > 0)
                mod = 1;
            else if (v11 < 0 && (v01 > v11 || v10 > v11 || v21 > v11 || v12 > v11))
                mod = -1;

            int current = v11 * 2 - mod;

            if (this.Mode == ZoomType.Square)
                output[i + ox + (j + oy) * width + (k + oz) * width * height] = current;
            else
            {
                int selected;
                
                bool ymod = (y) % 2 == 0;
                bool xmod = (x) % 2 == 0;

                if (!xmod && !ymod)
                if (this.Mode == ZoomType.Fuzzy)
                    selected = context.GetRandomRange(x, y, 0, 4);
                else
                    selected = context.GetRandomRange(x, y, 0, 3);
                else if (xmod && ymod)
                    selected = 4;
                else
                    selected = context.GetRandomRange(x, y, 0, 2);
                
                int ocx_e = ((x - i) % 2 == 0 ? 0 : ((i + 1) % 2));

                // Working out current cell value
                v00 = input[((i + 1)/ 2 + ox + ocx_e - 1) + (j / 2 + oy + ocy - 1) * width + (k + oz + ocz) * width * height];
                v01 = input[((i + 1)/ 2 + ox + ocx_e - 1) + (j / 2 + oy + ocy) * width + (k + oz + ocz) * width * height];
                v02 = input[((i + 1)/ 2 + ox + ocx_e - 1) + (j / 2 + oy + ocy + 1) * width + (k + oz + ocz) * width * height];
                v10 = input[((i + 1)/ 2 + ox + ocx_e) + (j / 2 + oy + ocy - 1) * width + (k + oz + ocz) * width * height];
                v11 = input[((i + 1)/ 2 + ox + ocx_e) + (j / 2 + oy + ocy) * width + (k + oz + ocz) * width * height];
                v12 = input[((i + 1)/ 2 + ox + ocx_e) + (j / 2 + oy + ocy + 1) * width + (k + oz + ocz) * width * height];
                v20 = input[((i + 1)/ 2 + ox + ocx_e + 1) + (j / 2 + oy + ocy - 1) * width + (k + oz + ocz) * width * height];
                v21 = input[((i + 1)/ 2 + ox + ocx_e + 1) + (j / 2 + oy + ocy) * width + (k + oz + ocz) * width * height];
                v22 = input[((i + 1)/ 2 + ox + ocx_e + 1) + (j / 2 + oy + ocy + 1) * width + (k + oz + ocz) * width * height];
                
                // v11 is the center value we're zooming in on.
                mod = 0;
                
                if ((v00 < v11 || v20 < v11 || v02 < v11 || v22 < v11) && v11 > 0)
                    mod = 1;
                else if (v11 < 0 && (v01 > v11 || v10 > v11 || v21 > v11 || v12 > v11))
                    mod = -1;

                int east = v11 * 2 - mod;

                switch (selected)
                {
                    case 0:
                        output[i + ox + (j + oy) * width + (k + oz) * width * height] = current;
                        break;
                    case 1:
                        int ocy_s = ((y - j) % 2 == 0 ? 0 : ((j + 1) % 2));

                        // Working out current cell value
                        v00 = input[(i / 2 + ox + ocx - 1) + ((j + 1) / 2 + oy + ocy_s - 1) * width + (k + oz + ocz) * width * height];
                        v01 = input[(i / 2 + ox + ocx - 1) + ((j + 1) / 2 + oy + ocy_s) * width + (k + oz + ocz) * width * height];
                        v02 = input[(i / 2 + ox + ocx - 1) + ((j + 1) / 2 + oy + ocy_s + 1) * width + (k + oz + ocz) * width * height];
                        v10 = input[(i / 2 + ox + ocx) + ((j + 1) / 2 + oy + ocy_s - 1) * width + (k + oz + ocz) * width * height];
                        v11 = input[(i / 2 + ox + ocx) + ((j + 1) / 2 + oy + ocy_s) * width + (k + oz + ocz) * width * height];
                        v12 = input[(i / 2 + ox + ocx) + ((j + 1) / 2 + oy + ocy_s + 1) * width + (k + oz + ocz) * width * height];
                        v20 = input[(i / 2 + ox + ocx + 1) + ((j + 1) / 2 + oy + ocy_s - 1) * width + (k + oz + ocz) * width * height];
                        v21 = input[(i / 2 + ox + ocx + 1) + ((j + 1) / 2 + oy + ocy_s) * width + (k + oz + ocz) * width * height];
                        v22 = input[(i / 2 + ox + ocx + 1) + ((j + 1) / 2 + oy + ocy_s + 1) * width + (k + oz + ocz) * width * height];
                        
                        // v11 is the center value we're zooming in on.
                        mod = 0;
                        
                        if ((v00 < v11 || v20 < v11 || v02 < v11 || v22 < v11) && v11 > 0)
                            mod = 1;
                        else if (v11 < 0 && (v01 > v11 || v10 > v11 || v21 > v11 || v12 > v11))
                            mod = -1;
                        
                        int south = v11 * 2 - mod;

                        if (xmod)
                            output[i + ox + (j + oy) * width + (k + oz) * width * height] = south;
                        else if (ymod)
                            output[i + ox + (j + oy) * width + (k + oz) * width * height] = east;
                        else
                            output[i + ox + (j + oy) * width + (k + oz) * width * height] = south;
                        break;
                    case 2:
                        output[i + ox + (j + oy) * width + (k + oz) * width * height] = east;
                        break;
                    case 3:
                        // Working out current cell value
                        v00 = input[((i + 2) / 2 + ox + ocx - 1) + ((j + 2) / 2 + oy + ocy - 1) * width + (k + oz + ocz) * width * height];
                        v01 = input[((i + 2) / 2 + ox + ocx - 1) + ((j + 2) / 2 + oy + ocy) * width + (k + oz + ocz) * width * height];
                        v02 = input[((i + 2) / 2 + ox + ocx - 1) + ((j + 2) / 2 + oy + ocy + 1) * width + (k + oz + ocz) * width * height];
                        v10 = input[((i + 2) / 2 + ox + ocx) + ((j + 2) / 2 + oy + ocy - 1) * width + (k + oz + ocz) * width * height];
                        v11 = input[((i + 2) / 2 + ox + ocx) + ((j + 2) / 2 + oy + ocy) * width + (k + oz + ocz) * width * height];
                        v12 = input[((i + 2) / 2 + ox + ocx) + ((j + 2) / 2 + oy + ocy + 1) * width + (k + oz + ocz) * width * height];
                        v20 = input[((i + 2) / 2 + ox + ocx + 1) + ((j + 2) / 2 + oy + ocy - 1) * width + (k + oz + ocz) * width * height];
                        v21 = input[((i + 2) / 2 + ox + ocx + 1) + ((j + 2) / 2 + oy + ocy) * width + (k + oz + ocz) * width * height];
                        v22 = input[((i + 2) / 2 + ox + ocx + 1) + ((j + 2) / 2 + oy + ocy + 1) * width + (k + oz + ocz) * width * height];
                        
                        // v11 is the center value we're zooming in on.
                        mod = 0;
                        
                        if ((v00 < v11 || v20 < v11 || v02 < v11 || v22 < v11) && v11 > 0)
                            mod = 1;
                        else if (v11 < 0 && (v01 > v11 || v10 > v11 || v21 > v11 || v12 > v11))
                            mod = -1;
                        
                        int southEast = v11 * 2 - mod;

                        output[i + ox + (j + oy) * width + (k + oz) * width * height] = southEast;
                        break;
                    case 4:
                        output[i + ox + (j + oy) * width + (k + oz) * width * height] = current;
                        break;
                }
            }
        }
        
        /// <summary>
        /// An enumeration defining the type of zoom to perform.
        /// </summary>
        public enum ZoomType
        {
            Square,
            Smooth,
            Fuzzy,
        }
        
        public enum ColorScheme
        {
            Land,
            Perlin,
        }

        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            int a;
            
            double divvalue = (double)this.EstimateMax;
            
            if (divvalue > 255)
                divvalue = 255;
            else if (divvalue < 1)
                divvalue = 1;
            
            a = (int)(value * ((double)255 / divvalue));
            
            if (a > 255)
                a = 255;

            if (this.ColorSet == ColorScheme.Perlin)
                return Color.FromArgb(a, a, a);
            else if (this.ColorSet == ColorScheme.Land)
                if (a == 0)
                    return Color.Blue;
                else
                    return Color.FromArgb(0, a, 0);
            else
                return Color.Gray;
        }
    }
}
