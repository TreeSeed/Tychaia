// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;

namespace Tychaia
{
    public interface IWorldFactory
    {
        TychaiaGameWorld CreateTychaiaGameWorld(GameState gameState, byte[] initialState, Action cleanup);
        PregenerateWorld CreatePregenerateWorld(ILevel level);
        ConnectWorld CreateConnectWorld();
    }
}
