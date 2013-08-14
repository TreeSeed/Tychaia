// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Drawing;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    [DataContract(Name = "AlgorithmExtend")]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Zooming)]
    [FlowDesignerName("2D Extend Value")]
    public class AlgorithmExtend2D : Algorithm<int, int>
    {
        public override int[] RequiredXBorder
        {
            get { return new[] { 1 }; }
        }

        public override int[] RequiredYBorder
        {
            get { return new[] { 1 }; }
        }

        public override string[] InputNames
        {
            get { return new[] { "Input" }; }
        }

        public override bool Is2DOnly
        {
            get { return true; }
        }

        // Will be able to use this algorithm for:
        // Land - This is the equivelent of ExtendLand
        // Towns - This is the equivelent of ExtendTowns
        // Landmarks/Terrain - We can spread landmarks over the world, we can create mountain landmarks by spreading their size out then modifying the terrain.
        // Monsters - We can spread monster villages out, simmilar to towns.
        // Dungeons - We can use this to smooth dungeons out, so that there are less or more pointy bits. Alternatively we can also use this to spread out some side sections.
        // Anything else?
        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z,
            int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            var selected = AlgorithmUtility.GetRandomRange(context.Seed, x, y, 0, 8, context.Modifier);

            switch (selected)
            {
                case 0:
                    output[(i + ox) + (j + oy) * width + (k + oz) * width * height] =
                        input[((i + ox) + 1) + ((j + oy) + 1) * width + (k + oz) * width * height];
                    break;
                case 1:
                    output[(i + ox) + (j + oy) * width + (k + oz) * width * height] =
                        input[((i + ox) - 1) + ((j + oy) - 1) * width + (k + oz) * width * height];
                    break;
                case 2:
                    output[(i + ox) + (j + oy) * width + (k + oz) * width * height] =
                        input[((i + ox) - 1) + ((j + oy) + 1) * width + (k + oz) * width * height];
                    break;
                case 3:
                    output[(i + ox) + (j + oy) * width + (k + oz) * width * height] =
                        input[((i + ox) + 1) + ((j + oy) - 1) * width + (k + oz) * width * height];
                    break;
                case 4:
                    output[(i + ox) + (j + oy) * width + (k + oz) * width * height] =
                        input[((i + ox) + 0) + ((j + oy) + 1) * width + (k + oz) * width * height];
                    break;
                case 5:
                    output[(i + ox) + (j + oy) * width + (k + oz) * width * height] =
                        input[((i + ox) + 1) + ((j + oy) + 0) * width + (k + oz) * width * height];
                    break;
                case 6:
                    output[(i + ox) + (j + oy) * width + (k + oz) * width * height] =
                        input[((i + ox) + 0) + ((j + oy) - 1) * width + (k + oz) * width * height];
                    break;
                case 7:
                    output[(i + ox) + (j + oy) * width + (k + oz) * width * height] =
                        input[((i + ox) - 1) + ((j + oy) + 0) * width + (k + oz) * width * height];
                    break;
            }
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
