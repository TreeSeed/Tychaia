//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

using System.ComponentModel;
using System.Runtime.Serialization;
using System;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// "Zooms" in on the 1/8 center region of the parent layer, predicting the most likely
    /// values for the data that needs to be filled in (since resolution is being doubled).
    /// Works in 3 dimensions.
    /// </summary>
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General3D)]
    [FlowDesignerCategory(FlowCategory.Zooming)]
    [FlowDesignerName("Zoom 3D")]
    public class AlgorithmZoom3D : Algorithm<int, int>
    {
        [DataMember]
        [DefaultValue(ZoomType.Square)]
        [Description("The zooming algorithm to use.")]
        public ZoomType Mode
        {
            get;
            set;
        }
        
        public override int[] RequiredXBorder { get { return new int[] {2}; } }
        public override int[] RequiredYBorder { get { return new int[] {2}; } }
        public override int[] RequiredZBorder { get { return new int[] {2}; } }
        public override bool[] InputWidthAtHalfSize { get { return new bool[] {true}; } }
        public override bool[] InputHeightAtHalfSize { get { return new bool[] {true}; } }
        public override bool[] InputDepthAtHalfSize { get { return new bool[] {true}; } }

        public AlgorithmZoom3D()
        {
            this.Mode = ZoomType.Square;
        }

        public override string[] InputNames
        {
            get { return new string[] { "Input" }; }
        }

        public override bool Is2DOnly
        {
            get { return false; }
        }

        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
//            bool xmod = x % 2 != 0;
//            bool ymod = y % 2 != 0;
//            int ocx = xmod ? (int)(i % 2) : 0;
//            int ocy = ymod ? (int)(j % 2) : 0;
//            int ocx_w = xmod ? (int)((i - 1) % 2) : 0;
//            int ocx_e = xmod ? (int)((i + 1) % 2) : 0;
//            int ocy_n = ymod ? (int)((j - 1) % 2) : 0;
//            int ocy_s = ymod ? (int)((j + 1) % 2) : 0;

            int ocx = ((x - i) % 2 == 0 ? 0 : (i % 2));
            int ocy = ((y - j) % 2 == 0 ? 0 : (j % 2));
            int ocz = ((z - k) % 2 == 0 ? 0 : (k % 2));

            int current = input[(i / 2) + ox + ocx + ((j / 2) + oy + ocy) * width + ((k / 2) + oz + ocz) * width * height];

            //Template for new Smooth system, have to fix/solve the Fuzzy Problem.
            if (this.Mode == ZoomType.Square)
                output[i + ox + (j + oy) * width + (k + oz) * width * height] = current;
            else
            {
                int selected;

                if ((x - i) % 2 == 0 && (y - j) % 2 == 0)
                if (this.Mode == ZoomType.Fuzzy)
                    selected = context.GetRandomRange(x, y, z, 4);
                else
                    selected = context.GetRandomRange(x, y, z, 3);
                else
                    selected = context.GetRandomRange(x, y, z, 2);

                int ocx_e = ((x - i) % 2 == 1 ? (ocx == 1 ? 0 : 1) : 0);
                int east = input[((i + 1) / 2 + ox + ocx_e) + (j / 2 + oy + ocy) * width + ((k / 2) + oz + ocz) * width * height];
                switch (selected)
                {
                    case 0:
                        output[i + ox + (j + oy) * width + (k + oz) * width * height] = current;
                        break;
                    case 1:
                        if ((x - i) % 2 == 0)
                        {
                            int ocy_s = ((y - j) % 2 == 1 ? (ocy == 1 ? 0 : 1) : 0);
                            int south = input[(i / 2 + ox + ocx) + ((j + 1) / 2 + oy + ocy_s) * width + ((k / 2) + oz + ocz) * width * height];
                            output[i + ox + (j + oy) * width + (k + oz) * width * height] = south;
                        }
                        else if ((y - i) % 2 == 0)
                            output[i + ox + (j + oy) * width + (k + oz) * width * height] = east;
                        else
                            output[i + ox + (j + oy) * width + (k + oz) * width * height] = current;
                        break;
                    case 2:
                        output[i + ox + (j + oy) * width + (k + oz) * width * height] = east;
                        break;
                    case 3:
                        int southEast = input[((i + 2) / 2 + ox + ocx) + ((j + 2) / 2 + oy + ocy) * width + ((k / 2) + oz + ocz) * width * height];
                        output[i + ox + (j + oy) * width + (k + oz) * width * height] = southEast;
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
            SquareSmooth,
            Smooth,
            Fuzzy,
        }

        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }

    /// <summary>
    /// "Zooms" in on the 1/4 center region of the parent layer, predicting the most likely
    /// values for the data that needs to be filled in (since resolution is being doubled).
    /// Only works in 2 dimensions.
    /// </summary>
    
}
