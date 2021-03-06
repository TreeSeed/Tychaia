// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Microsoft.Xna.Framework;
using Protogame;
using Tychaia.Runtime;

namespace Tychaia
{
    public class PredeterminedChunkPurgerAI : IChunkAI
    {
        private readonly IPredeterminedChunkPositions m_PredeterminedChunkPositions;

        public PredeterminedChunkPurgerAI(IPredeterminedChunkPositions predeterminedChunkPositions)
        {
            this.m_PredeterminedChunkPositions = predeterminedChunkPositions;
        }

        public ClientChunk[] Process(
            TychaiaGameWorld world, 
            ChunkManagerEntity manager, 
            IGameContext gameContext, 
            IRenderContext renderContext)
        {
            foreach (
                var position in
                    this.m_PredeterminedChunkPositions.GetPurgableAbsolutePositions(
                        new Vector3(
                            world.IsometricCamera.Chunk.X, 
                            world.IsometricCamera.Chunk.Y, 
                            world.IsometricCamera.Chunk.Z)))
            {
                var chunk = world.ChunkOctree.Get((long)position.X, (long)position.Y, (long)position.Z);
                if (chunk != null)
                {
                    Console.WriteLine("FIXME: PURGING CHUNK");

                    // chunk.Purge();
                }
            }

            return null;
        }
    }
}