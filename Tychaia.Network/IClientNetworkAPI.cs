// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.Network
{
    public interface IClientNetworkAPI : INetworkAPI
    {
        double DisconnectingForSeconds
        {
            get;
        }

        bool IsDisconnected { get; }

        bool IsPotentiallyDisconnecting { get; }
    }
}