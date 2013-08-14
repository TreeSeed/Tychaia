// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public class FrustumBasedChunkGeneratorAI : IChunkAI
    {
        private IProfiler m_Profiler;
        private IChunkFactory m_ChunkFactory;
        private IChunkSizePolicy m_ChunkSizePolicy;
        private IFrustumChunkCache m_FrustumChunkCache;
        private IDebugCubeRenderer m_DebugCubeRenderer;

        public FrustumBasedChunkGeneratorAI(
            IProfiler profiler,
            IChunkFactory chunkFactory,
            IChunkSizePolicy chunkSizePolicy,
            IFrustumChunkCache frustumChunkCache,
            IDebugCubeRenderer debugCubeRenderer)
        {
            this.m_Profiler = profiler;
            this.m_ChunkFactory = chunkFactory;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_FrustumChunkCache = frustumChunkCache;
            this.m_DebugCubeRenderer = debugCubeRenderer;
            this.ShowDebugInfo = "false";

            // TODO: Find some better way of doing this.  Probably need to expose this information in
            // the IIsometricCamera interface and then call SetFrustumScope in the Process method.
            this.m_FrustumChunkCache.SetFrustumScope(
                Matrix.CreateLookAt(
                    new Vector3(15, 30, 15) * 35,
                    Vector3.Zero,
                    Vector3.Up) *
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4f / 3f, 1.0f, 5000.0f));
        }

        public string ShowDebugInfo { get; set; }

        public Chunk[] Process(
            TychaiaGameWorld world,
            ChunkManagerEntity manager,
            IGameContext gameContext,
            IRenderContext renderContext)
        {
            if (this.ShowDebugInfo.ToLower() == "true")
            {
                foreach (var position in this.m_FrustumChunkCache.GetAbsolutePositions(
                    new Vector3(
                        world.IsometricCamera.Chunk.X,
                        world.IsometricCamera.Chunk.Y,
                        world.IsometricCamera.Chunk.Z)))
                {
                    Action<Color> box = color => this.m_DebugCubeRenderer.RenderWireframeCube(
                        renderContext,
                        new Protogame.BoundingBox
                        {
                            X = position.X,
                            Y = position.Y,
                            Z = position.Z,
                            Width = this.m_ChunkSizePolicy.CellVoxelWidth * this.m_ChunkSizePolicy.ChunkCellWidth,
                            Height = this.m_ChunkSizePolicy.CellVoxelHeight * this.m_ChunkSizePolicy.ChunkCellHeight,
                            Depth = this.m_ChunkSizePolicy.CellVoxelDepth * this.m_ChunkSizePolicy.ChunkCellDepth
                        },
                        color);
                    var chunk = this.GetChunkOrGenerate(
                        world.ChunkOctree,
                        (long)position.X,
                        (long)position.Y,
                        (long)position.Z);
                    if (!chunk.Generated)
                        box(Color.Yellow);
                    else if (chunk.GraphicsEmpty)
                        box(Color.Red);
                    else
                        box(Color.Green);
                }
            }
            else
            {
                foreach (var position in this.m_FrustumChunkCache.GetAbsolutePositions(
                    new Vector3(
                        world.IsometricCamera.Chunk.X,
                        world.IsometricCamera.Chunk.Y,
                        world.IsometricCamera.Chunk.Z)))
                {
                    this.GetChunkOrGenerate(
                        world.ChunkOctree,
                        (long)position.X,
                        (long)position.Y,
                        (long)position.Z);
                }
            }
            return null;
        }

        private Chunk GetChunkOrGenerate(ChunkOctree octree, long x, long y, long z)
        {
            using (this.m_Profiler.Measure("tychaia-chunk_test"))
            {
                var existing = octree.Get(x, y, z);
                if (existing != null)
                    return existing;
            }
            using (this.m_Profiler.Measure("tychaia-chunk_create"))
            {
                return this.m_ChunkFactory.CreateChunk(
                    null,
                    octree,
                    x,
                    y,
                    z);
            }
        }
    }
}

