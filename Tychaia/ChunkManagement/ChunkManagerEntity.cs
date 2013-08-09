// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public class ChunkManagerEntity : Entity
    {
        private readonly IChunkFactory m_ChunkFactory;
        private readonly IChunkSizePolicy m_ChunkSizePolicy;
        private readonly TychaiaGameWorld m_World;

        public ChunkManagerEntity(
            TychaiaGameWorld gameWorld,
            IChunkSizePolicy chunkSizePolicy,
            IChunkFactory chunkFactory)
        {
            this.m_World = gameWorld;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_ChunkFactory = chunkFactory;
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            base.Render(gameContext, renderContext);

            // Find the camera's focus position.
            var focus = this.m_World.IsometricCamera.CurrentFocus;

            // Find the chunk that belongs at this position.
            for (var x = -3; x <= 3; x++)
                for (var y = -3; y <= 3; y++)
                    for (var z = -3; z <= 3; z++)
                    {
                        var chunk = this.GetChunkOrGenerate(this.m_World.ChunkOctree, new Vector3(
                            this.m_World.IsometricCamera.Chunk.X +
                            x * (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth),
                            this.m_World.IsometricCamera.Chunk.Y +
                            y * (this.m_ChunkSizePolicy.ChunkCellHeight * this.m_ChunkSizePolicy.CellVoxelHeight),
                            this.m_World.IsometricCamera.Chunk.Z +
                            z * (this.m_ChunkSizePolicy.ChunkCellDepth * this.m_ChunkSizePolicy.CellVoxelDepth)));
                        chunk.Render(gameContext, renderContext);
                    }
        }

        private Chunk GetChunkOrGenerate(ChunkOctree octree, Vector3 chunkPos)
        {
            var existing = octree.Get((long) chunkPos.X, (long) chunkPos.Y, (long) chunkPos.Z);
            if (existing != null)
                return existing;
            return this.m_ChunkFactory.CreateChunk(
                null,
                octree,
                (long) chunkPos.X,
                (long) chunkPos.Y,
                (long) chunkPos.Z);
        }
    }
}