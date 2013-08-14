// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
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
        private readonly Vector3[] m_AOTChunkPositions;
        private readonly IChunkAI[] m_ChunkAI;
        private Chunk[] m_ChunksToRenderNext;

        public ChunkManagerEntity(
            TychaiaGameWorld gameWorld,
            IChunkSizePolicy chunkSizePolicy,
            IAssetManagerProvider assetManagerProvider,
            IChunkFactory chunkFactory,
            IChunkAI[] chunkAI,
            IProfiler profiler)
        {
            this.m_World = gameWorld;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_ChunkFactory = chunkFactory;
            this.m_ChunkAI = chunkAI;
            this.m_TextureAtlasAsset = assetManagerProvider.GetAssetManager().Get<TextureAtlasAsset>("atlas");
            this.m_Profiler = profiler;
            this.m_ChunksToRenderNext = new Chunk[0];
        }

        public IChunkAI[] GetAIs()
        {
            return this.m_ChunkAI;
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            base.Render(gameContext, renderContext);

            // For each of the chunk AIs, process them.  It's not ideal to have this in
            // the Render() call, but some AIs need access to the render context so that
            // they can do bounding frustum checks to find out what's on screen.
            using (this.m_Profiler.Measure("tychaia-chunk_ai"))
            {
                foreach (var ai in this.m_ChunkAI)
                {
                    var result = ai.Process(this.m_World, this, gameContext, renderContext);
                    if (result != null)
                        this.m_ChunksToRenderNext = result;
                }
            }

            // Find the chunk that belongs at this position.
            using (this.m_Profiler.Measure("tychaia-chunk_render"))
            {
                foreach (var chunk in this.m_ChunksToRenderNext)
                {
                    if (chunk != null)
                        chunk.Render(gameContext, renderContext);
                }
            }
        }
    }
}
