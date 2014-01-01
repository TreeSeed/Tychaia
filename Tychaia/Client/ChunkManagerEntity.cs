// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia
{
    public class ChunkManagerEntity : Entity
    {
        private readonly IChunkAI[] m_ChunkAI;

        private readonly IChunkRenderer m_ChunkRenderer;

        private readonly IProfiler m_Profiler;

        private readonly TychaiaGameWorld m_World;

        /// <remarks>
        /// Don't inline this field; we use it so that AIs can return null if they
        /// determine that the list of rendered chunks doesn't need to be recalculated.
        /// </remarks>
        private ClientChunk[] m_ChunksToRenderNext;

        public ChunkManagerEntity(
            TychaiaGameWorld gameWorld, 
            IChunkAI[] chunkAI, 
            IProfiler profiler, 
            IChunkRenderer chunkRenderer)
        {
            this.m_World = gameWorld;
            this.m_ChunkAI = chunkAI;
            this.m_Profiler = profiler;
            this.m_ChunkRenderer = chunkRenderer;
            this.m_ChunksToRenderNext = new ClientChunk[0];
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
                    {
                        this.m_ChunksToRenderNext = result;
                    }
                }
            }

            // Find the chunk that belongs at this position.
            using (this.m_Profiler.Measure("tychaia-chunk_render"))
            {
                foreach (var chunk in this.m_ChunksToRenderNext)
                {
                    if (chunk != null)
                    {
                        this.m_ChunkRenderer.Render(renderContext, chunk);
                    }
                }
            }
        }
    }
}