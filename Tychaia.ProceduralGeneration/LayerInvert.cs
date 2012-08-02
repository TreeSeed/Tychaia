using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Inverts the values in the layer.
    /// </summary>
    [DataContract()]
    public class LayerInvert : Layer2D
    {
        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum value in the input range.")]
        public int MinRange
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum value in the input range.")]
        public int MaxRange
        {
            get;
            set;
        }

        public LayerInvert(Layer parent)
            : base(parent)
        {
            this.MinRange = 0;
            this.MaxRange = 100;
        }

        public override int[] GenerateData(int x, int y, int width, int height)
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height];

            int[] parent = this.Parents[0].GenerateData(x, y, width, height);
            int[] data = new int[width * height];

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    data[i + j * width] = (this.MaxRange - (parent[i + j * width] - this.MinRange)) + this.MinRange;

            return data;
        }

        public override Dictionary<int, System.Drawing.Brush> GetLayerColors()
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return null;
            else
                return this.Parents[0].GetLayerColors();
        }

        public override string ToString()
        {
            return "Invert";
        }
    }
}
