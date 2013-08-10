// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using Tychaia.Disk;

namespace Tychaia
{
    public interface IChunkFactory
    {
        Chunk CreateChunk(
            ILevel level,
            ChunkOctree octree,
            long x,
            long y,
            long z);
    }
}