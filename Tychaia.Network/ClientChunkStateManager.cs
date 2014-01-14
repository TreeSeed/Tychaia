// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Protogame;
using Tychaia.Globals;
using Tychaia.Runtime;

namespace Tychaia.Network
{
    public class ClientChunkStateManager
    {
        private readonly ChunkOctree<ServerChunk> m_ClientHasChunkOctree;
    
        private readonly IPositionScaleTranslation m_PositionScaleTranslation;
    
        private readonly IPredeterminedChunkPositions m_PredeterminedChunkPositions;
    
        public ClientChunkStateManager(
            IChunkOctreeFactory chunkOctreeFactory,
            IPositionScaleTranslation positionScaleTranslation,
            IPredeterminedChunkPositions predeterminedChunkPositions,
            MxClient client)
        {
            this.m_PositionScaleTranslation = positionScaleTranslation;
            this.m_PredeterminedChunkPositions = predeterminedChunkPositions;
            
            this.m_ClientHasChunkOctree = chunkOctreeFactory.CreateChunkOctree<ServerChunk>();
        }
        
        public ChunkOctree<ServerChunk> Octree
        {
            get
            {
                return this.m_ClientHasChunkOctree;
            }
        }
        
        public void RecalculateDesiredChunks(
            PlayerServerEntity playerEntity,
            ChunkOctree<ServerChunk> serverOctree,
            Action<long, long, long> chunkRequired)
        {
            var chunks = new List<ChunkPos>();
            
            var current = serverOctree.Get((long)playerEntity.X, (long)playerEntity.Y, (long)playerEntity.Z);
            
            foreach (var l in this.m_PredeterminedChunkPositions.GetAbsolutePositions(new Vector3(
                (float)current.X,
                (float)current.Y,
                (float)current.Z)))
            {
                chunks.Add(
                    new ChunkPos 
                    {
                        X = (long)l.X,
                        Y = (long)l.Y,
                        Z = (long)l.Z
                    });
            }
            
            // Check if each of the chunks is already in the octree.
            foreach (var pos in chunks.ToArray())
            {
                if (this.m_ClientHasChunkOctree.Get(pos.X, pos.Y, pos.Z) != null)
                {
                    chunks.Remove(pos);
                }
            }
            
            // Callback for required chunks.
            foreach (var chunk in chunks)
            {
                chunkRequired(chunk.X, chunk.Y, chunk.Z);
            }
        }
        
        private class ChunkPos
        {
            public long X { get; set; }
            
            public long Y { get; set; }
            
            public long Z { get; set; }
        }
    }
}
