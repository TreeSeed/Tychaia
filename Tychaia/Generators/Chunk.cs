//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Tychaia.Disk;
using Tychaia.Globals;
using Protogame;
using Microsoft.Xna.Framework;

namespace Tychaia
{
    public class Chunk
    {
        private IFilteredConsole m_FilteredConsole;
        private IFilteredFeatures m_FilteredFeatures;
        private IChunkFactory m_ChunkFactory;
        private IChunkSizePolicy m_ChunkSizePolicy;
        private IRenderCache m_RenderCache;
        private IAssetManager m_AssetManager;
        private IChunkGenerator m_ChunkGenerator;

        public readonly long X;
        public readonly long Y;
        public readonly long Z;
        public BlockAsset[, ,] Blocks = null;
        private static object m_AccessLock = new object();
        private int m_Seed = MenuWorld.m_StaticSeed; // All chunks are generated from the same seed.
        private bool m_IsGenerating = false;
        private bool m_IsGenerated = false;
        private ChunkOctree m_Octree = null;
        private ILevel m_DiskLevel = null;

        public Chunk(
            ILevel level,
            ChunkOctree octree,
            IChunkFactory chunkFactory,
            IFilteredConsole filteredConsole,
            IFilteredFeatures filteredFeatures,
            IChunkSizePolicy chunkSizePolicy,
            IRenderCache renderCache,
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
            this.m_RenderCache = renderCache;
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
            this.m_ChunkGenerator = chunkGenerator;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.m_Octree.Set(this);
            this.Blocks = new BlockAsset[this.m_ChunkSizePolicy.ChunkCellWidth, this.m_ChunkSizePolicy.ChunkCellHeight, this.m_ChunkSizePolicy.ChunkCellDepth];
            this.m_DiskLevel = level;
            this.m_FilteredConsole.WriteLine(FilterCategory.ChunkValidation, "Chunk created for " + x + ", " + y + ", " + z + ".");
            
            this.m_ChunkGenerator.Generate(this);
        }
        
        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
                return;
            
            // FIXME: The check always returns "Disjoint"...
            /*
            // Check if this chunk is even within the camera's view.
            var min = new Vector3(this.X, this.Y, this.Z);
            var max = min + new Vector3(
                this.m_ChunkSizePolicy.ChunkCellWidth,
                this.m_ChunkSizePolicy.ChunkCellHeight,
                this.m_ChunkSizePolicy.ChunkCellDepth);
            var scale = Matrix.CreateScale(
                this.m_ChunkSizePolicy.CellVoxelWidth,
                this.m_ChunkSizePolicy.CellVoxelHeight,
                this.m_ChunkSizePolicy.CellVoxelDepth);
            min = Vector3.Transform(min, scale);
            max = Vector3.Transform(max, scale);
            var bounds = new Microsoft.Xna.Framework.BoundingBox(min, max);
            if (renderContext.BoundingFrustrum.Contains(bounds) == ContainmentType.Disjoint)
            {
                Console.WriteLine("Skipping render of " + this.X + "," + this.Y + ","+this.Z + " because it's outside the camera's bounds.");
                return;
            }
            */
        
            // TODO: Determine the structure of a chunk and cache it in the render cache.
            for (var x = 0; x < this.m_ChunkSizePolicy.ChunkCellWidth; x++)
            for (var y = 0; y < this.m_ChunkSizePolicy.ChunkCellHeight; y++)
            for (var z = 0; z < this.m_ChunkSizePolicy.ChunkCellDepth; z++)
            {
                var block = this.Blocks[x, y, z];
                if (block == null)
                    continue;
                block.Render(
                    renderContext,
                    this.m_RenderCache,
                    this.m_ChunkSizePolicy,
                    new Vector3(
                        x * this.m_ChunkSizePolicy.CellVoxelWidth,
                        y * this.m_ChunkSizePolicy.CellVoxelHeight,
                        z * this.m_ChunkSizePolicy.CellVoxelDepth) +
                    new Vector3(this.X, this.Y, this.Z));
            }
        }
        
        #region Relative Addressing
        
        public Chunk West
        {
            get
            {
                lock (m_AccessLock)
                {
                    Chunk c = this.m_Octree.Get(this.X - (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth), this.Y, this.Z);
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_DiskLevel, this.m_Octree, this.X - (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth), this.Y, this.Z);
                    else if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeLookup) && c == this)
                        throw new InvalidOperationException("Relative addressing of chunk resulted in same chunk.");
                    else
                        return c;
                }
            }
        }

        public Chunk East
        {
            get
            {
                lock (m_AccessLock)
                {
                    Chunk c = this.m_Octree.Get(this.X + (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth), this.Y, this.Z);
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_DiskLevel, this.m_Octree, this.X + (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.CellVoxelWidth), this.Y, this.Z);
                    else if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeLookup) && c == this)
                        throw new InvalidOperationException("Relative addressing of chunk resulted in same chunk.");
                    else
                        return c;
                }
            }
        }

        public Chunk North
        {
            get
            {
                lock (m_AccessLock)
                {
                    Chunk c = this.m_Octree.Get(this.X, this.Y, this.Z - (this.m_ChunkSizePolicy.ChunkCellDepth * this.m_ChunkSizePolicy.CellVoxelDepth));
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_DiskLevel, this.m_Octree, this.X, this.Y, this.Z - (this.m_ChunkSizePolicy.ChunkCellDepth * this.m_ChunkSizePolicy.CellVoxelDepth));
                    else if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeLookup) && c == this)
                        throw new InvalidOperationException("Relative addressing of chunk resulted in same chunk.");
                    else
                        return c;
                }
            }
        }

        public Chunk South
        {
            get
            {
                lock (m_AccessLock)
                {
                    Chunk c = this.m_Octree.Get(this.X, this.Y, this.Z + (this.m_ChunkSizePolicy.ChunkCellDepth * this.m_ChunkSizePolicy.CellVoxelDepth));
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_DiskLevel, this.m_Octree, this.X, this.Y, this.Z + (this.m_ChunkSizePolicy.ChunkCellDepth * this.m_ChunkSizePolicy.CellVoxelDepth));
                    else if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeLookup) && c == this)
                        throw new InvalidOperationException("Relative addressing of chunk resulted in same chunk.");
                    else
                        return c;
                }
            }
        }

        public Chunk Up
        {
            get
            {
                lock (m_AccessLock)
                {
                    Chunk c = this.m_Octree.Get(this.X, this.Y - (this.m_ChunkSizePolicy.ChunkCellHeight * this.m_ChunkSizePolicy.CellVoxelHeight), this.Z);
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_DiskLevel, this.m_Octree, this.X, this.Y - (this.m_ChunkSizePolicy.ChunkCellHeight * this.m_ChunkSizePolicy.CellVoxelHeight), this.Z);
                    else if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeLookup) && c == this)
                        throw new InvalidOperationException("Relative addressing of chunk resulted in same chunk.");
                    else
                        return c;
                }
            }
        }

        public Chunk Down
        {
            get
            {
                lock (m_AccessLock)
                {
                    Chunk c = this.m_Octree.Get(this.X, this.Y + (this.m_ChunkSizePolicy.ChunkCellHeight * this.m_ChunkSizePolicy.CellVoxelHeight), this.Z);
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_DiskLevel, this.m_Octree, this.X, this.Y + (this.m_ChunkSizePolicy.ChunkCellHeight * this.m_ChunkSizePolicy.CellVoxelHeight), this.Z);
                    else if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeLookup) && c == this)
                        throw new InvalidOperationException("Relative addressing of chunk resulted in same chunk.");
                    else
                        return c;
                }
            }
        }
        
        #endregion
    }

    public class ChunkInfo
    {
        public ChunkInfo()
        {
            this.Objects = new List<object>();
        }

        public ILevel LevelDisk
        {
            get;
            set;
        }

        public int Seed
        {
            get;
            set;
        }

        public Random Random
        {
            get;
            set;
        }

        public Cube Bounds
        {
            get;
            set;
        }

        public List<object> Objects
        {
            get;
            private set;
        }
    }
}
