// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Tychaia.Globals;

namespace Tychaia.Runtime
{
    public class DefaultPredeterminedChunkPositions : IPredeterminedChunkPositions
    {
        private IChunkSizePolicy m_ChunkSizePolicy;

        public DefaultPredeterminedChunkPositions(
            IChunkSizePolicy chunkSizePolicy)
        {
            this.m_ChunkSizePolicy = chunkSizePolicy;
        }

        public IEnumerable<Vector3> GetRelativePositions()
        {
            yield return new Vector3(0, 0, 0);
            yield return new Vector3(1, 0, 0);
            yield return new Vector3(0, 0, 1);
            yield return new Vector3(1, 0, 1);
            yield return new Vector3(-1, 0, 0);
            yield return new Vector3(0, 0, -1);
            yield return new Vector3(-1, 0, -1);
            yield return new Vector3(1, 0, -1);
            yield return new Vector3(-1, 0, 1);
            yield return new Vector3(1, 0, -2);
            yield return new Vector3(-2, 0, 1);
            yield return new Vector3(0, 0, -2);
            yield return new Vector3(-2, 0, 0);
            yield return new Vector3(-1, 0, -2);
            yield return new Vector3(-2, 0, -1);

            yield return new Vector3(0, -1, 0);
            yield return new Vector3(1, -1, 0);
            yield return new Vector3(0, -1, 1);
            yield return new Vector3(1, -1, 1);
            yield return new Vector3(-1, -1, 0);
            yield return new Vector3(0, -1, -1);
            yield return new Vector3(-1, -1, -1);
            yield return new Vector3(1, -1, -1);
            yield return new Vector3(-1, -1, 1);
            yield return new Vector3(1, -1, -2);
            yield return new Vector3(-2, -1, 1);
            yield return new Vector3(0, -1, -2);
            yield return new Vector3(-2, -1, 0);
            yield return new Vector3(-1, -1, -2);
            yield return new Vector3(-2, -1, -1);
            yield return new Vector3(1, -1, -3);
            yield return new Vector3(-3, -1, 1);
            yield return new Vector3(0, -1, -3);
            yield return new Vector3(-3, -1, 0);

            yield return new Vector3(0, 1, 0);
            yield return new Vector3(1, 1, 0);
            yield return new Vector3(0, 1, 1);
            yield return new Vector3(1, 1, 1);
            yield return new Vector3(-1, 1, 0);
            yield return new Vector3(0, 1, -1);
            yield return new Vector3(-1, 1, -1);
            yield return new Vector3(1, 1, -1);
            yield return new Vector3(-1, 1, 1);
            yield return new Vector3(1, 1, -2);
            yield return new Vector3(-2, 1, 1);
            yield return new Vector3(0, 1, -2);
            yield return new Vector3(-2, 1, 0);
            yield return new Vector3(-1, 1, -2);
            yield return new Vector3(-2, 1, -1);
            yield return new Vector3(1, 1, -3);
            yield return new Vector3(-3, 1, 1);
            yield return new Vector3(0, 1, -3);
            yield return new Vector3(-3, 1, 0);
        }

        public IEnumerable<Vector3> GetScaledRelativePositions()
        {
            foreach (var relative in this.GetRelativePositions())
                yield return new Vector3(
                    relative.X * this.m_ChunkSizePolicy.CellVoxelWidth * this.m_ChunkSizePolicy.ChunkCellWidth,
                    relative.Y * this.m_ChunkSizePolicy.CellVoxelHeight * this.m_ChunkSizePolicy.ChunkCellHeight,
                    relative.Z * this.m_ChunkSizePolicy.CellVoxelDepth * this.m_ChunkSizePolicy.ChunkCellDepth);
        }

        public IEnumerable<Vector3> GetAbsolutePositions(Vector3 absolute)
        {
            foreach (var relative in this.GetScaledRelativePositions())
                yield return absolute + relative;
        }

        public IEnumerable<RuntimeChunk> GetChunks(ChunkOctree octree, Vector3 focus)
        {
            foreach (var position in this.GetAbsolutePositions(focus))
                yield return octree.Get((long)position.X, (long)position.Y, (long)position.Z);
        }

        public IEnumerable<Vector3> GetPurgableRelativePositions()
        {
            yield return new Vector3(-4, -4, 0);
            yield return new Vector3(-4, -3, 0);
            yield return new Vector3(-4, -2, 0);
            yield return new Vector3(-4, -1, 0);
            yield return new Vector3(-4, 0, 0);
            yield return new Vector3(-4, 1, 0);
            yield return new Vector3(-4, 2, 0);
            yield return new Vector3(-4, 3, 0);
            yield return new Vector3(-4, 4, 0);

            yield return new Vector3(4, -4, 0);
            yield return new Vector3(4, -3, 0);
            yield return new Vector3(4, -2, 0);
            yield return new Vector3(4, -1, 0);
            yield return new Vector3(4, 0, 0);
            yield return new Vector3(4, 1, 0);
            yield return new Vector3(4, 2, 0);
            yield return new Vector3(4, 3, 0);
            yield return new Vector3(4, 4, 0);
        }

        public IEnumerable<Vector3> GetPurgableScaledRelativePositions()
        {
            foreach (var relative in this.GetPurgableRelativePositions())
                yield return new Vector3(
                    relative.X * this.m_ChunkSizePolicy.CellVoxelWidth * this.m_ChunkSizePolicy.ChunkCellWidth,
                    relative.Y * this.m_ChunkSizePolicy.CellVoxelHeight * this.m_ChunkSizePolicy.ChunkCellHeight,
                    relative.Z * this.m_ChunkSizePolicy.CellVoxelDepth * this.m_ChunkSizePolicy.ChunkCellDepth);
        }

        public IEnumerable<Vector3> GetPurgableAbsolutePositions(Vector3 absolute)
        {
            foreach (var relative in this.GetPurgableScaledRelativePositions())
                yield return absolute + relative;
        }
    }
}
