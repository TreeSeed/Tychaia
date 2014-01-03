// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.Runtime;

namespace Tychaia.Client
{
    public interface IClientChunkFactory
    {
        ClientChunk CreateClientChunk(ChunkOctree<ClientChunk> octree, long x, long y, long z);
    }
}