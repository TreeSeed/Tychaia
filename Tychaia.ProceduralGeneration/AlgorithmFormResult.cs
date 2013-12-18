// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Tychaia.Data;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Output)]
    [FlowDesignerName("Form Result")]
    public class AlgorithmFormResult : Algorithm<Cell, int, int, Cell>
    {
        public override string[] InputNames
        {
            get { return new[] { "Block Info", "Heightmap", "Edges" }; }
        }

        public override bool Is2DOnly
        {
            get { return false; }
        }

        public override bool[] InputIs2D
        {
            get { return new[] { false, true, false }; }
        }

        public override void ProcessCell(
            IRuntimeContext context,
            Cell[] blockInfo,
            int[] heightMap,
            int[] edges,
            Cell[] output,
            long x,
            long y,
            long z,
            int i,
            int j,
            int k,
            int width,
            int height,
            int depth,
            int ox,
            int oy,
            int oz)
        {
            // Block generation
            output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)].BlockAssetName =
                blockInfo[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)].BlockAssetName;
            output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)].HeightMap =
                heightMap[(i + ox) + ((j + oy) * width)] + (heightMap[(i + ox) + ((j + oy) * width)] < 0 ? 1 : 0);
            output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)].EdgeDetection =
                edges[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)];

            // Beings generation
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            unchecked
            {
                var u = 0;
                foreach (var v in ((object)value).GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Select(x => x.GetValue(value)))
                {
                    if (v != null)
                    {
                        u += v.GetHashCode();
                    }
                }

                return Color.FromArgb(u);
            }
        }
    }
}
