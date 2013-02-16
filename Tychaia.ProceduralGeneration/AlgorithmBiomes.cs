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
    [FlowDesignerName("Initial Land")]
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

        // Will be able to use this algorithm for:
        // Land - This is the equivelent of InitialLand
        // Towns - This is the equivelent of InitialTowns
        // Landmarks - We can spread landmarks over the world, which we can then use values to determine the size/value of the landmarks.
        // Monsters - By utilising multiple value scaling we can either distribute individual monsters or monster groups or even monster villages.
        // Tresure chests - Spreading tresure chests in dungeons (can be used as an estimated location then moved slightly too).
        public override void ProcessCell(IRuntimeContext context, int[] inputA, int[] inputB, int[] inputC, Biome[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth)
        {
            output[i + j * width + k * width * height] = Biomes.BiomeEngine.GetBiomeForCell(inputA[i + j * width + k * width * height], inputB[i + j * width + k * width * height], inputC[i + j * width + k * width * height]);
        }
        
        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (value == -1)
                return Color.Black;
            else
                return BiomeEngine.Biomes[value].BrushColor;
        }
    }
}

