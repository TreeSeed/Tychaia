// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Protogame;
using Tychaia.Globals;

namespace Tychaia
{
    public class ChunkOctree
    {
        private readonly IFilteredFeatures m_FilteredFeatures;
        private readonly IPositionScaleTranslation m_PositionScaleTranslation;
        private readonly PositionOctree<RuntimeChunk> m_Octree = new PositionOctree<RuntimeChunk>();

        public ChunkOctree(
            IFilteredFeatures filteredFeatures,
            IPositionScaleTranslation positionScaleTranslation)
        {
            this.m_FilteredFeatures = filteredFeatures;
            this.m_PositionScaleTranslation = positionScaleTranslation;
        }
        
        public RuntimeChunk Get(long x, long y, long z)
        {
            var c = PositionOctreeUtil.GetFast64(
                this.m_Octree,
                this.m_PositionScaleTranslation.Translate(x),
                this.m_PositionScaleTranslation.Translate(y),
                this.m_PositionScaleTranslation.Translate(z));
            if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeValidation) && c != null &&
                (c.X != x || c.Y != y || c.Z != z))
                throw new InvalidOperationException(
                    "Octree lookup result returned chunk with different position than originally stored!");
            return c;
        }

        public void Set(RuntimeChunk chunk)
        {
            var xx = this.m_PositionScaleTranslation.Translate(chunk.X);
            var yy = this.m_PositionScaleTranslation.Translate(chunk.Y);
            var zz = this.m_PositionScaleTranslation.Translate(chunk.Z);
            this.m_Octree.Insert(chunk, xx, yy, zz);
            if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeValidation))
            {
                if (this.m_Octree.Find(xx, yy, zz) != chunk)
                    throw new InvalidOperationException("Octree did not store data correctly for " + xx +
                                                        ", " + yy + ", " + zz + ".");
            }
        }
    }
}
