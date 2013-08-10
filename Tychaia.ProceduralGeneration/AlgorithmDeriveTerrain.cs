// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.Undefined)]
    [FlowDesignerCategory(FlowCategory.Buggy)]
    [FlowDesignerName("Derive Terrain")]
    public class AlgorithmDeriveTerrain : Algorithm<int, int, int, int>
    {
        public AlgorithmDeriveTerrain()
        {
            this.CheckDiagonals = true;
        }

        public override int[] RequiredXBorder
        {
            get
            {
                return new[]
                {
                    1,
                    1,
                    1
                };
            }
        }

        public override int[] RequiredYBorder
        {
            get
            {
                return new[]
                {
                    1,
                    1,
                    1
                };
            }
        }

        [DataMember]
        [DefaultValue(true)]
        [Description("Whether diagonals to cells will be evaluated for their height.")]
        public bool CheckDiagonals { get; set; }

        public override string[] InputNames
        {
            get { return new[] { "Previous Terrain", "New Land", "Terrain Modifier" }; }
        }

        public override bool Is2DOnly
        {
            get { return true; }
        }

        public override void ProcessCell(IRuntimeContext context, int[] inputA, int[] inputB, int[] inputC, int[] output,
            long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            if (inputB[i + ox + (j + oy) * width + (k + oz) * width * height] != 0)
            {
                if (inputA[i + ox + (j + oy) * width + (k + oz) * width * height] == 0)
                    output[i + ox + (j + oy) * width + (k + oz) * width * height] = 1;
                else
                    output[i + ox + (j + oy) * width + (k + oz) * width * height] =
                        inputA[i + ox + (j + oy) * width + (k + oz) * width * height];

                if (inputB[i + 1 + ox + (j + oy) * width + (k + oz) * width * height] != 0 &&
                    inputB[i - 1 + ox + (j + oy) * width + (k + oz) * width * height] != 0 &&
                    inputB[i + ox + (j + 1 + oy) * width + (k + oz) * width * height] != 0 &&
                    inputB[i + ox + (j - 1 + oy) * width + (k + oz) * width * height] != 0)
                {
                    if (this.CheckDiagonals &&
                        inputB[i + 1 + ox + (j + 1 + oy) * width + (k + oz) * width * height] != 0 &&
                        inputB[i + 1 + ox + (j - 1 + oy) * width + (k + oz) * width * height] != 0 &&
                        inputB[i - 1 + ox + (j + 1 + oy) * width + (k + oz) * width * height] != 0 &&
                        inputB[i - 1 + ox + (j - 1 + oy) * width + (k + oz) * width * height] != 0)
                    {
                        output[i + ox + (j + oy) * width + (k + oz) * width * height] += 1;
                    }
                    else
                    {
                        output[i + ox + (j + oy) * width + (k + oz) * width * height] += 1;
                    }
                }

                var add = inputC[i + ox + (j + oy) * width + (k + oz) * width * height];

                while (add > 50)
                {
                    output[i + ox + (j + oy) * width + (k + oz) * width * height] += 1;
                    add -= 25;
                }
            }
            else
            {
                output[i + ox + (j + oy) * width + (k + oz) * width * height] = 0;
            }
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (value == 0)
                return Color.FromArgb(0, 0, 255);

            int a = value * 20;

            if (a > 255)
                a = 255;

            //int a = Math.Min(Math.Max(value, 0), 255);
            return Color.FromArgb(a, a, a);
        }
    }
}