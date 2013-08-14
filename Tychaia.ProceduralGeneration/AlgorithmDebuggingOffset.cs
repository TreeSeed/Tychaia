// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.Undefined)]
    [FlowDesignerCategory(FlowCategory.Debugging)]
    [FlowDesignerName("Debug Offset")]
    public class AlgorithmDebuggingOffset : Algorithm<int, int>
    {
        [DataMember]
        [DefaultValue(0)]
        public int OffsetX { get; set; }

        [DataMember]
        [DefaultValue(0)]
        public int OffsetY { get; set; }

        [DataMember]
        [DefaultValue(0)]
        public int OffsetZ { get; set; }

        public override int[] RequiredXBorder
        {
            get { return new[] { Math.Abs(this.OffsetX) }; }
        }

        public override int[] RequiredYBorder
        {
            get { return new[] { Math.Abs(this.OffsetY) }; }
        }

        public override int[] RequiredZBorder
        {
            get { return new[] { Math.Abs(this.OffsetZ) }; }
        }

        [DataMember]
        public bool ShowAs2D { get; set; }

        public override bool Is2DOnly
        {
            get { return this.ShowAs2D; }
        }

        public Action<IRuntimeContext, int[], int[], long, long, long, int, int, int, int, int, int, int, int, int>
            Delegate { get; set; }

        public override string[] InputNames
        {
            get { return new[] { "Parent" }; }
        }

        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z,
            int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] =
                input[
                    (i + this.OffsetX + ox) + (j + this.OffsetY + oy) * width + (k + this.OffsetZ + oz) * width * height
                    ];
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
