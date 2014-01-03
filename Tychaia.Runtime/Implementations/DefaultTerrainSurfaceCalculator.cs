// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.Globals;

namespace Tychaia.Runtime
{
    public class DefaultTerrainSurfaceCalculator : ITerrainSurfaceCalculator
    {
        private readonly IChunkSizePolicy m_ChunkSizePolicy;

        public DefaultTerrainSurfaceCalculator(IChunkSizePolicy chunkSizePolicy)
        {
            this.m_ChunkSizePolicy = chunkSizePolicy;
        }

        public float? GetSurfaceY<T>(ChunkOctree<T> octree, float x, float z) where T : class, IChunk
        {
            if (octree == null)
            {
                return null;
            }

            var chunk = octree.Get((long)x, 0, (long)z);

            if (chunk == null || chunk.Cells == null)
            {
                return null;
            }

            var ax = (int)(x - chunk.X) / this.m_ChunkSizePolicy.CellVoxelWidth;
            var az = (int)(z - chunk.Z) / this.m_ChunkSizePolicy.CellVoxelDepth;

            if (ax >= 0 && ax < this.m_ChunkSizePolicy.ChunkCellWidth && az >= 0
                && az < this.m_ChunkSizePolicy.ChunkCellDepth)
            {
                var idx = ax + (az * this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.ChunkCellDepth);
                return chunk.Cells[idx].HeightMap * this.m_ChunkSizePolicy.CellVoxelDepth;
            }

            if (ax >= this.m_ChunkSizePolicy.ChunkCellWidth && ax < this.m_ChunkSizePolicy.ChunkCellWidth * 2 && az >= 0
                && az < this.m_ChunkSizePolicy.ChunkCellDepth)
            {
                var eastChunk = octree.Get((long)x + this.m_ChunkSizePolicy.ChunkCellWidth, 0, (long)z);
                if (eastChunk == null || eastChunk.Cells == null)
                {
                    return null;
                }

                var idx = (ax - this.m_ChunkSizePolicy.ChunkCellWidth)
                          + (az * this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.ChunkCellDepth);
                return eastChunk.Cells[idx].HeightMap * this.m_ChunkSizePolicy.CellVoxelDepth;
            }

            if (ax >= 0 && ax < this.m_ChunkSizePolicy.ChunkCellWidth && az >= this.m_ChunkSizePolicy.ChunkCellWidth
                && az < this.m_ChunkSizePolicy.ChunkCellDepth * 2)
            {
                var southChunk = octree.Get((long)x, 0, (long)z + this.m_ChunkSizePolicy.ChunkCellWidth);
                if (southChunk == null || southChunk.Cells == null)
                {
                    return null;
                }

                var idx = ax
                          + ((az - this.m_ChunkSizePolicy.ChunkCellWidth) * this.m_ChunkSizePolicy.ChunkCellWidth
                             * this.m_ChunkSizePolicy.ChunkCellDepth);
                return southChunk.Cells[idx].HeightMap * this.m_ChunkSizePolicy.CellVoxelDepth;
            }

            if (ax >= this.m_ChunkSizePolicy.ChunkCellWidth && ax < this.m_ChunkSizePolicy.ChunkCellWidth * 2
                && az >= this.m_ChunkSizePolicy.ChunkCellWidth && az < this.m_ChunkSizePolicy.ChunkCellDepth * 2)
            {
                var southEastChunk = octree.Get(
                    (long)x + this.m_ChunkSizePolicy.ChunkCellWidth, 
                    0, 
                    (long)z + this.m_ChunkSizePolicy.ChunkCellWidth);
                if (southEastChunk == null || southEastChunk.Cells == null)
                {
                    return null;
                }

                var idx = (ax - this.m_ChunkSizePolicy.ChunkCellWidth)
                          + ((az - this.m_ChunkSizePolicy.ChunkCellWidth) * this.m_ChunkSizePolicy.ChunkCellWidth
                             * this.m_ChunkSizePolicy.ChunkCellDepth);
                return southEastChunk.Cells[idx].HeightMap * this.m_ChunkSizePolicy.CellVoxelDepth;
            }

            return null;
        }
    }
}