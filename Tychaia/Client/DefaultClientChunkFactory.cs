// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.Network;
using Tychaia.Runtime;

namespace Tychaia.Client
{
    public class DefaultClientChunkFactory : IClientChunkFactory
    {
        private readonly INetworkAPI m_NetworkAPI;

        public DefaultClientChunkFactory(INetworkAPI networkAPI)
        {
            this.m_NetworkAPI = networkAPI;
        }

        public ClientChunk CreateClientChunk(ChunkOctree<ClientChunk> octree, long x, long y, long z)
        {
            var existing = octree.Get(x, y, z);

            if (existing != null)
            {
                return existing;
            }

            var @new = new ClientChunk(x, y, z);
            octree.Set(@new);

            this.m_NetworkAPI.SendMessage(
                "require chunk", 
                InMemorySerializer.Serialize(new ChunkRequest { X = x, Y = y, Z = z }),
                reliable: true);

            return @new;
        }
    }
}