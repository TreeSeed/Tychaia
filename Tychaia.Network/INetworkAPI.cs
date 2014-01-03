// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Protogame;

namespace Tychaia.Network
{
    public interface INetworkAPI
    {
        string[] PlayersInGame { get; }

        void ListenForMessage(string type, Action<MxClient, byte[]> callback);

        void StopListeningForMessage(string type);

        void SendMessage(string type, byte[] data, MxClient client = null, bool reliable = false);

        void Update();
    }
}