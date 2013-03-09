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

        public AlgorithmZoom()
        {
            this.Mode = ZoomType.Square;
            this.Layer2d = false;
        }

        public override string[] InputNames
        {
            get { return new string[] { "Input" }; }
        }

        [DataMember]
        [DefaultValue(false)]
        [Description("This layer is 2d.")]
        public bool Layer2d
        {
            get;
            set;
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2d; }
        }

        public override void Initialize(IRuntimeContext context)
        {
        }

        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz, int[] ocx, int[] ocy, int[] ocz)
        {

//            bool xmod = x % 2 != 0;
//            bool ymod = y % 2 != 0;
//            int ocx = xmod ? (int)(i % 2) : 0;
//            int ocy = ymod ? (int)(j % 2) : 0;
//            int ocx_w = xmod ? (int)((i - 1) % 2) : 0;
//            int ocx_e = xmod ? (int)((i + 1) % 2) : 0;
//            int ocy_n = ymod ? (int)((j - 1) % 2) : 0;
//            int ocy_s = ymod ? (int)((j + 1) % 2) : 0;

            int current = input[(i / 2) + ox + ocx[0] + ((j / 2) + oy + ocy[0]) * width + ((k / 2) + oz + ocz[0]) * width * height];
            
            int ocx_e = ((x - i) % 2 == 1 ? (ocx[0]== 1 ? 0 : 1) : 0);
            int ocx_w = ((x - i) % 2 == 1 ? (ocx[0]== 1 ? 0 : 1) : 0);
            int ocy_s = ((y - j) % 2 == 1 ? (ocy[0]== 1 ? 0 : 1) : 0);
            int ocy_n = ((y - j) % 2 == 1 ? (ocy[0]== 1 ? 0 : 1) : 0);

            int north = input[(i / 2 + ox + ocx[0]) + ((j - 1) / 2 + oy + ocy_n) * width + ((k / 2) + oz + ocz[0]) * width * height];
            int south = input[(i / 2 + ox + ocx[0]) + ((j + 1) / 2 + oy + ocy_s) * width + ((k / 2) + oz + ocz[0]) * width * height];
            int east = input[((i + 1) / 2 + ox + ocx_e) + (j / 2 + oy + ocy[0]) * width + ((k / 2) + oz + ocz[0]) * width * height];
            int west = input[((i - 1) / 2 + ox + ocx_w) + (j / 2 + oy + ocy[0]) * width + ((k / 2) + oz + ocz[0]) * width * height];
            int southEast = input[((i + 2) / 2 + ox + ocx[0]) + ((j + 2) / 2 + oy + ocy[0]) * width + ((k / 2) + oz + ocz[0]) * width * height];

            //Template for new Smooth system, have to fix/solve the Fuzzy Problem.
            if (this.Mode == ZoomType.Square)
                output[i + ox + (j + oy) * width + (k + oz) * width * height] = current;
            else
            {
                int selected = 0;

                if (x % 2 == 0 && y % 2 == 0)
                if (this.Mode == ZoomType.Fuzzy)
                    selected = context.GetRandomRange(x, y, z, 4);
                else
                    selected = context.GetRandomRange(x, y, z, 3);
                else
                    selected = context.GetRandomRange(x, y, z, 2);

                switch (selected)
                {
                    case 0:
                        output[i + ox + (j + oy) * width + (k + oz) * width * height] = current;
                        break;
                    case 1:
                        if (x % 2 == 0)
                            output[i + ox + (j + oy) * width + (k + oz) * width * height] = south;
                        else if (y % 2 == 0)
                            output[i + ox + (j + oy) * width + (k + oz) * width * height] = east;
                        else
                            output[i + ox + (j + oy) * width + (k + oz) * width * height] = current;
                        break;
                    case 2:
                        output[i + ox + (j + oy) * width + (k + oz) * width * height] = east;
                        break;
                    case 3:
                        output[i + ox + (j + oy) * width + (k + oz) * width * height] = southEast;
                        break;
                }
            }


//            if (this.Mode == ZoomType.Smooth || this.Mode == ZoomType.Fuzzy)
//                output[i + j * width] = context.Smooth(this.Mode == ZoomType.Fuzzy, x, y, north, south, west, east, southEast, current, i, j, 0, 0, width, input);
//            else                    


            //output[i + ox + (j + oy) * width + (k + oz) * width * height] = current;

        }

        /// <summary>
        /// An enumeration defining the type of zoom to perform.
        /// </summary>
        public enum ZoomType
        {
            Square,
            SquareSmooth,
            Fuzzy,
        }

        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
