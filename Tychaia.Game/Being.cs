using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Game
{
    public class Being : ChunkEntity
    {
        public Being(Protogame.World world)
            : base(world)
        {
        }

        public int MaximumHealth
        {
            get;
            set;
        }

        public int CurrentHealth
        {
            get;
            set;
        }

        public const int MaximumEnergy = 100;

        public int CurrentEnergy
        {
            get;
            set;
        }

        public int RegenerationEnergy
        {
            get;
            set;
        }

        public int MovementSpeed
        {
            get;
            set;
        }
    }
}
