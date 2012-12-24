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
    /// reduce the number of mountains that exist.
    /// </summary>
    [DataContract]
    [FlowDesignerCategory(FlowCategory.Land)]
    [FlowDesignerName("Terrain Mixdown")]
    public class LayerTerrainMixdown : Layer2D
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

        public LayerTerrainMixdown(Layer terrain, Layer perlin)
            : base(new Layer[] { terrain, perlin })
        {
            this.MinPerlin = 0;
            this.MaxPerlin = 100;
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
                        if (this.MaxPerlin != this.MinPerlin)
                            factor = (perlin[i + j * width] - this.MinPerlin) / (double)(this.MaxPerlin - this.MinPerlin);
                        data[i + j * width] = (int)(terrain[i + j * width] * factor);
                        if (data[i + j * width] <= 0)
                            data[i + j * width] = 1;
                    }

            return data;
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            return LayerColors.GetTerrainBrushes(20);
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Terrain", "Perlin" };
        }

        public override string ToString()
        {
            return "Terrain Mixdown";
        }
    }
}
