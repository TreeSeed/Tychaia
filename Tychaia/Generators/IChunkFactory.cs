using System;
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

