using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame.Structure;
using Tychaia.Globals;

namespace Tychaia.Generators
{
    public class ChunkOctree
    {
        private PositionOctree<Chunk> m_Octree = new PositionOctree<Chunk>();

        #region IAlgorithm Members

        public Chunk Get(long x, long y, long z)
        {
            Chunk c = this.m_Octree.Find(x / 256, y / 256, z / 256);
            if (c != null && (c.X != x || c.Y != y || c.Z != z) && FilteredFeatures.IsEnabled(Feature.DebugOctreeValidation))
                throw new InvalidOperationException("Octree lookup result returned chunk with different position than originally stored!");
            return c;
        }

        public void Set(Chunk chunk)
        {
            this.m_Octree.Insert(chunk, chunk.X / 256, chunk.Y / 256, chunk.Z / 256);
            if (FilteredFeatures.IsEnabled(Feature.DebugOctreeValidation))
            {
                if (this.m_Octree.Find(chunk.X / 256, chunk.Y / 256, chunk.Z / 256) != chunk)
                    throw new InvalidOperationException("Octree did not store data correctly for " + chunk.X / 256 + ", " + chunk.Y / 256 + ", " + chunk.Z / 256 + ".");
            }
        }

        #endregion
    }
}
