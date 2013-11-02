// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia
{
    public class ServerCommand : ICommand
    {
        public string[] Names
        {
            get
            {
                return new[] { "server" };
            }
        }
        
        public string[] Descriptions
        {
            get
            {
                return new[]
                {
                    "Send an internal message to the server."
                };
            }
        }

        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            if (parameters.Length != 1)
                return "Not enough parameters (usage: server <message>).";

            var tychaiaWorld = gameContext.World as TychaiaGameWorld;
            if (tychaiaWorld == null)
                return "Must be in-game to run this command.";
            
            return tychaiaWorld.SendInternalServerMessage(parameters[0]);
        }
    }
}
