// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.ProceduralGeneration
{
    public interface ICellOrderCalculator
    {
        int[] CalculateCellRenderOrder(int width, int height);
    }
}