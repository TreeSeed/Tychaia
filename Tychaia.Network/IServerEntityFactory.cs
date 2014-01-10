// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.Network
{
    public interface IServerEntityFactory
    {
        EnemyServerEntity CreateEnemyServerEntity(TychaiaServer server, TychaiaServerWorld serverWorld, int uniqueEnemyIdentifier);
        PlayerServerEntity CreatePlayerServerEntity(TychaiaServer server, TychaiaServerWorld serverWorld, MxClient client, int uniqueClientIdentifier);
    }
}