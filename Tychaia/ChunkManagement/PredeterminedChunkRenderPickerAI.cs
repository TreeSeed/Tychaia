// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Linq;
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public class PredeterminedChunkRenderPickerAI : IChunkAI
    {
        private IChunkSizePolicy m_ChunkSizePolicy;
        private IPredeterminedChunkPositions m_PredeterminedChunkPositions;

        private bool m_FirstProcess = true;
        private Vector3 m_PreviousFocusChunk;

        public PredeterminedChunkRenderPickerAI(
            IChunkSizePolicy chunkSizePolicy,
            IPredeterminedChunkPositions predeterminedChunkPositions)
        {
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_PredeterminedChunkPositions = predeterminedChunkPositions;
        }

        public Chunk[] Process(
            TychaiaGameWorld world,
            ChunkManagerEntity manager,
            IGameContext gameContext,
            IRenderContext renderContext)
        {
            // If the currently focused chunk hasn't changed, then just use
            // the previous list of chunks.
            if (world.IsometricCamera.CurrentFocus == this.m_PreviousFocusChunk && !this.m_FirstProcess)
                return null;

            this.m_FirstProcess = false;
            this.m_PreviousFocusChunk = world.IsometricCamera.CurrentFocus;
            return this.m_PredeterminedChunkPositions.GetChunks(
                world.ChunkOctree,
                new Vector3(
                    world.IsometricCamera.Chunk.X,
                    world.IsometricCamera.Chunk.Y,
                    world.IsometricCamera.Chunk.Z)).ToArray();
        }
    }
}

