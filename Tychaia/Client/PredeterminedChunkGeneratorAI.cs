// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Microsoft.Xna.Framework;
using Protogame;
using Tychaia.Client;
using Tychaia.Globals;
using Tychaia.Runtime;

namespace Tychaia
{
    public class PredeterminedChunkGeneratorAI : IChunkAI
    {
        private IProfiler m_Profiler;

        private IClientChunkFactory m_ClientChunkFactory;
        private IChunkSizePolicy m_ChunkSizePolicy;
        private IDebugCubeRenderer m_DebugCubeRenderer;
        private IPredeterminedChunkPositions m_PredeterminedChunkPositions;

        public PredeterminedChunkGeneratorAI(
            IProfiler profiler,
            IChunkSizePolicy chunkSizePolicy,
            IDebugCubeRenderer debugCubeRenderer,
            IPredeterminedChunkPositions predeterminedChunkPositions,
            IClientChunkFactory clientChunkFactory)
        {
            this.m_Profiler = profiler;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_DebugCubeRenderer = debugCubeRenderer;
            this.m_PredeterminedChunkPositions = predeterminedChunkPositions;
            this.m_ClientChunkFactory = clientChunkFactory;
            this.ShowDebugInfo = "false";
        }

        public string ShowDebugInfo { get; set; }

        public ClientChunk[] Process(
            TychaiaGameWorld world,
            ChunkManagerEntity manager,
            IGameContext gameContext,
            IRenderContext renderContext)
        {
            if (this.ShowDebugInfo.ToLower() == "true")
            {
                foreach (var position in this.m_PredeterminedChunkPositions.GetAbsolutePositions(
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
                        world.Level,
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
                foreach (var position in this.m_PredeterminedChunkPositions.GetAbsolutePositions(
                    new Vector3(
                        world.IsometricCamera.Chunk.X,
                        world.IsometricCamera.Chunk.Y,
                        world.IsometricCamera.Chunk.Z)))
                {
                    this.GetChunkOrGenerate(
                        world.ChunkOctree,
                        world.Level,
                        (long)position.X,
                        (long)position.Y,
                        (long)position.Z);
                }
            }
            
            return null;
        }

        private ClientChunk GetChunkOrGenerate(ChunkOctree<ClientChunk> octree, ILevel level, long x, long y, long z)
        {
            using (this.m_Profiler.Measure("tychaia-chunk_test"))
            {
                var existing = octree.Get(x, y, z);
                if (existing != null)
                    return existing;
            }
            
            using (this.m_Profiler.Measure("tychaia-chunk_create"))
            {
                return this.m_ClientChunkFactory.CreateClientChunk(
                    octree,
                    x,
                    y,
                    z);
            }
        }
    }
}
