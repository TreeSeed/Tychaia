// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.Globals
{
    public interface IPositionScaleTranslation
    {
        /// <summary>
        /// Translates a position in the pixel address space into a position octree address
        /// space.
        /// </summary>
        long Translate(long pixelPosition);
    }
}
