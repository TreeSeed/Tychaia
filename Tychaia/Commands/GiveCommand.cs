// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Linq;
using Protogame;
using Tychaia.Game;
using System;

namespace Tychaia
{
    public class GiveCommand : ICommand
    {
        public string[] Names { get { return new[] { "give" }; } }
        public string[] Descriptions
        {
            get
            {
                return new[]
                {
                    "Give the player a named item."
                };
            }
        }

        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            if (parameters.Length < 1)
                return "Not enough parameters (usage: give <name> [<weight>]).";

            var player = gameContext.World.Entities.OfType<PlayerEntity>().FirstOrDefault();
            if (player == null)
                return "Must be in-game to run this command.";
            Item item;
            if (parameters.Length == 1)
                item = new Item { Name = parameters[0] };
            else
            {
                try
                {
                    item = new WeightedItem
                    {
                        Name = parameters[0],
                        Weight = (Weight)Enum.Parse(typeof(Weight), parameters[1])
                    };
                }
                catch (ArgumentException)
                {
                    return "No such weighting exists.";
                }
            }
            player.RuntimeData.Inventory.Add(item);
            return "Added " + parameters[0] + " to player's inventory.";
        }
    }
}

