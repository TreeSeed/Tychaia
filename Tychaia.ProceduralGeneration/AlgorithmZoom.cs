using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// "Zooms" in on the 1/4 center region of the parent layer, predicting the most likely
    /// values for the data that needs to be filled in (since resolution is being doubled).
    /// </summary>
    [DataContract]
    [FlowDesignerCategory(FlowCategory.General)]
    [FlowDesignerName("Zoom Iterations")]
    public class AlgorithmZoom : Algorithm<int, int>
    {
        [DataMember]
        [DefaultValue(ZoomType.Smooth)]
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

        public AlgorithmZoom()
        {
            this.Mode = ZoomType.Smooth;
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
            bool xmod = x % 2 != 0;
            bool ymod = y % 2 != 0;
            int ocx = xmod ? (int)(i % 2) : 0;
            int ocy = ymod ? (int)(j % 2) : 0;
            int ocx_w = xmod ? (int)((i - 1) % 2) : 0;
            int ocx_e = xmod ? (int)((i + 1) % 2) : 0;
            int ocy_n = ymod ? (int)((j - 1) % 2) : 0;
            int ocy_s = ymod ? (int)((j + 1) % 2) : 0;

            int current = input[(i / 2) + ox + ((j / 2) + oy) * width + ((k / 2) + oz) * width * height];
            /*
            int north = input[(i / 2 + ocx) + ((j - 1) / 2 + ocy_n) * width];
            int south = input[(i / 2 + ocx) + ((j + 1) / 2 + ocy_s) * width];
            int east = input[((i + 1) / 2 + ocx_e) + (j / 2 + ocy) * width];
            int west = input[((i - 1) / 2 + ocx_w) + (j / 2 + ocy) * width];
            int southEast = input[((i + 2) / 2 + ocx) + ((j + 2) / 2 + ocy) * width];
*/
            /*  Template for new Smooth system, have to fix/solve the Fuzzy Problem.
            int selected = 0;

            if (x % 2 == 0 && y % 2 == 0)
                if (ZoomType == ZoomType.Fuzzy)
                    selected = this.GetRandomRange(x, y, 0, 4);
                else
                    selected = this.GetRandomRange(x, y, 0, 3);
            else
                selected = this.GetRandomRange(x, y, 0, 2);

            switch (selected)
            {
                case 0:
                    output[i + j * width] = current;
                case 1:
                    if (x % 2 == 0)
                        output[i + j * width] = south;
                    else if (y % 2 == 0)
                        output[i + j * width] = east;
                    else
                        output[i + j * width] = current;
                case 2:
                    output[i + j * width] = east;
                case 3:
                    output[i + j * width] = southEast;
            }
            */

//            if (this.Mode == ZoomType.Smooth || this.Mode == ZoomType.Fuzzy)
//                output[i + j * width] = context.Smooth(this.Mode == ZoomType.Fuzzy, x, y, north, south, west, east, southEast, current, i, j, 0, 0, width, input);
//            else                    
                output[i + ox + (j + oy) * width + (k + oz) * width * height] = current;
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

        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
