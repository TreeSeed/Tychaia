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
    [FlowDesignerCategory(FlowCategory.Land)]
    [FlowDesignerName("Determine Height Change")]
    public class LayerHeightChange : Layer2D
    {
        [DataMember]
        [DefaultValue(true)]
        [Description("True = X, False = Y")]
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
            long rw = width + 1;
            long rh = height + 1;

            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height];

            int[] terrain = this.Parents[0].GenerateData(x, y, rw, rh);
            int[] data = new int[width * height];

            // Multiply existing terrain data with the value in the perlin map.
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (this.XorY == false)
                        data[i + j * width] = terrain[i + j * rw] - terrain[i + (j + 1) * rw];
                    else
                        data[i + j * width] = terrain[i + j * rw] - terrain[i + 1 + j * rw];

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
