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
    public class Layer3DReplace : Layer3D
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
        public ReplaceMode3D ReplaceMode
        {
            get;
            set;
        }

        public Layer3DReplace(Layer input, Layer replace)
            : base(new Layer[] { input, replace })
        {
        }

        protected override int[] GenerateDataImpl(long x, long y, long z, long width, long height, long depth)
        {
            if (this.Parents.Length < 2 || this.Parents[0] == null || this.Parents[1] == null)
                return new int[width * height * depth];

            long ox = 1;
            long oy = 1;
            long oz = 1;
            long rw = width + 2;
            long rh = height + 2;
            long rd = depth + 2;
            int[] input = this.Parents[0].GenerateData(x - ox, y - oy, z - oz, rw, rh, rd);
            int[] replace = this.Parents[1].GenerateData(x, y, z, width, height, depth);
            int[] data = new int[width * height * depth];

            // Copy and replace data.
            for (long i = 0; i < width; i++)
                for (long j = 0; j < height; j++)
                    for (long k = 0; k < depth; k++)
                        if (input[(i + ox) + (j + oy) * rw + (k + oz) * rw * rh] == this.Find)
                            this.PerformReplace(data, replace, input, i, j, k, width, height, depth, ox, oy, oz, rw, rh, rd);
                        else
                            data[i + j * width + k * width * height] = input[(i + ox) + (j + oy) * rw + (k + oz) * rw * rh];

            return data;
        }

        private void PerformReplace(int[] data, int[] replace, int[] input, long i, long j, long k, long width, long height, long depth, long ox, long oy, long oz, long rw, long rh, long rd)
        {
            switch (this.ReplaceMode)
            {
                case ReplaceMode3D.All:
                    data[i + j * width + k * width * height] = replace[i + j * width + k * width * height];
                    break;
                case ReplaceMode3D.Surface:
                    if (input[(i + ox) + (j + oy) * rw + (k + oz + 1) * rw * rh] == -1)
                        data[i + j * width + k * width * height] = replace[i + j * width + k * width * height];
                    break;
                case ReplaceMode3D.Beneath:
                    if (input[(i + ox) + (j + oy) * rw + (k + oz + 1) * rw * rh] != -1)
                        data[i + j * width + k * width * height] = replace[i + j * width + k * width * height];
                    break;
            }
        } 

        public override Dictionary<int, System.Drawing.Brush> GetLayerColors()
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
            return "Replace 3D";
        }

        public override bool[] GetParents3DRequired()
        {
            return new bool[] { true, true };
        }
    }

    public enum ReplaceMode3D
    {
        All,
        Surface,
        Beneath
    }
}
