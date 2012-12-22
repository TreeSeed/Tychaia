using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Accepts a terrain input layer and a perlin noise map and combines the two to
    /// create an ore distribution field.
    /// </summary>
    [DataContract]
    public class LayerOreMixdown : Layer2D
    {
        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the perlin noise map.")]
        public int MinPerlin
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum integer value in the perlin noise map.")]
        public int MaxPerlin
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

        [DataMember]
        [DefaultValue(20)]
        [Description("The maximum integer value in the output ore distribution map.")]
        public int MaxOre
        {
            get;
            set;
        }

        public LayerOreMixdown(Layer terrain, Layer perlin)
            : base(new Layer[] { terrain, perlin })
        {
            this.MinPerlin = 0;
            this.MaxPerlin = 100;
            this.MaxTerrain = 20;
            this.MaxOre = 20;
        }

        protected override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            if (this.Parents.Length < 2 || this.Parents[0] == null || this.Parents[1] == null)
                return new int[width * height];

            int[] terrain = this.Parents[0].GenerateData(x, y, width, height);
            int[] perlin = this.Parents[1].GenerateData(x, y, width, height);
            int[] data = new int[width * height];

            // Copy 1-for-1 the water cells.
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (terrain[i + j * width] == 0)
                        data[i + j * width] = 0;
                    else
                        data[i + j * width] = -1;

            // Multiply existing terrain data with the value in the perlin map.
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (data[i + j * width] == -1)
                    {
                        double factor = 1;
                        double terFactor = 0;
                        if (this.MaxPerlin != this.MinPerlin)
                            factor = (perlin[i + j * width] - this.MinPerlin) / (double)(this.MaxPerlin - this.MinPerlin);
                        if (this.MaxTerrain != 0)
                            terFactor = terrain[i + j * width] / (double)this.MaxTerrain;
                        data[i + j * width] = (int)Math.Round(terFactor * factor * this.MaxOre);
                        if (data[i + j * width] <= 0)
                            data[i + j * width] = 0;
                    }

            return data;
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            return LayerColors.GetGradientBrushes(0, this.MaxTerrain);
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Terrain", "Voronoi Mixdown" };
        }

        public override string ToString()
        {
            return "Ore Mixdown";
        }
    }
}
