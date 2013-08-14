// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.Game
{
    public class ChunkEntity : Entity
    {
        private IWorld m_World;
        private float m_Z;

        protected ChunkEntity(IWorld world)
        {
            this.m_World = world;
            base.X = 0;
            base.Y = 0;
        }

        public float ImageOffsetX { get; set; }

        public float ImageOffsetY { get; set; }

        public float Z
        {
            get { return this.m_Z; }
            set { this.m_Z = value; }
        }

        public int Depth { get; set; }
    }
}
