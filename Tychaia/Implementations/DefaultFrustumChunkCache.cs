// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Tychaia
{
    public class DefaultFrustumChunkCache : IFrustumChunkCache
    {
        private IChunkSizePolicy m_ChunkSizePolicy;
        private float m_ChunkVoxelWidth;
        private float m_ChunkVoxelHeight;
        private float m_ChunkVoxelDepth;

        private BoundingFrustum m_BoundingFrustum;
        private List<Vector3> m_PositionCache;

        public DefaultFrustumChunkCache(
            IChunkSizePolicy chunkSizePolicy)
        {
            this.m_ChunkSizePolicy = chunkSizePolicy;

            this.m_ChunkVoxelWidth = this.m_ChunkSizePolicy.CellVoxelWidth * this.m_ChunkSizePolicy.ChunkCellWidth;
            this.m_ChunkVoxelHeight = this.m_ChunkSizePolicy.CellVoxelHeight * this.m_ChunkSizePolicy.ChunkCellHeight;
            this.m_ChunkVoxelDepth = this.m_ChunkSizePolicy.CellVoxelDepth * this.m_ChunkSizePolicy.ChunkCellDepth;
        }

        public void SetFrustumScope(Matrix frustum)
        {
            this.m_BoundingFrustum = new BoundingFrustum(frustum);

            // Calculate in a 5x5x5 radius from the imaginary center chunk.
            this.m_PositionCache = new List<Vector3>();
            for (var a = -2; a <= 2; a++)
            for (var x = -5; x <= 5; x++)
            for (var z = -5; z <= 5; z++)
            {
                var y = 0;
                if (a == -2) y = 0;
                else if (a == -1) y = 1;
                else if (a == 0) y = -1;
                else if (a == 1) y = 2;
                else if (a == 2) y = -2;
                if (this.m_BoundingFrustum.Contains(this.GetBoundingBoxForChunk(
                    x * (long)this.m_ChunkVoxelWidth,
                    y * (long)this.m_ChunkVoxelHeight,
                    z * (long)this.m_ChunkVoxelDepth)) != ContainmentType.Disjoint)
                {
                    this.m_PositionCache.Add(new Vector3(
                        x * this.m_ChunkVoxelWidth,
                        y * this.m_ChunkVoxelHeight,
                        z * this.m_ChunkVoxelDepth));
                }
            }
        }

        private BoundingBox GetBoundingBoxForChunk(long rx, long ry, long rz)
        {
            return new BoundingBox(
                new Vector3(
                    rx,
                    ry,
                    rz),
                new Vector3(
                    rx + this.m_ChunkVoxelWidth,
                    ry + this.m_ChunkVoxelHeight,
                    rz + this.m_ChunkVoxelDepth));
        }

        public IEnumerable<Vector3> GetRelativePositions()
        {
            return this.m_PositionCache;
        }

        public IEnumerable<Vector3> GetAbsolutePositions(Vector3 currentFocus)
        {
            foreach (var relative in this.GetRelativePositions())
                yield return relative + currentFocus;
        }

        public IEnumerable<RuntimeChunk> GetChunks(ChunkOctree octree, Vector3 focus)
        {
            foreach (var position in this.GetAbsolutePositions(focus))
                yield return octree.Get((long)position.X, (long)position.Y, (long)position.Z);
        }
    }
}

