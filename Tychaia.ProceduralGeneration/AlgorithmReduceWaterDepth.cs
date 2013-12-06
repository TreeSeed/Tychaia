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
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Terrain)]
    [FlowDesignerName("Reduce Water Depth")]
    public class AlgorithmReduceWaterDepth : Algorithm<int, int>
    {
        public AlgorithmReduceWaterDepth()
        {
            this.Divisor = 1;
        }

        [DataMember]
        [DefaultValue(2)]
        [Description("The divisor for reduction of water depth.")]
        public int Divisor { get; set; }

        public override bool Is2DOnly
        {
            get { return true; }
        }

        public override bool[] InputIs2D
        {
            get { return new[] { true }; }
        }

        public override string[] InputNames
        {
            get { return new[] { "Input" }; }
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
            if (input[(i + ox) + ((j + oy) * width)] < 0)
                output[(i + ox) + ((j + oy) * width)] =
                    (int)
                        Math.Floor(input[(i + ox) + ((j + oy) * width)] /
                                   (double)(this.Divisor == 0 ? 1 : Math.Abs(this.Divisor)));
            else
                output[(i + ox) + ((j + oy) * width)] =
                    input[(i + ox) + ((j + oy) * width)];
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            var color = this.DelegateColorForValueToParent(parent, value);
            int b = color.B;
            if (b > 0)
                b = color.B - (byte)((255 - (int) color.B) * (this.Divisor - 1));
            b = Math.Min(b, 255);
            b = Math.Max(0, b);
            return Color.FromArgb(color.A, color.R, color.G, b);
        }
    }
}
