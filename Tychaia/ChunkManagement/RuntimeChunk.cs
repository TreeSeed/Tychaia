// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;
using Tychaia.Globals;
using Tychaia.ProceduralGeneration;
using Tychaia.Data;

namespace Tychaia
{
    public class RuntimeChunk : IDisposable
    {
        private static readonly object m_AccessLock = new object();
        public readonly long X;
        public readonly long Y;
        public readonly long Z;
        private readonly IChunkFactory m_ChunkFactory;
        private readonly IChunkSizePolicy m_ChunkSizePolicy;
        private readonly ILevel m_Level;
        private readonly IFilteredFeatures m_FilteredFeatures;
        private readonly ChunkOctree m_Octree;
        private readonly IRenderCache m_RenderCache;
        private readonly TextureAtlasAsset m_TextureAtlasAsset;
        private readonly IProfiler m_Profiler;
        public BlockAsset[,,] Blocks = null;
        public Cell[,,] Cells = null;
        private IAssetManager m_AssetManager;
        private IChunkGenerator m_ChunkGenerator;
        private IFilteredConsole m_FilteredConsole;

        public bool GraphicsEmpty { get; private set; }
        public bool Generated { get; set; }
        public VertexPositionTexture[] GeneratedVertexes { get; set; }
        public int[] GeneratedIndices { get; set; } 

        private VertexBuffer m_VertexBuffer;
        private IndexBuffer m_IndexBuffer;

        // TODO: This is ugly.
        private int m_Seed = MenuWorld.StaticSeed; // All chunks are generated from the same seed.

        public RuntimeChunk(
            ILevel level,
            ChunkOctree octree,
            IChunkFactory chunkFactory,
            IFilteredConsole filteredConsole,
            IFilteredFeatures filteredFeatures,
            IChunkSizePolicy chunkSizePolicy,
            IRenderCache renderCache,
            IAssetManagerProvider assetManagerProvider,
            IChunkGenerator chunkGenerator,
            IProfiler profiler,
            long x,
            long y,
            long z)
        {
            this.m_ChunkSizePolicy = chunkSizePolicy;

            if (x % (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth) != 0)
                throw new InvalidOperationException("X position must be aligned to chunk grid.");
            if (y % (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth) != 0)
                throw new InvalidOperationException("Y position must be aligned to chunk grid.");
            if (z % (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth) != 0)
                throw new InvalidOperationException("Z position must be aligned to chunk grid.");

            this.m_Octree = octree;
            this.m_FilteredConsole = filteredConsole;
            this.m_FilteredFeatures = filteredFeatures;
            this.m_ChunkFactory = chunkFactory;
            this.m_RenderCache = renderCache;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_TextureAtlasAsset = this.m_AssetManager.Get<TextureAtlasAsset>("atlas");
            this.m_ChunkGenerator = chunkGenerator;
            this.m_Profiler = profiler;
            this.X = x;
            this.Y = y;
            this.Z = z;
            if (this.m_Octree != null)
                this.m_Octree.Set(this);
            this.Blocks = new BlockAsset[this.m_ChunkSizePolicy.ChunkCellWidth, this.m_ChunkSizePolicy.ChunkCellHeight,
                    this.m_ChunkSizePolicy.ChunkCellDepth];
            this.Cells = new Cell[this.m_ChunkSizePolicy.ChunkCellWidth, this.m_ChunkSizePolicy.ChunkCellHeight,
                    this.m_ChunkSizePolicy.ChunkCellDepth];
            this.m_Level = level;
            this.m_FilteredConsole.WriteLine(FilterCategory.ChunkValidation,
                "Chunk created for " + x + ", " + y + ", " + z + ".");

            if (this.m_Level.HasChunk(this))
            {
                try
                {
                    this.m_Level.LoadChunk(this);
                }
                catch (Exception ex)
                {
                    this.m_FilteredConsole.WriteLine(FilterCategory.ChunkValidation, "Unable to load chunk " + x + ", " + y + ", " + z + " from disk due to exception:");
                    this.m_FilteredConsole.WriteLine(FilterCategory.ChunkValidation, ex.ToString());
                    this.m_FilteredConsole.WriteLine(FilterCategory.ChunkValidation, "Chunk " + x + ", " + y + ", " + z + " will be regenerated!");
                    this.Generated = false;
                    this.m_ChunkGenerator.Generate(this);
                }
            }
            else
            {
                this.m_ChunkGenerator.Generate(this);
            }
        }

        public void Dispose()
        {
            if (this.m_VertexBuffer != null)
                this.m_VertexBuffer.Dispose();
            if (this.m_IndexBuffer != null)
                this.m_IndexBuffer.Dispose();
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
                return;

            if (this.GraphicsEmpty)
                return;

            if (this.Generated && this.m_VertexBuffer == null && this.m_IndexBuffer == null)
                this.CalculateBuffers(renderContext);

            if (this.m_VertexBuffer != null && this.m_IndexBuffer != null)
            {
                renderContext.EnableTextures();
                renderContext.SetActiveTexture(this.m_TextureAtlasAsset.TextureAtlas.Texture);
                renderContext.GraphicsDevice.Indices = this.m_IndexBuffer;
                renderContext.GraphicsDevice.SetVertexBuffer(this.m_VertexBuffer);
                renderContext.World = Matrix.CreateScale(32);
                foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    renderContext.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.m_VertexBuffer.VertexCount, 0, this.m_IndexBuffer.IndexCount / 3);
                }
            }
        }

        /// <summary>
        /// Calculates the vertex and index buffers for rendering.
        /// </summary>
        public void CalculateBuffers(IRenderContext renderContext)
        {
            if (this.GeneratedVertexes.Length == 0)
            {
                this.GraphicsEmpty = true;
                return;
            }
            this.m_VertexBuffer = new VertexBuffer(
                renderContext.GraphicsDevice,
                VertexPositionTexture.VertexDeclaration,
                this.GeneratedVertexes.Length,
                BufferUsage.WriteOnly);
            this.m_VertexBuffer.SetData(this.GeneratedVertexes);
            this.m_IndexBuffer = new IndexBuffer(
                renderContext.GraphicsDevice,
                typeof(int),
                this.GeneratedIndices.Length,
                BufferUsage.WriteOnly);
            this.m_IndexBuffer.SetData(this.GeneratedIndices);
        }

        /// <summary>
        /// Saves the chunk back to disk.
        /// </summary>
        public void Save()
        {
            this.m_Level.SaveChunk(this);
        }

        #region Relative Addressing

        public RuntimeChunk West
        {
            get
            {
                lock (m_AccessLock)
                {
                    var c =
                        this.m_Octree.Get(
                            this.X - (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth),
                            this.Y, this.Z);
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_Level, this.m_Octree,
                            this.X - (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth),
                            this.Y, this.Z);
                    if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeLookup) && c == this)
                        throw new InvalidOperationException("Relative addressing of chunk resulted in same chunk.");
                    return c;
                }
            }
        }

        public RuntimeChunk East
        {
            get
            {
                lock (m_AccessLock)
                {
                    var c =
                        this.m_Octree.Get(
                            this.X + (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth),
                            this.Y, this.Z);
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_Level, this.m_Octree,
                            this.X + (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth),
                            this.Y, this.Z);
                    if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeLookup) && c == this)
                        throw new InvalidOperationException("Relative addressing of chunk resulted in same chunk.");
                    return c;
                }
            }
        }

        public RuntimeChunk North
        {
            get
            {
                lock (m_AccessLock)
                {
                    var c = this.m_Octree.Get(this.X, this.Y,
                        this.Z - (this.m_ChunkSizePolicy.ChunkCellDepth * this.m_ChunkSizePolicy.CellVoxelDepth));
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_Level, this.m_Octree, this.X, this.Y,
                            this.Z - (this.m_ChunkSizePolicy.ChunkCellDepth * this.m_ChunkSizePolicy.CellVoxelDepth));
                    if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeLookup) && c == this)
                        throw new InvalidOperationException("Relative addressing of chunk resulted in same chunk.");
                    return c;
                }
            }
        }

        public RuntimeChunk South
        {
            get
            {
                lock (m_AccessLock)
                {
                    var c = this.m_Octree.Get(this.X, this.Y,
                        this.Z + (this.m_ChunkSizePolicy.ChunkCellDepth * this.m_ChunkSizePolicy.CellVoxelDepth));
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_Level, this.m_Octree, this.X, this.Y,
                            this.Z + (this.m_ChunkSizePolicy.ChunkCellDepth * this.m_ChunkSizePolicy.CellVoxelDepth));
                    if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeLookup) && c == this)
                        throw new InvalidOperationException("Relative addressing of chunk resulted in same chunk.");
                    return c;
                }
            }
        }

        public RuntimeChunk Up
        {
            get
            {
                lock (m_AccessLock)
                {
                    var c = this.m_Octree.Get(this.X,
                        this.Y - (this.m_ChunkSizePolicy.ChunkCellHeight * this.m_ChunkSizePolicy.CellVoxelHeight),
                        this.Z);
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_Level, this.m_Octree, this.X,
                            this.Y - (this.m_ChunkSizePolicy.ChunkCellHeight * this.m_ChunkSizePolicy.CellVoxelHeight),
                            this.Z);
                    if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeLookup) && c == this)
                        throw new InvalidOperationException("Relative addressing of chunk resulted in same chunk.");
                    return c;
                }
            }
        }

        public RuntimeChunk Down
        {
            get
            {
                lock (m_AccessLock)
                {
                    var c = this.m_Octree.Get(this.X,
                        this.Y + (this.m_ChunkSizePolicy.ChunkCellHeight * this.m_ChunkSizePolicy.CellVoxelHeight),
                        this.Z);
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_Level, this.m_Octree, this.X,
                            this.Y + (this.m_ChunkSizePolicy.ChunkCellHeight * this.m_ChunkSizePolicy.CellVoxelHeight),
                            this.Z);
                    if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeLookup) && c == this)
                        throw new InvalidOperationException("Relative addressing of chunk resulted in same chunk.");
                    return c;
                }
            }
        }

        #endregion
    }
}
