// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.Game;

namespace Tychaia
{
    public interface IEntityFactory
    {
        PlayerEntity CreatePlayerEntity(Player runtimeData);
    }
}