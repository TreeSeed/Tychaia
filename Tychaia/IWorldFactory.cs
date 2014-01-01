// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Net;

namespace Tychaia
{
    public interface IWorldFactory
    {
        TychaiaGameWorld CreateTychaiaGameWorld(int uniqueClientIdentifier, Action cleanup);
        PregenerateWorld CreatePregenerateWorld(ILevel level);
        ConnectWorld CreateConnectWorld(bool startServer, IPAddress address, int port);

        MultiplayerWorld CreateMultiplayerWorld();
    }
}
