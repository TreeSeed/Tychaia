// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public class ChunkManagerEntity : Entity
    {
        private readonly IChunkFactory m_ChunkFactory;
        private readonly IChunkSizePolicy m_ChunkSizePolicy;
        private readonly TychaiaGameWorld m_World;
        private readonly TextureAtlasAsset m_TextureAtlasAsset;
        private readonly IProfiler m_Profiler;
        private readonly Vector3[] m_CachedChunkPositions;

        public ChunkManagerEntity(
            TychaiaGameWorld gameWorld,
            IChunkSizePolicy chunkSizePolicy,
            IAssetManagerProvider assetManagerProvider,
            IChunkFactory chunkFactory,
            IProfiler profiler)
        {
            this.m_World = gameWorld;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_ChunkFactory = chunkFactory;
            this.m_TextureAtlasAsset = assetManagerProvider.GetAssetManager().Get<TextureAtlasAsset>("atlas");
            this.m_Profiler = profiler;
            
            const int dist = 2;
            this.m_CachedChunkPositions = new Vector3[(dist * 2 + 1) * (dist * 2 + 1) * (dist * 2 + 1)];
            var i = 0;
            for (var x = -dist; x <= dist; x++)
                for (var y = -dist; y <= dist; y++)
                    for (var z = -dist; z <= dist; z++)
                    {
                        this.m_CachedChunkPositions[i] = new Vector3(
                            x * (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth),
                            y * (this.m_ChunkSizePolicy.ChunkCellHeight * this.m_ChunkSizePolicy.CellVoxelHeight),
                            z * (this.m_ChunkSizePolicy.ChunkCellDepth * this.m_ChunkSizePolicy.CellVoxelDepth));
                        i++;
                    }
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            base.Render(gameContext, renderContext);

            // Find the camera's focus position.
            var focus = this.m_World.IsometricCamera.CurrentFocus;

            // Find the chunk that belongs at this position.
            using (this.m_Profiler.Measure("tychaia-chunk_loop"))
            {
                var orig = new Vector3(
                    this.m_World.IsometricCamera.Chunk.X,
                    this.m_World.IsometricCamera.Chunk.Y,
                    this.m_World.IsometricCamera.Chunk.Z);
                foreach (var pos in this.m_CachedChunkPositions)
                {
                    Chunk chunk;
                    using (this.m_Profiler.Measure("tychaia-chunk_get_or_generate"))
                    {
                        chunk = this.GetChunkOrGenerate(this.m_World.ChunkOctree, orig + pos);
                    }
                    using (this.m_Profiler.Measure("tychaia-chunk_render"))
                    {
                        chunk.Render(gameContext, renderContext);
                    }
                }
             }
        }

        private Chunk GetChunkOrGenerate(ChunkOctree octree, Vector3 chunkPos)
        {
            using (this.m_Profiler.Measure("tychaia-chunk_test"))
            {
                var existing = octree.Get((long) chunkPos.X, (long) chunkPos.Y, (long) chunkPos.Z);
                if (existing != null)
                    return existing;
            }
            using (this.m_Profiler.Measure("tychaia-chunk_create"))
            {
                return this.m_ChunkFactory.CreateChunk(
                    null,
                    octree,
                    (long) chunkPos.X,
                    (long) chunkPos.Y,
                    (long) chunkPos.Z);
            }
        }
    }
}