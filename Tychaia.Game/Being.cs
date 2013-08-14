// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.Game
{
    public class Being : ChunkEntity
    {
        public const int MaximumEnergy = 100;

        public Being(IWorld world)
            : base(world)
        {
        }

        public int MaximumHealth { get; set; }

        public int CurrentHealth { get; set; }

        public int CurrentEnergy { get; set; }

        public int RegenerationEnergy { get; set; }

        public int MovementSpeed { get; set; }
    }
}
