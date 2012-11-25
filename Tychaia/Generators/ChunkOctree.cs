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
            return this.m_Octree.Find(x, y, z);
        }

        public void Set(Chunk chunk)
        {
            this.m_Octree.Insert(chunk);
        }

        #endregion
    }
}
