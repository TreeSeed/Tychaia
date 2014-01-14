// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Protogame;

namespace Tychaia.Network
{
    public interface IServerFactory
    {
        ServerClientManager CreateServerClientManager(
            TychaiaServerWorld world,
            TychaiaServer server,
            int uniqueID,
            string initialPlayerName,
            MxClient client);
            
        ClientChunkStateManager CreateClientChunkStateManager(
            MxClient client);
    }
}
