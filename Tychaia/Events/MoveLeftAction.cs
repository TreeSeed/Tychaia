// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia
{
    public class MoveLeftAction : IEventEntityAction<PlayerEntity>
    {
        public void Handle(IGameContext context, PlayerEntity entity, Event @event)
        {
            entity.MoveInDirection(context, 270);
        }
    }
}
