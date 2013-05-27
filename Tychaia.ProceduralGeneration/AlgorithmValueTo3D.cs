//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using Tychaia.ProceduralGeneration.Biomes;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Output)]
    [FlowDesignerName("Value To 3D")]
    public class AlgorithmValueTo3D : Algorithm<int, int>
    {
        [DataMember]
        [DefaultValue(64)]
        [Description("Estimate maximum value.")]
        public int EstimateMax
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(ColorScheme.Perlin)]
        [Description("The color scheme to use.")]
        public ColorScheme ColorSet
        {
            get;
            set;
        }

        public override string[] InputNames
        {
            get { return new string[] { "Value" }; }
        }

        public override bool Is2DOnly
        {
            get { return false; }
        }

        public AlgorithmValueTo3D()
        {
            this.EstimateMax = 64;
            this.ColorSet = ColorScheme.Perlin;
        }

        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            if (input[(i + ox) + (j + oy) * width + (0 + oz) * width * height] >= z)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = (int)z;
            else
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = Int32.MaxValue;
        }

        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (value == Int32.MaxValue)
                return Color.Transparent;

            if (ColorSet == ColorScheme.Land)
                return this.DelegateColorForValueToParent(parent, value);

            int a;

            double divvalue = (double)this.EstimateMax;

            if (divvalue > 255)
                divvalue = 255;
            else if (divvalue < 1)
                divvalue = 1;

            a = (int)(value * ((double)255 / divvalue));

            if (a < 0)
            {
                if (a < -255)
                {
                    if (ColorSet == ColorScheme.Land)
                        return Color.FromArgb(0, 0, 255);
                    else if (ColorSet == ColorScheme.Perlin)
                        return Color.FromArgb(255, 255, 255);

                    return Color.Red;
                }

                if (ColorSet == ColorScheme.Land)
                    return Color.FromArgb(0, 0, 255 - a);
                else if (ColorSet == ColorScheme.Perlin)
                    return Color.FromArgb(-a, -a, -a);
            }

            if (a > 255)
                a = 255;

            if (a == 0)
                return Color.Black;

            if (ColorSet == ColorScheme.Land)
                return Color.FromArgb(0, a, 0);
            else if (ColorSet == ColorScheme.Perlin)
                return Color.FromArgb(a, a, a);

            return Color.Red;
        }

        public enum ColorScheme
        {
            Land,
            Perlin,
        }
    }

}

