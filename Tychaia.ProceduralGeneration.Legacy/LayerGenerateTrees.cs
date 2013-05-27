//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using Tychaia.ProceduralGeneration.Biomes;
using System.Linq;
using System.Collections.Generic;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Generates a layer of initial procedural generation data where each cell
    /// indicates either landmass or ocean.
    /// </summary>
    [DataContract]
    [FlowDesignerCategory(FlowCategory.Trees)]
    [FlowDesignerName("Generate Trees")]
    public class LayerGenerateTrees : Layer2D
    {
        public LayerGenerateTrees(Layer2D biomes)
            : base(biomes)
        {
        }

        protected override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height];

            int[] data = new int[width * height];
            int[] parent = this.Parents[0].GenerateData(x, y, width, height);

            for (int a = 0; a < width; a++)
                for (int b = 0; b < height; b++)
                {
                    // Get the tree population value.
                    var chance = 0.0;
                    if (parent[a + b * width] < 0 || parent[a + b * width] >= BiomeEngine.SecondaryBiomes.Count)
                        chance = 0.0;
                    else
                    {
                        var sb = BiomeEngine.SecondaryBiomes[parent[a + b * width]];
                        if (sb == null)
                            chance = 0.0;
                        else
                            chance = sb.TreeChance;
                    }

                    // Apply the population value.
                    if (this.GetRandomDouble(x + a, y + b, 0) < chance)
                        data[a + b * width] = 1;
                    else
                        data[a + b * width] = 0;
                }

            return data;
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            return LayerColors.TreeBrushes;
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Secondary Biomes" };
        }

        public override string ToString()
        {
            return "Generate Trees";
        }
    }
}

