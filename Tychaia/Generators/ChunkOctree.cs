// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System;
using Protogame;
using Tychaia.Globals;

namespace Tychaia
{
    public class ChunkOctree
    {
        private readonly IFilteredFeatures m_FilteredFeatures;
        private readonly PositionOctree<Chunk> m_Octree = new PositionOctree<Chunk>();

        public ChunkOctree(
            IFilteredFeatures filteredFeatures)
        {
            this.m_FilteredFeatures = filteredFeatures;
        }

        public Chunk Get(long x, long y, long z)
        {
            var c = this.m_Octree.Find(x / 256, y / 256, z / 256);
            if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeValidation) && c != null &&
                (c.X != x || c.Y != y || c.Z != z))
                throw new InvalidOperationException(
                    "Octree lookup result returned chunk with different position than originally stored!");
            return c;
        }

        public void Set(Chunk chunk)
        {
            this.m_Octree.Insert(chunk, chunk.X / 256, chunk.Y / 256, chunk.Z / 256);
            if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeValidation))
            {
                if (this.m_Octree.Find(chunk.X / 256, chunk.Y / 256, chunk.Z / 256) != chunk)
                    throw new InvalidOperationException("Octree did not store data correctly for " + chunk.X / 256 +
                                                        ", " + chunk.Y / 256 + ", " + chunk.Z / 256 + ".");
            }
        }
    }
}