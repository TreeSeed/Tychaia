// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;

namespace Tychaia.Network
{
    public interface INetworkAPI
    {
        void ListenForMessage(string type, Action<string> callback);

        void SendMessage(string type, string data);
    }
}