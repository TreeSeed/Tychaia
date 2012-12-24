using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Replaces cells in one layer with the input from another layer.
    /// </summary>
    [DataContract()]
    [FlowDesignerCategory(FlowCategory.General)]
    [FlowDesignerName("Replace Values")]
    public class LayerReplace : Layer2D
    {
        [DataMember]
        [Description("The value to find and replace in the input.")]
        public double Find
        {
            get;
            set;
        }

        [DataMember]
        [Description("The type of replacement to perform.")]
        public ReplaceMode2D ReplaceMode
        {
            get;
            set;
        }

        public LayerReplace(Layer input, Layer replace)
            : base(new Layer[] { input, replace })
        {
        }

        protected override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            if (this.Parents.Length < 2 || this.Parents[0] == null || this.Parents[1] == null)
                return new int[width * height];

            long ox = 1;
            long oy = 1;
            long rw = width + 2;
            long rh = height + 2;
            int[] input = this.Parents[0].GenerateData(x - ox, y - oy, rw, rh);
            int[] replace = this.Parents[1].GenerateData(x, y, width, height);
            int[] data = new int[width * height];

            // Copy and replace data.
            for (long i = 0; i < width; i++)
                for (long j = 0; j < height; j++)
                    if (input[(i + ox) + (j + oy) * rw] == this.Find)
                        this.PerformReplace(data, replace, input, i, j, width, height, ox, oy, rw, rh);
                    else
                        data[i + j * width] = input[(i + ox) + (j + oy) * rw];

            return data;
        }

        private void PerformReplace(int[] data, int[] replace, int[] input, long i, long j, long width, long height, long ox, long oy, long rw, long rh)
        {
            switch (this.ReplaceMode)
            {
                case ReplaceMode2D.All:
                    data[i + j * width] = replace[i + j * width];
                    break;
            }
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            if (this.Parents.Length < 2 || this.Parents[0] == null || this.Parents[1] == null)
                return null;
            else
                return this.Parents[1].GetLayerColors();
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Input", "Replace" };
        }

        public override string ToString()
        {
            return "Replace";
        }
    }

    public enum ReplaceMode2D
    {
        All
    }
}
