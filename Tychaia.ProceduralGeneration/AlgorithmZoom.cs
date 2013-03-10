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
        public override int[] RequiredZBorder { get { return new int[] {0}; } }
        public override bool[] InputWidthAtHalfSize { get { return new bool[] {true}; } }
        public override bool[] InputHeightAtHalfSize { get { return new bool[] {true}; } }
        public override bool[] InputDepthAtHalfSize { get { return new bool[] {false}; } }

        public AlgorithmZoom()
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

        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz, int[] ocx, int[] ocy, int[] ocz)
        {
            long rw = width / 2 + ox * 2;

            int current = this.FindZoomedPoint(input, i, j, ox, oy, x, y, rw);
            int north = this.FindZoomedPoint(input, i, j - 1, ox, oy, x, y, rw);
            int south = this.FindZoomedPoint(input, i, j + 1, ox, oy, x, y, rw);
            int east = this.FindZoomedPoint(input, i + 1, j, ox, oy, x, y, rw);
            int west = this.FindZoomedPoint(input, i - 1, j, ox, oy, x, y, rw);
            int southEast = this.FindZoomedPoint(input, i + 1, j + 1, ox, oy, x, y, rw);
                
            if (this.Mode == ZoomType.Smooth || this.Mode == ZoomType.Fuzzy)
                output[i + ox + (j + oy) * width + (k + oz) * width * height] = context.Smooth(
                    this.Mode == ZoomType.Fuzzy,
                    x + i,
                    y + j,
                    north,
                    south,
                    west,
                    east,
                    southEast,
                    current,
                    i,
                    j,
                    ox,
                    oy,
                    rw,
                    input);
            else                    
                output[i + ox + (j + oy) * width + (k + oz) * width * height] = current;
        }
        
        private int FindZoomedPoint(int[] input, long i, long j, long ox, long oy, long x, long y, long rw)
        {
            int ocx = (x % 2 != 0) ? (int)(i % 2) : 0;
            int ocy = (y % 2 != 0) ? (int)(j % 2) : 0;
            
            return input[(i / 2 + ox + ocx) + (j / 2 + oy + ocy) * rw];
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
}
