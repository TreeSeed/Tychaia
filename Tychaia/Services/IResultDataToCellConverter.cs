// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.Data;
using Tychaia.ProceduralGeneration;

namespace Tychaia
{
    public interface IResultDataToCellConverter
    {
        Cell ConvertToCell(ResultData resultData);
    }
}
