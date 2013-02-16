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

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerCategory(FlowCategory.Land)]
    [FlowDesignerName("Biomes")]
    public class AlgorithmBiomes : Algorithm<int, int, int, Biome>
    {
        public override string[] InputNames
        {
            get { return new string[] { "Rainfall", "Temperature", "Terrain" }; }
        }

        public override bool Is2DOnly
        {
            get { return true; }
        }
        
        public AlgorithmBiomes()
        {
        }
        public override void ProcessCell(IRuntimeContext context, int[] inputA, int[] inputB, int[] inputC, Biome[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth)
        {
			if (inputC[i + j * width + k * width * height] != 0)
            	output[i + j * width + k * width * height] = Biomes.BiomeEngine.GetBiomeForCell(inputA[i + j * width + k * width * height], inputB[i + j * width + k * width * height], inputC[i + j * width + k * width * height]);
        	else
				output[i + j * width + k * width * height] = null;
		}
        
        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (value == null)
                return Color.Black;
            else
                return value.BrushColor;
        }
    }
}

