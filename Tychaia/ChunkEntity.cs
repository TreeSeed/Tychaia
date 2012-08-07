using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using Microsoft.Xna.Framework;

namespace Tychaia
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

        public virtual T CollidesAt<T>(World world, int x, int y) where T : Entity
        {
            throw new InvalidOperationException();
            return Helpers.CollidesAt<T>(this, world, x, y);
        }
    }
}
