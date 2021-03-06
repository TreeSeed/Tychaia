// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.Runtime
{
    public interface IChunkOctreeFactory
    {
        ChunkOctree<T> CreateChunkOctree<T>() where T : class, IChunk;
    }
}
