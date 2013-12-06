// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Output)]
    [FlowDesignerName("Height Change")]
    public class AlgorithmHeightChange : Algorithm<int, int>
    {
        public AlgorithmHeightChange()
        {
            this.XorY = true;
            this.EstimateMax = 5;
            this.Layer2D = true;
        }

        public override int[] RequiredXBorder
        {
            get { return new[] { 2 }; }
        }

        public override int[] RequiredYBorder
        {
            get { return new[] { 2 }; }
        }

        public override int[] RequiredZBorder
        {
            get { return new[] { 0 }; }
        }

        public override string[] InputNames
        {
            get { return new[] { "2D Terrain Map" }; }
        }

        [DataMember]
        [DefaultValue(true)]
        [Description("True = X, False = Y")]
        public bool XorY { get; set; }

        [DataMember]
        [DefaultValue(false)]
        [Description("Invert the blue / green colours.")]
        public bool InverseColours { get; set; }

        [DataMember]
        [DefaultValue(5)]
        [Description("Estimate maximum value.")]
        public int EstimateMax { get; set; }

        [DataMember]
        [DefaultValue(true)]
        [Description("Show this layer as 2D in the editor.")]
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
            if (!this.XorY)
                output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] =
                    input[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] -
                    input[(i + ox) + ((j + 1 + oy) * width) + ((k + oz) * width * height)];
            else
                output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] =
                    input[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] -
                    input[(i + 1 + ox) + ((j + oy) * width) + ((k + oz) * width * height)];
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (value == 0)
                return Color.Black;

            int a;

            double divvalue = this.EstimateMax;

            if (divvalue > 255)
                divvalue = 255;
            else if (divvalue < 1)
                divvalue = 1;

            a = (int)(value * ((double) 255 / divvalue));

            if (a < 0)
            {
                if (this.InverseColours)
                {
                    if (a < -255)
                        return Color.FromArgb(0, 255, 0);
                    return Color.FromArgb(0, -a, 0);
                }

                if (a < -255)
                    return Color.FromArgb(0, 0, 255);
                return Color.FromArgb(0, 0, -a);
            }

            if (a > 255)
                a = 255;

            if (a == 0)
                return Color.Black;
            if (this.InverseColours)
                return Color.FromArgb(0, 0, a);
            return Color.FromArgb(0, a, 0);
        }
    }
}
