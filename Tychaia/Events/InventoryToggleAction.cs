// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia
{
    public class InventoryToggleAction : IEventEntityAction<InventoryUIEntity>
    {
        public void Handle(InventoryUIEntity entity, Event @event)
        {
            entity.ToggleRight();
        }
    }
}

