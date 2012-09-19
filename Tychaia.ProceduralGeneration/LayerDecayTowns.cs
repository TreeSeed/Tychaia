using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Accepts a viability map and a town map and combines them to produce ruins.
    /// </summary>
    [DataContract]
    public class LayerDecayTowns : Layer2D
    {
        [DataMember]
        [DefaultValue(5)]
        [Description("The number of points required on the viability map to place a town.")]
        public int ViabilityPoints
        {
            get;
            set;
        }

        public LayerDecayTowns(Layer viability, Layer towns)
            : base(new Layer[] { viability, towns })
        {
            this.ViabilityPoints = 5;
        }

        protected override int[] GenerateDataImpl(int x, int y, int width, int height)
        {
            if (this.Parents.Length < 2 || this.Parents[0] == null || this.Parents[1] == null)
                return new int[width * height];

            int[] viability = this.Parents[0].GenerateData(x, y, width, height);
            int[] towns = this.Parents[1].GenerateData(x, y, width, height);
            int[] data = new int[width * height];

            // Decay towns if not meeting viability requirement.
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if (towns[i + j * width] == 0)
                    {
                        data[i + j * width] = 0;
                        continue;
                    }

                    if (viability[i + j * width] > this.ViabilityPoints)
                        data[i + j * width] = 1;
                    else
                        data[i + j * width] = 2;
                }

            return data;
        }

        public override Dictionary<int, System.Drawing.Brush> GetLayerColors()
        {
            return LayerColors.TownBrushes;
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Viability", "Towns" };
        }

        public override string ToString()
        {
            return "Decay Towns";
        }
    }
}
