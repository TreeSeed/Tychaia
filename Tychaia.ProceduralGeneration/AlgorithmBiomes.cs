// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Biomes)]
    [FlowDesignerName("Biomes")]
    public class AlgorithmBiomes : Algorithm<int, int, int, Biome>
    {
        public override string[] InputNames
        {
            get { return new[] { "Rainfall", "Temperature", "Terrain" }; }
        }

        public override bool[] InputIs2D
        {
            get { return new[] { true, true, true }; }
        }

        public override bool Is2DOnly
        {
            get { return true; }
        }

        public override void ProcessCell(
            IRuntimeContext context,
            int[] inputA,
            int[] inputB,
            int[] inputC,
            Biome[] output,
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
            if (inputC[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] != 0)
                output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] =
                    BiomeEngine.GetBiomeForCell(
                        inputA[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)],
                        inputB[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)],
                        inputC[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)]);
            else if (inputC[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] == 0)
                output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] =
                    BiomeEngine.Biomes.First(v => v is WaterBiome);
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (value == null)
                return Color.Transparent;
            return value.BrushColor;
        }
    }
}
