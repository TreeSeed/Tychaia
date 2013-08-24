// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.Data;
using Tychaia.ProceduralGeneration;

namespace Tychaia
{
    public class DefaultFlowBundleToCellConverter : IFlowBundleToCellConverter
    {
        public Cell ConvertToCell(FlowBundle bundle)
        {
            return new Cell
            {
                BlockAssetName = bundle.Get("BlockInfo").BlockAssetName,
                HeightMap = bundle.Get("HeightMap")
            };
        }
    }
}

