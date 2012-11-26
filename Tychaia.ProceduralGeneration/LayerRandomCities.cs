using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Generates city biomes based on input data.
    /// </summary>
    [DataContract]
    public class LayerRandomCities : Layer2D
    {
        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the soil fertility map.")]
        public int MinSoilFertility
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum integer value in the soil fertility map.")]
        public int MaxSoilFertility
        {
            get;
            set;
        }
                
        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the military strength map.")]
        public int MinMilitaryStrength
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum integer value in the military strength map.")]
        public int MaxMilitaryStrength
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the Ore Density map.")]
        public int MinOreDensity
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(20)]
        [Description("The maximum integer value in the Ore Density map.")]
        public int MaxOreDensity
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the rare ore density map.")]
        public int MinRareOreDensity
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(20)]
        [Description("The maximum integer value in the rare ore density map.")]
        public int MaxRareOreDensity
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The maximum integer value in the height map.")]
        public int MinHeight
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(20)]
        [Description("The maximum integer value in the height map.")]
        public int MaxHeight
        {
            get;
            set;
        }

        public LayerRandomCities(Layer secondarybiome, Layer soilfertility, Layer militarystrength, Layer oredensity, Layer rareoredensity, Layer terrain)
            : base(new Layer[] { secondarybiome, soilfertility, militarystrength, oredensity, rareoredensity, terrain })
        {
            this.MinSoilFertility = 0;
            this.MaxSoilFertility = 100;
            this.MinMilitaryStrength = 0;
            this.MaxMilitaryStrength = 100;
            this.MinOreDensity = 0;
            this.MaxOreDensity = 100;
            this.MinRareOreDensity = 0;
            this.MaxRareOreDensity = 20;
            this.MinHeight = 0;
            this.MaxHeight = 20;
        }

        protected override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            if (this.Parents.Length < 6 || this.Parents[0] == null || this.Parents[1] == null || this.Parents[2] == null || this.Parents[3] == null || this.Parents[4] == null || this.Parents[5] == null)
                return new int[width * height];

            int[] biome = this.Parents[0].GenerateData(x, y, width, height);
            int[] soilfertility = this.Parents[1].GenerateData(x, y, width, height);
            int[] militarystrength = this.Parents[2].GenerateData(x, y, width, height);
            int[] oredensity = this.Parents[3].GenerateData(x, y, width, height);
            int[] rareoredensity = this.Parents[4].GenerateData(x, y, width, height);
            int[] terrain = this.Parents[5].GenerateData(x, y, width, height);
            int[] data = new int[width * height];

            // Write out the secondary biomes.
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    try
                    {
                        if (biome[i + j * width] == 0)
                        {
                            data[1 + j * width] = 0;
                        }
                        else
                        {
                            // Normalize values.
                            // int nbiome = biome[i + j * width];
                            double nsoilfertility = (soilfertility[i + j * width] - this.MinSoilFertility) / (double)(this.MaxSoilFertility - this.MinSoilFertility);
                            double nmilitarystrength = (militarystrength[i + j * width] - this.MinMilitaryStrength) / (double)(this.MaxMilitaryStrength - this.MinMilitaryStrength);
                            double noredensity = (oredensity[i + j * width] - this.MinOreDensity) / (double)(this.MaxOreDensity - this.MinOreDensity);
                            double nrareoredensity = (rareoredensity[i + j * width] - this.MinRareOreDensity) / (double)(this.MaxRareOreDensity - this.MinRareOreDensity);
                            double nterrain = (terrain[i + j * width] - this.MinHeight) / (double)(this.MaxHeight - this.MinHeight);

                            // Store result.
                            data[i + j * width] = CitiesEngine.GetCityBiomeForCell(nsoilfertility, nmilitarystrength, noredensity, nrareoredensity, nterrain);
                        }
                    }
                    catch (Exception e)
                    {
                        // In case of overflow, underflow or divide by zero.
                        data[i + j * width] = 0;
                    }
                }

            return data;
        }

        public override Dictionary<int, System.Drawing.Brush> GetLayerColors()
        {
            return CitiesEngine.GetCityBiomeBrushes();
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Biome", "Soil Fertility", "Military Strength", "Ore Density", "Rare Ore Density", "Terrain" };
        }

        public override string ToString()
        {
            return "City Biomes";
        }
    }
}
