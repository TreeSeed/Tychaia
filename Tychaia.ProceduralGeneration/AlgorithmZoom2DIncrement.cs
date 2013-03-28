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
        [DefaultValue(ZoomType.Smooth)]
        [Description("The zooming algorithm to use.")]
        public ZoomType Mode
        {
            get;
            set;
        }

        public bool __DEBUG_TestEdges
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

        // Keep offsets odd (otherwise it screws it up)
        // TODO: Fix offsets odd not giving the correct ocx/ocy values
        public override int[] RequiredXBorder { get { return new int[] {2}; } }
        public override int[] RequiredYBorder { get { return new int[] {2}; } }
        public override int[] RequiredZBorder { get { return new int[] {0}; } }
        public override bool[] InputWidthAtHalfSize { get { return new bool[] {false}; } }
        public override bool[] InputHeightAtHalfSize { get { return new bool[] {false}; } }
        public override bool[] InputDepthAtHalfSize { get { return new bool[] {false}; } }
        
        public AlgorithmZoom2DIncrement()
        {
            this.Mode = ZoomType.Smooth;
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
            // Offsets for all positions
            bool ocx = (x - i) % 2 == 0;
            bool ocy = (y - j) % 2 == 0;
//            int ocxv = (ocx ? 0 : (i % 2)); // Shifting by an even number
//            int ocxvo = (ocx ? 0 : ((i + 1) % 2)); // Shifting by an odd number
//            int ocyv = (ocy ? 0 : (j % 2));
//            int ocyvo = (ocy ? 0 : ((j + 1) % 2));
            int ocz = 0; // Just a placeholder here

            // Working out current cell value
            // This does have to be i + 1 rather than +1 at the end.
            // Has to be the same as any other access :/
//            int v00 = input[((i + 0) / 2 + ox + ocxv - 1) + ((j + 0) / 2 + oy + ocyv - 1) * width + (k + oz + ocz) * width * height];
//            int v01 = input[((i + 0) / 2 + ox + ocxv - 1) + ((j + 0) / 2 + oy + ocyv - 0) * width + (k + oz + ocz) * width * height];
//            int v02 = input[((i + 0) / 2 + ox + ocxv - 1) + ((j + 0) / 2 + oy + ocyv + 1) * width + (k + oz + ocz) * width * height];
//            int v10 = input[((i + 0) / 2 + ox + ocxv + 0)  + ((j + 0) / 2 + oy + ocyv - 1) * width + (k + oz + ocz) * width * height];
//            int v11 = input[((i + 0) / 2 + ox + ocxv + 0)  + ((j + 0) / 2 + oy + ocyv) * width + (k + oz + ocz) * width * height];
//            int v12 = input[((i + 0) / 2 + ox + ocxv + 0)  + ((j + 0) / 2 + oy + ocyv + 1) * width + (k + oz + ocz) * width * height];
//            int v20 = input[((i + 0) / 2 + ox + ocxv + 1) + ((j + 0) / 2 + oy + ocyv - 1) * width + (k + oz + ocz) * width * height];
//            int v21 = input[((i + 0) / 2 + ox + ocxv + 1) + ((j + 0) / 2 + oy + ocyv + 0) * width + (k + oz + ocz) * width * height];
//            int v22 = input[((i + 0) / 2 + ox + ocxv + 1) + ((j + 0) / 2 + oy + ocyv + 1) * width + (k + oz + ocz) * width * height];
  

            int ocxvo = 0;
            int ocxv = 0;
            int ocyvo = 0; 
            int ocyv = 0;
            int v00 = input[((i - 1) + ox - ocxvo - 1) + ((j - 1) + oy - ocyvo - 1) * width + (k + oz + ocz) * width * height];
            int v01 = input[((i - 1) + ox - ocxvo - 0) + ((j + 0) + oy + ocyv - 0) * width + (k + oz + ocz) * width * height];
            int v02 = input[((i - 1) + ox - ocxvo - 1) + ((j + 1) + oy + ocyvo + 1) * width + (k + oz + ocz) * width * height];
            int v10 = input[((i + 0) + ox + ocxv + 0) + ((j - 1) + oy - ocyvo - 0) * width + (k + oz + ocz) * width * height];
            int v11 = input[((i + 0) + ox + ocxv + 0) + ((j + 0) + oy + ocyv) * width + (k + oz + ocz) * width * height];
            int v12 = input[((i + 0) + ox + ocxv + 0) + ((j + 1) + oy + ocyvo + 0) * width + (k + oz + ocz) * width * height];
            int v20 = input[((i + 1) + ox + ocxvo + 1) + ((j - 1) + oy - ocyvo - 1) * width + (k + oz + ocz) * width * height];
            int v21 = input[((i + 1) + ox + ocxvo + 0) + ((j + 0) + oy + ocyv + 0) * width + (k + oz + ocz) * width * height];
            int v22 = input[((i + 1) + ox + ocxvo + 1) + ((j + 1) + oy + ocyvo + 1) * width + (k + oz + ocz) * width * height];
            // v11 is the center value we're zooming in on.
            int mod = 0;

            if (v11 < 1)
            {
                // Water bitches.
            }
            else
            {
                if (__DEBUG_TestEdges)
                {
                    if (v12 < v11 ||
                        v21 < v11 ||
                        v10 < v11 ||
                        v01 < v11)
                        mod = 0;
                    else
                        mod = 1;
                }
                else
                {
                    
                    if (v12 < v11 ||
                        v21 < v11)
                        mod = 0;
                    else
                        mod = 1;
                }
            }

            /*if (v11 < 1)
            {
//                if (v00 > v11 ||
//                    v01 > v11 ||
//                    v02 > v11 ||
//                    v10 > v11 ||
//                    v12 > v11 ||
//                    v20 > v11 ||
//                    v21 > v11 ||
//                    v22 > v11)
//                    mod = 0;
//                else
//                    mod = -1;
            }
            else
            {

                // TODO: Doesn't work
//                if (v00 < v11 ||
//                    v01 < v11 ||
//                    v02 < v11 ||
//                    v10 < v11 ||
//                    v12 < v11 ||
//                    v20 < v11 ||
//                    v21 < v11 ||
//                    v22 < v11)
                    // TODO: Does work
                    if (v12 < v11 || // only these 2
                        v21 < v11) //|| // only these 2
//                        v10 < v11 ||
//                        v01 < v11)
                    mod = 0;
                else
                    mod = 1;
            }*/

            int current = v11 + mod;

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
                                
                // v11 is the center value we're zooming in on.
                
                int east = v21 + mod;

                switch (selected)
                {
                    case 0:
                        output[i + ox + (j + oy) * width + (k + oz) * width * height] = current;
                        break;
                    case 1:

                        // Working out current cell value
                        
                        // v11 is the center value we're zooming in on.
                        int south = v02 + mod;

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
                        // v11 is the center value we're zooming in on.

                        int southEast = v22 + mod;

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
            else if (a < -255)
                a = -255;

            if (this.ColorSet == ColorScheme.Perlin)
                return Color.FromArgb(a, a, a);
            else if (this.ColorSet == ColorScheme.Land)
            if (a == 0)
                return Color.FromArgb(0, 0, 255);
            else if (a > 0)
                return Color.FromArgb(0, a, 0);
            else
                return Color.FromArgb(0, 0, 255 + a);
            else
                return Color.Gray;
        }
    }
}
