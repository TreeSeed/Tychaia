using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Generates secondary biomes based on input data.
    /// </summary>
    [DataContract]
    public class LayerSecondaryBiome : Layer2D
    {
        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the rainfall map.")]
        public int MinRainfall
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum integer value in the rainfall map.")]
        public int MaxRainfall
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the temperature map.")]
        public int MinTemperature
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum integer value in the temperature map.")]
        public int MaxTemperature
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the terrain map.")]
        public int MinTerrain
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(20)]
        [Description("The maximum integer value in the terrain map.")]
        public int MaxTerrain
        {
            get;
            set;
        }

        public LayerSecondaryBiome(Layer biome, Layer rainfall, Layer temperature, Layer terrain)
            : base(new Layer[] { biome, rainfall, temperature, terrain })
        {
            this.MinRainfall = 0;
            this.MaxRainfall = 100;
            this.MinTemperature = 0;
            this.MaxTemperature = 100;
            this.MinTerrain = 0;
            this.MaxTerrain = 20;
        }

        protected override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            if (this.Parents.Length < 4 || this.Parents[0] == null || this.Parents[1] == null || this.Parents[2] == null || this.Parents[3] == null)
                return new int[width * height];

            int[] biome = this.Parents[0].GenerateData(x, y, width, height);
            int[] rainfall = this.Parents[1].GenerateData(x, y, width, height);
            int[] temperature = this.Parents[2].GenerateData(x, y, width, height);
            int[] terrain = this.Parents[3].GenerateData(x, y, width, height);
            int[] data = new int[width * height];

            // Write out the secondary biomes.
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    try
                    {
                        // Normalize values.
                        int nbiome = biome[i + j * width];
                        double nrain = (rainfall[i + j * width] - this.MinRainfall) / (double)(this.MaxRainfall - this.MinRainfall);
                        double ntemp = (temperature[i + j * width] - this.MinTemperature) / (double)(this.MaxTemperature - this.MinTemperature);
                        double nterrain = (terrain[i + j * width] - this.MinTerrain) / (double)(this.MaxTerrain - this.MinTerrain);

                        // Store result.
                        data[i + j * width] = BiomeEngine.GetSecondaryBiomeForCell(nbiome, nrain, ntemp, nterrain);
                    }
                    catch (Exception)
                    {
                        // In case of overflow, underflow or divide by zero.
                        data[i + j * width] = 0;
                    }
                }

            return data;
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            return BiomeEngine.GetSecondaryBiomeBrushes();
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Biome", "Rainfall", "Temperature", "Terrain" };
        }

        public override string ToString()
        {
            return "Secondary Biomes";
        }
    }
}
