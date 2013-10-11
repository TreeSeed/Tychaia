// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.Data;
using Tychaia.Globals;
using Tychaia.ProceduralGeneration;

namespace Tychaia
{
    [NoProfile]
    public interface IResultDataToCellConverter
    {
        Cell ConvertToCell(ResultData resultData);
    }
}
