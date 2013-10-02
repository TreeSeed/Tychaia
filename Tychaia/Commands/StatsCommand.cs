// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Linq;
using Protogame;
using Tychaia.Game;

namespace Tychaia
{
    public class StatsCommand : ICommand
    {
        public string[] Names
        {
            get
            {
                return new[]
                {
                    "stats"
                };
            }
        }
        
        public string[] Descriptions
        {
            get
            {
                return new[]
                {
                    "Modifies the stats of the player."
                };
            }
        }

        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            if (parameters.Length < 1)
                return "Not enough parameters.";

            var player = gameContext.World.Entities.OfType<PlayerEntity>().FirstOrDefault();
            if (player == null)
                return "Must be in-game to run this command.";

            switch (parameters[0].ToLower())
            {
                case "help":
                    return @"health <current> [<max>] - Set current and (optionally) maximum health.
stamina <current> [<max>] - Set current and (optionally) maximum stamina.
mana <current> [<max>] - Set current and (optionally) maximum mana.";
                case "health":
                    if (parameters.Length < 2 || parameters.Length > 3)
                        return "Incorrect parameters.";
                    player.RuntimeData.Health = Convert.ToInt32(parameters[1]);
                    if (parameters.Length >= 3)
                        player.RuntimeData.MaxHealth = Convert.ToInt32(parameters[2]);
                    return "Health updated.";
                case "stamina":
                    if (parameters.Length < 2 || parameters.Length > 3)
                        return "Incorrect parameters.";
                    player.RuntimeData.Stamina = Convert.ToInt32(parameters[1]);
                    if (parameters.Length >= 3)
                        player.RuntimeData.MaxStamina = Convert.ToInt32(parameters[2]);
                    return "Health updated.";
                case "mana":
                    if (parameters.Length < 2 || parameters.Length > 3)
                        return "Incorrect parameters.";
                    player.RuntimeData.Mana = Convert.ToInt32(parameters[1]);
                    if (parameters.Length >= 3)
                        player.RuntimeData.MaxMana = Convert.ToInt32(parameters[2]);
                    return "Health updated.";
                default:
                    return "Unknown command (try `help`).";
            }
        }
    }
}
