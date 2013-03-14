using System.ComponentModel;
using System.Runtime.Serialization;
using System;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// "Zooms" in on the 1/4 center region of the parent layer, predicting the most likely
    /// values for the data that needs to be filled in (since resolution is being doubled).
    /// Only works in 2 dimensions.
    /// </summary>
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General2D)]
    [FlowDesignerCategory(FlowCategory.Zooming)]
    [FlowDesignerName("Zoom 2D")]
    public class AlgorithmZoom2D : Algorithm<int, int>
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
        public override int[] RequiredZBorder { get { return new int[] {0}; } }
        public override bool[] InputWidthAtHalfSize { get { return new bool[] {true}; } }
        public override bool[] InputHeightAtHalfSize { get { return new bool[] {true}; } }
        public override bool[] InputDepthAtHalfSize { get { return new bool[] {false}; } }
        
        public AlgorithmZoom2D()
        {
            this.Mode = ZoomType.Square;
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
            
            int current = input[(i / 2) + ox + ocx + ((j / 2) + oy + ocy) * width + (k + oz + ocz) * width * height];

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
                int east = input[((i + 1) / 2 + ox + ocx_e) + (j / 2 + oy + ocy) * width + ((k) + oz + ocz) * width * height];

                switch (selected)
                {
                    case 0:
                        output[i + ox + (j + oy) * width + (k + oz) * width * height] = current;
                        break;
                    case 1:

                        int ocy_s = ((y - j) % 2 == 0 ? 0 : ((j + 1) % 2));
                        int south = input[(i / 2 + ox + ocx) + ((j + 1) / 2 + oy + ocy_s) * width + (k + oz + ocz) * width * height];

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
                        int southEast = input[((i + 2) / 2 + ox + ocx) + ((j + 2) / 2 + oy + ocy) * width + (k + oz + ocz) * width * height];
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
        
        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
