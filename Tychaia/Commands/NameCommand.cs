// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Text;
using Protogame;
using Tychaia.Globals;
using Tychaia.Network;

namespace Tychaia
{
    public class NameCommand : ICommand
    {
        private readonly INetworkAPIProvider m_NetworkAPIProvider;

        private readonly IPersistentStorage m_PersistentStorage;

        public NameCommand(INetworkAPIProvider networkAPIProvider, IPersistentStorage persistentStorage)
        {
            this.m_NetworkAPIProvider = networkAPIProvider;
            this.m_PersistentStorage = persistentStorage;
        }

        public string[] Descriptions
        {
            get
            {
                return new[] { "Change your in-game name." };
            }
        }

        public string[] Names
        {
            get
            {
                return new[] { "name" };
            }
        }

        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            if (parameters.Length == 0)
            {
                return "usage: name <new name>";
            }

            var newName = parameters[0];

            this.m_PersistentStorage.Settings.Name = newName;

            if (this.m_NetworkAPIProvider.IsAvailable)
            {
                this.m_NetworkAPIProvider.GetAPI().SendMessage("change name", Encoding.ASCII.GetBytes(newName));
            }

            return "Your name has been changed";
        }
    }
}