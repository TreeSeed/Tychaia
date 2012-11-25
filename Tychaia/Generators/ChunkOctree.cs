using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame.Structure;

namespace Tychaia.Generators
{
    public class ChunkOctree
    {
        private PositionOctree<Chunk> m_Octree = new PositionOctree<Chunk>();

        #region IAlgorithm Members

        public Chunk Get(long x, long y, long z)
        {
            return this.m_Octree.Find(x / 256, y / 256, z / 256);
        }

        public void Set(Chunk chunk)
        {
            this.m_Octree.Insert(chunk, chunk.X / 256, chunk.Y / 256, chunk.Z / 256);
        }

        #endregion
    }
}
