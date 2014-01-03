// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.Runtime
{
    public interface ITerrainSurfaceCalculator
    {
        float? GetSurfaceY<T>(ChunkOctree<T> octree, float x, float y) where T : class, IChunk;
    }
}