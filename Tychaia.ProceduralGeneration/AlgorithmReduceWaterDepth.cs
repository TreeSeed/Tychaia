//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.Specific)]
    [FlowDesignerCategory(FlowCategory.Terrain)]
    [FlowDesignerName("Reduce Water Depth")]
    public class AlgorithmReduceWaterDepth : Algorithm<int, int>
    {
        [DataMember]
        [DefaultValue(2)]
        [Description("The divisor for reduction of water depth.")]
        public int Divisor
        {
            get;
            set;
        }

        public override bool Is2DOnly
        {
            get { return true; }
        }

        public AlgorithmReduceWaterDepth()
        {
            this.Divisor = 1;
        }

        public override string[] InputNames
        {
            get
            {
                return new[] { "Input" };
            }
        }

        // Will be able to use this algorithm for:
        // Land - This is the equivelent of InitialLand
        // Towns - This is the equivelent of InitialTowns
        // Landmarks - We can spread landmarks over the world, which we can then use values to determine the size/value of the landmarks.
        // Monsters - By utilising multiple value scaling we can either distribute individual monsters or monster groups or even monster villages.
        // Tresure chests - Spreading tresure chests in dungeons (can be used as an estimated location then moved slightly too).
        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            if (input[(i + ox) + (j + oy) * width + (k + oz) * width * height] < 0)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] =
                    (int)Math.Floor(input[(i + ox) + (j + oy) * width + (k + oz) * width * height] / (double)(this.Divisor == 0 ? 1 : Math.Abs(this.Divisor)));
            else
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = input[(i + ox) + (j + oy) * width + (k + oz) * width * height];
        }


        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            var color = this.DelegateColorForValueToParent(parent, value);
            int b = color.B;
            if (b > 0)
                b = color.B - (byte)((255 - (int)color.B) * (this.Divisor - 1));
            b = Math.Min(b, 255);
            b = Math.Max(0, b);
            return Color.FromArgb(color.A, color.R, color.G, b);
        }
    }
}

