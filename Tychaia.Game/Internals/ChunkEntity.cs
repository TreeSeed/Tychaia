using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using Microsoft.Xna.Framework;

namespace Tychaia.Game
{
    public class ChunkEntity : Entity
    {
        private World m_World = null;

        protected ChunkEntity(World world)
        {
            this.m_World = world;
            base.X = 0;
            base.Y = 0;
        }

        public float ImageOffsetX
        {
            get;
            set;
        }

        public float ImageOffsetY
        {
            get;
            set;
        }

        public float Z
        {
            get;
            set;
        }

        public int Depth
        {
            get;
            set;
        }

        public override T CollidesAt<T>(World world, int x, int y)
        {
            throw new InvalidOperationException();
        }
    }
}
