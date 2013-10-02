// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Linq;
using Protogame;
using Tychaia.Game;

namespace Tychaia
{
    public class EquipCommand : ICommand
    {
        public string[] Names
        {
            get
            {
                return new[] { "equip" };
            }
        }
        
        public string[] Descriptions
        {
            get
            {
                return new[]
                {
                    "Equip an item in the player's inventory."
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
            var inventory = player.RuntimeData.Inventory;

            var item = inventory.AllItems.FirstOrDefault(x => x.Name == parameters[0]);
            if (item == null)
                return "No such item.";
            if (inventory.Equip(item))
                return "Item equipped.";
            return "Unable to equip item.";
        }
    }
}
