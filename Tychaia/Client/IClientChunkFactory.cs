using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tychaia.Runtime;

namespace Tychaia.Client
{
    public interface IClientChunkFactory
    {
        ClientChunk CreateClientChunk(ChunkOctree<ClientChunk> octree, long x, long y, long z);
    }
}
