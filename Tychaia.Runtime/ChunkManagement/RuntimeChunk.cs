// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;
using Tychaia.Data;
using Tychaia.Globals;

namespace Tychaia
{
    public class RuntimeChunk : IDisposable
    {
        public readonly long X;
        public readonly long Y;
        public readonly long Z;
        private static readonly object m_AccessLock = new object();
        private readonly IChunkFactory m_ChunkFactory;
        private readonly IChunkSizePolicy m_ChunkSizePolicy;
        private readonly IFilteredFeatures m_FilteredFeatures;
        private readonly ChunkOctree m_Octree;
        private IChunkGenerator m_ChunkGenerator;
        private IFilteredConsole m_FilteredConsole;

        public RuntimeChunk(
            ILevel level,
            ChunkOctree octree,
            IChunkFactory chunkFactory,
            IFilteredConsole filteredConsole,
            IFilteredFeatures filteredFeatures,
            IChunkSizePolicy chunkSizePolicy,
            IAssetManagerProvider assetManagerProvider,
            IChunkGenerator chunkGenerator,
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
            this.AssetManager = assetManagerProvider.GetAssetManager();
            this.m_ChunkGenerator = chunkGenerator;
            this.X = x;
            this.Y = y;
            this.Z = z;
            if (this.m_Octree != null)
                this.m_Octree.Set(this);
            this.Cells = new Cell[this.m_ChunkSizePolicy.ChunkCellWidth,
                this.m_ChunkSizePolicy.ChunkCellHeight,
                this.m_ChunkSizePolicy.ChunkCellDepth];
            this.Level = level;
            this.m_FilteredConsole.WriteLine(
                FilterCategory.ChunkValidation,
                "Chunk created for " + x + ", " + y + ", " + z + ".");

            this.m_ChunkGenerator.Generate(this);
        }

        public ILevel Level { get; private set; }
        public Cell[,,] Cells { get; set; }
        public EnemyEntity[] Enemies { get; set; }
        public bool EntitiesInitialized { get; set; }
        public bool GraphicsEmpty { get; protected set; }
        public bool Generated { get; set; }
        public VertexPositionTexture[] GeneratedVertexes { get; set; }
        public int[] GeneratedIndices { get; set; }

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
                            this.Y,
                            this.Z);
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(
                            this.Level,
                            this.m_Octree,
                            this.X - (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth),
                            this.Y,
                            this.Z);
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
                            this.Y,
                            this.Z);
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(
                            this.Level,
                            this.m_Octree,
                            this.X + (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth),
                            this.Y,
                            this.Z);
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
                    var c = this.m_Octree.Get(
                        this.X,
                        this.Y,
                        this.Z - (this.m_ChunkSizePolicy.ChunkCellDepth * this.m_ChunkSizePolicy.CellVoxelDepth));
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(
                            this.Level,
                            this.m_Octree,
                            this.X,
                            this.Y,
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
                    var c = this.m_Octree.Get(
                        this.X,
                        this.Y,
                        this.Z + (this.m_ChunkSizePolicy.ChunkCellDepth * this.m_ChunkSizePolicy.CellVoxelDepth));
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(
                            this.Level,
                            this.m_Octree,
                            this.X,
                            this.Y,
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
                    var c = this.m_Octree.Get(
                        this.X,
                        this.Y - (this.m_ChunkSizePolicy.ChunkCellHeight * this.m_ChunkSizePolicy.CellVoxelHeight),
                        this.Z);
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(
                            this.Level,
                            this.m_Octree,
                            this.X,
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
                    var c = this.m_Octree.Get(
                        this.X,
                        this.Y + (this.m_ChunkSizePolicy.ChunkCellHeight * this.m_ChunkSizePolicy.CellVoxelHeight),
                        this.Z);
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(
                            this.Level,
                            this.m_Octree,
                            this.X,
                            this.Y + (this.m_ChunkSizePolicy.ChunkCellHeight * this.m_ChunkSizePolicy.CellVoxelHeight),
                            this.Z);
                    if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeLookup) && c == this)
                        throw new InvalidOperationException("Relative addressing of chunk resulted in same chunk.");
                    return c;
                }
            }
        }

        #endregion
        
        protected IAssetManager AssetManager { get; private set; }
        protected VertexBuffer VertexBuffer { get; set; }
        protected IndexBuffer IndexBuffer { get; set; }
        
        public virtual void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            // Overridden in ClientRuntimeChunk.
        }

        public void Dispose()
        {
            if (this.VertexBuffer != null)
                this.VertexBuffer.Dispose();
            if (this.IndexBuffer != null)
                this.IndexBuffer.Dispose();
            this.m_FilteredConsole = null;
            this.AssetManager = null;
            this.m_ChunkGenerator = null;
            this.Cells = null;
            this.Generated = false;
        }

        /// <summary>
        /// Saves the chunk back to disk.
        /// </summary>
        public void Save()
        {
            this.Level.SaveChunk(this);
        }

        /// <summary>
        /// Saves the state of the chunk to disk and purges it from memory.
        /// </summary>
        public void Purge()
        {
            this.Save();
            this.Dispose();
            if (this.m_Octree != null)
                this.m_Octree.Clear(this);
        }
    }
}
