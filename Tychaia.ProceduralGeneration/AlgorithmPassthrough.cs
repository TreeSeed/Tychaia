// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Drawing;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.Undefined)]
    [FlowDesignerCategory(FlowCategory.Debugging)]
    [FlowDesignerName("Passthrough")]
    public class AlgorithmPassthrough : Algorithm<int, int>
    {
        public override int[] RequiredXBorder
        {
            get { return new[] { this.XBorder }; }
        }

        public override int[] RequiredYBorder
        {
            get { return new[] { this.YBorder }; }
        }

        public override int[] RequiredZBorder
        {
            get { return new[] { this.ZBorder }; }
        }

        public override bool[] InputWidthAtHalfSize
        {
            get { return new[] { this.WidthHalf }; }
        }

        public override bool[] InputHeightAtHalfSize
        {
            get { return new[] { this.HeightHalf }; }
        }

        public override bool[] InputDepthAtHalfSize
        {
            get { return new[] { this.DepthHalf }; }
        }

        public int XBorder { get; set; }
        public int YBorder { get; set; }
        public int ZBorder { get; set; }
        public bool WidthHalf { get; set; }
        public bool HeightHalf { get; set; }
        public bool DepthHalf { get; set; }

        public override string[] InputNames
        {
            get { return new[] { "Input" }; }
        }

        public bool Layer2D
        {
            get;
            set;
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public override bool[] InputIs2D
        {
            get { return new[] { this.Layer2D }; }
        }

        public override void ProcessCell(
            IRuntimeContext context,
            int[] input,
            int[] output,
            long x,
            long y,
            long z,
            int i,
            int j,
            int k,
            int width,
            int height,
            int depth,
            int ox,
            int oy,
            int oz)
        {
            output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] =
                input[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)];
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }

    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.Undefined)]
    [FlowDesignerCategory(FlowCategory.Debugging)]
    [FlowDesignerName("Multi Passthrough")]
    public class AlgorithmMultiPassthrough : Algorithm<int, int, int, int>
    {
        public override int[] RequiredXBorder
        {
            get
            {
                return new[]
                {
                    this.XBorderA,
                    this.XBorderB,
                    0
                };
            }
        }

        public override int[] RequiredYBorder
        {
            get
            {
                return new[]
                {
                    this.YBorderA,
                    this.YBorderB,
                    0
                };
            }
        }

        public override bool[] InputWidthAtHalfSize
        {
            get
            {
                return new[]
                {
                    this.WidthHalfA,
                    false,
                    false
                };
            }
        }

        public override bool[] InputHeightAtHalfSize
        {
            get
            {
                return new[]
                {
                    this.HeightHalfA,
                    false,
                    false
                };
            }
        }

        public override bool[] InputIs2D
        {
            get
            {
                return new bool[]
                {
                    this.Layer2D,
                    this.Layer2D,
                    this.Layer2D
                };
            }
        }

        public int XBorderA { get; set; }
        public int YBorderA { get; set; }
        public int XBorderB { get; set; }
        public int YBorderB { get; set; }
        public bool WidthHalfA { get; set; }
        public bool HeightHalfA { get; set; }

        public override string[] InputNames
        {
            get { return new[] { "Input A", "Input B", "Input C" }; }
        }

        public bool Layer2D
        {
            get;
            set;
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public override void Initialize(IRuntimeContext context)
        {
        }

        public override void ProcessCell(IRuntimeContext context, int[] inputA, int[] inputB, int[] inputC, int[] output,
            long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] =
                inputA[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)];
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
