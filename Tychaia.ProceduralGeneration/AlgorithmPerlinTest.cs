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
    [FlowDesignerCategory(FlowCategory.BaseLayers)]
    [FlowDesignerName("Initial Perlin")]
    public class AlgorithmPerlinTest : Algorithm<int>
    {
        [DataMember]
        [DefaultValue(1)]
        [Description("The scale of the perlin noise map.")]
        public int Scale
        {
            get;
            set;
        }

        public override bool Is2DOnly
        {
            get { return false; }
        }
        
        public AlgorithmPerlinTest()
        {
            this.Scale = 1;
        }

        // Will be able to use this algorithm for:
        // Land - This is the equivelent of InitialLand
        // Towns - This is the equivelent of InitialTowns
        // Landmarks - We can spread landmarks over the world, which we can then use values to determine the size/value of the landmarks.
        // Monsters - By utilising multiple value scaling we can either distribute individual monsters or monster groups or even monster villages.
        // Tresure chests - Spreading tresure chests in dungeons (can be used as an estimated location then moved slightly too).
        public override void ProcessCell(IRuntimeContext context, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            double result = 0;
            double b = 16;

            if (Scale < 1)
                Scale = 1;

            for (int a = 1; a <= b; a *= 2)
                result += (context.GetRandomDouble(x / (a * Scale), y / (a * Scale), z / (a * Scale), context.Modifier) / (double)(b / a));

            result /= 1.96875;

            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = (int)((result) * 100);
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return Color.FromArgb((int)(255 * (value / 100f)), (int)(255 * (value / 100f)), (int)(255 * (value / 100f)));
        }
    }
}

