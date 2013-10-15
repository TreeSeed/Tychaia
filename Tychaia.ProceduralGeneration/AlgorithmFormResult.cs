// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Drawing;
using System.Runtime.Serialization;
using Tychaia.Data;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.Undefined)]
    [FlowDesignerCategory(FlowCategory.General)]
    [FlowDesignerName("Form Result")]
    public class AlgorithmFormResult : Algorithm<Cell, int, Cell>
    {
        public override string[] InputNames
        {
            get { return new[] { "Block Info", "Heightmap" }; }
        }

        public override bool Is2DOnly
        {
            get { return false; }
        }

        public override bool[] InputIs2D
        {
            get { return new[] { false, true }; }
        }

        public override void ProcessCell(
            IRuntimeContext context,
            Cell[] blockInfo,
            int[] heightMap,
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
            output[i + ox + ((j + oy) * width) + ((k + oz) * width * height)].BlockAssetName =
                blockInfo[i + ox + ((j + oy) * width) + ((k + oz) * width * height)].BlockAssetName;
            output[i + ox + ((j + oy) * width) + ((k + oz) * width * height)].HeightMap =
                heightMap[i + ox + ((j + oy) * width)];
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            // TODO: Combine colours like the flow bundles did.
            return Color.White;
        }
    }
}
