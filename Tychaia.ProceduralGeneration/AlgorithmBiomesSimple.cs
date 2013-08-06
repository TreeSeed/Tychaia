//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Drawing;
using Tychaia.ProceduralGeneration.Biomes;
using System.Linq;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Biomes)]
    [FlowDesignerName("Simple Biomes")]
    public class AlgorithmBiomesSimple : Algorithm<int, Biome>
    {
        public override string[] InputNames
        {
            get { return new string[] { "Terrain" }; }
        }

        public override bool Is2DOnly
        {
            get { return true; }
        }

        public override void ProcessCell(IRuntimeContext context, int[] input, Biome[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            if (input[(i + ox) + (j + oy) * width + (k + oz) * width * height] >= 0)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = BiomeEngine.Biomes[AlgorithmUtility.GetRandomRange(context.Seed, x, y, 0, BiomeEngine.Biomes.Count)];
            else
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = Biomes.BiomeEngine.Biomes.First(v => v is WaterBiome);
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (value == null)
                return Color.Transparent;
            else
                return value.BrushColor;
        }
    }
}

