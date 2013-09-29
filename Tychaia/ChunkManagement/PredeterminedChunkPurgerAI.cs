// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Protogame;
using Tychaia.Globals;

namespace Tychaia
{
    public class PredeterminedChunkPurgerAI : IChunkAI
    {
        private IProfiler m_Profiler;
        private IChunkFactory m_ChunkFactory;
        private IChunkSizePolicy m_ChunkSizePolicy;
        private IPredeterminedChunkPositions m_PredeterminedChunkPositions;

        public PredeterminedChunkPurgerAI(
            IProfiler profiler,
            IChunkFactory chunkFactory,
            IChunkSizePolicy chunkSizePolicy,
            IPredeterminedChunkPositions predeterminedChunkPositions)
        {
            this.m_Profiler = profiler;
            this.m_ChunkFactory = chunkFactory;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_PredeterminedChunkPositions = predeterminedChunkPositions;
        }

        public RuntimeChunk[] Process(
            TychaiaGameWorld world,
            ChunkManagerEntity manager,
            IGameContext gameContext,
            IRenderContext renderContext)
        {
            foreach (var position in this.m_PredeterminedChunkPositions.GetPurgableAbsolutePositions(
                new Vector3(
                    world.IsometricCamera.Chunk.X,
                    world.IsometricCamera.Chunk.Y,
                    world.IsometricCamera.Chunk.Z)))
            {
                var chunk = world.ChunkOctree.Get(
                    (long)position.X,
                    (long)position.Y,
                    (long)position.Z);
                if (chunk != null)
                {
                    Console.WriteLine("PURGING CHUNK");
                    chunk.Purge();
                }
            }
            return null;
        }
    }
}

