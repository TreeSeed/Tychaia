using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Accepts a terrain input layer
    /// Outputs the change of height both X + Y
    /// </summary>

    [DataContract]
    public class LayerHeightChange : Layer2D
    {
        [DataMember]
        [DefaultValue(true)]
        [Description("True = X, False = y")]
        public bool XorY
        {
            get;
            set;
        }

        public LayerHeightChange(Layer terrain)
            : base(new Layer[] { terrain })
        {
            this.XorY = true;
        }

        protected override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            int ox = 1;
            int oy = 1;
            int rw = width / 2 + ox * 2;
            int rh = height / 2 + oy * 2;

            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height];

            int[] terrain = this.Parents[0].GenerateData(x, y, width, height);
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
                    if (this.XorY = false)
                        data[i + j * width] = terrain[i - ox + (j - oy) * rw] - terrain[i - ox + (j - oy + 1) * rw];
                    else
                        data[i + j * width] = terrain[i - ox + (j - oy) * rw] - terrain[i - ox + 1 + (j - oy) * rw];
                        

            return data;
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            return LayerColors.GetTerrainBrushes(20);
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Terrain" };
        }

        public override string ToString()
        {
            return "Height Change";
        }
    }
}
