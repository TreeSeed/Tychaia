using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Generates a layer that indicates viability for towns.
    /// </summary>
    [DataContract]
    public class LayerDeriveViability : Layer2D
    {
        public LayerDeriveViability(Layer soil, Layer animal, Layer ore, Layer terrain, Layer secondaryBiomes)
            : base(new Layer[] { soil, animal, ore, terrain, secondaryBiomes })
        {
        }

        protected override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            if (this.Parents.Length < 5 || this.Parents[0] == null || this.Parents[1] == null ||
                this.Parents[2] == null || this.Parents[3] == null || this.Parents[4] == null)
                return new int[width * height];

            int[] soil = this.Parents[0].GenerateData(x, y, width, height);
            int[] animal = this.Parents[1].GenerateData(x, y, width, height);
            int[] ore = this.Parents[2].GenerateData(x, y, width, height);
            int[] terrain = this.Parents[3].GenerateData(x, y, width, height);
            int[] data = new int[width * height];

            // Copy 1-for-1 the water cells.
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    // Use point system to evaluate viability.
                    int points = 0;

                    // Points for soil density.
                    if (soil[i + j * width] >= 50)
                        points++;
                    if (soil[i + j * width] >= 80)
                        points++;

                    // Points for animal density.
                    if (animal[i + j * width] >= 50)
                        points++;
                    if (animal[i + j * width] >= 80)
                        points++;

                    // Points for ore distribution.
                    if (ore[i + j * width] >= 3)
                        points++;
                    if (ore[i + j * width] >= 8)
                        points++;
                    if (ore[i + j * width] >= 14)
                        points++;

                    // Points for distance to water.
                    if (terrain[i + j * width] <= 10)
                        points++;
                    if (terrain[i + j * width] <= 5)
                        points++;
                    if (terrain[i + j * width] <= 2)
                        points++;

                    // Points for suitable secondary biomes.
                    /*
                    double suitability = BiomeEngine.GetSecondaryBiomeTownSuitability(secondaryBiomes[i + j * width]);
                    if (suitability >= 0.3)
                        points++;
                    if (suitability >= 0.6)
                        points++;
                    if (suitability >= 0.9)
                        points++;
                    */

                    // If water, no points.
                    if (terrain[i + j * width] == 0)
                        points = 0;

                    // Store points in result.
                    data[i + j * width] = Math.Min(10, points);
                }

            return data;
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            return LayerColors.GetGradientBrushes(0, 10);
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Soil Density", "Animal Density", "Ore Distribution", "Terrain", "Secondary Biomes" };
        }

        public override string ToString()
        {
            return "Derive Viability";
        }
    }
}
