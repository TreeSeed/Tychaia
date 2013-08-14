// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public class FrustumBasedChunkRenderPickerAI : IChunkAI
    {
        private IChunkSizePolicy m_ChunkSizePolicy;
        private IFrustumChunkCache m_FrustumChunkCache;

        private bool m_FirstProcess = true;
        private Vector3 m_PreviousFocusChunk;

        public FrustumBasedChunkRenderPickerAI(
            IChunkSizePolicy chunkSizePolicy,
            IFrustumChunkCache frustumChunkCache)
        {
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_FrustumChunkCache = frustumChunkCache;

            // TODO: Find some better way of doing this.  Probably need to expose this information in
            // the IIsometricCamera interface and then call SetFrustumScope in the Process method.
            this.m_FrustumChunkCache.SetFrustumScope(
                Matrix.CreateLookAt(
                    new Vector3(15, 30, 15) * 35,
                    Vector3.Zero,
                    Vector3.Up) *
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4f / 3f, 1.0f, 5000.0f));
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
            return this.m_FrustumChunkCache.GetChunks(
                world.ChunkOctree,
                new Vector3(
                    world.IsometricCamera.Chunk.X,
                    world.IsometricCamera.Chunk.Y,
                    world.IsometricCamera.Chunk.Z)).Concat(new List<Chunk>
                {
                    world.IsometricCamera.Chunk.East.North,
                    world.IsometricCamera.Chunk.West.South
                }).ToArray();
        }
    }
}

