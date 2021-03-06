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
    [FlowDesignerCategory(FlowCategory.Initials)]
    [FlowDesignerName("Distance from 0")]
    public class AlgorithmDistanceFrom0 : Algorithm<int>
    {
        public AlgorithmDistanceFrom0()
        {
            this.Xcalculation = true;
            this.Ycalculation = true;
            this.Zcalculation = true;
            this.Xdivider = 20;
            this.Ydivider = 20;
            this.Zdivider = 20;
        }

        [DataMember]
        [DefaultValue(true)]
        [Description("Calculate distance from X axis.")]
        public bool Xcalculation { get; set; }

        [DataMember]
        [DefaultValue(true)]
        [Description("Calculate distance from Y axis.")]
        public bool Ycalculation { get; set; }

        [DataMember]
        [DefaultValue(true)]
        [Description("Calculate distance from Z axis.")]
        public bool Zcalculation { get; set; }

        [DataMember]
        [DefaultValue(20)]
        [Description("Divides X value by this much.")]
        public long Xdivider { get; set; }

        [DataMember]
        [DefaultValue(20)]
        [Description("Divides Y value by this much.")]
        public long Ydivider { get; set; }

        [DataMember]
        [DefaultValue(20)]
        [Description("Divides Z value by this much.")]
        public long Zdivider { get; set; }

        public override bool Is2DOnly
        {
            get { return false; }
        }

        public override void ProcessCell(
            IRuntimeContext context,
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
            output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] = 0;

            if (this.Xcalculation)
            {
                output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] +=
                    ((int)(Math.Abs(x) / this.Xdivider) > int.MaxValue
                        ? int.MaxValue
                        : (int)(Math.Abs(x) / this.Xdivider));
            }

            if (this.Ycalculation)
            {
                output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] +=
                    ((int)(Math.Abs(y) / this.Ydivider) > int.MaxValue
                        ? int.MaxValue
                        : (int) (Math.Abs(y) / this.Ydivider));
            }

            if (this.Zcalculation)
            {
                output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] +=
                    ((int)(Math.Abs(z) / this.Zdivider) > int.MaxValue
                        ? int.MaxValue
                        : (int)(Math.Abs(z) / this.Zdivider));
            }
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            int a = value;

            if (a > 255)
                a = 255;

            return Color.FromArgb(a, a, a);
        }
    }
}
