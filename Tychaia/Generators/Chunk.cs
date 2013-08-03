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

namespace Tychaia
{
    public class Chunk
    {
        private IFilteredConsole m_FilteredConsole;
        private IFilteredFeatures m_FilteredFeatures;
        private IChunkFactory m_ChunkFactory;
    
        public const int CHUNK_SIZE = 16;

        public const int Width = ChunkSize.Width;
        public const int Height = ChunkSize.Height;
        public const int Depth = ChunkSize.Depth;

        public readonly long X;
        public readonly long Y;
        public readonly long Z;
        public BlockAsset[, ,] m_Blocks = null;
        public int[] m_RawData = null;
        private static object m_AccessLock = new object();
        private int m_Seed = MenuWorld.m_StaticSeed; // All chunks are generated from the same seed.
        private bool m_IsGenerating = false;
        private bool m_IsGenerated = false;
        #if NOT_MIGRATED
        private ChunkRenderer.RenderTask m_RenderTask = null;
        private UniqueRenderCache.UniqueRender m_UniqueRender = null;
        #endif
        private ChunkOctree m_Octree = null;
        private ILevel m_DiskLevel = null;

        public Chunk(
            ILevel level,
            ChunkOctree octree,
            IChunkFactory chunkFactory,
            IFilteredConsole filteredConsole,
            IFilteredFeatures filteredFeatures,
            long x,
            long y,
            long z)
        {
            this.m_Octree = octree;
            this.m_FilteredConsole = filteredConsole;
            this.m_FilteredFeatures = filteredFeatures;
            this.m_ChunkFactory = chunkFactory;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.m_Octree.Set(this);
            this.m_Blocks = new BlockAsset[Chunk.Width, Chunk.Height, Chunk.Depth];
            this.m_RawData = new int[Chunk.Width * Chunk.Height * Chunk.Depth];
            this.m_DiskLevel = level;
            this.m_FilteredConsole.WriteLine(FilterCategory.ChunkValidation, "Chunk created for " + x + ", " + y + ", " + z + ".");
            this.Generate();
        }

        public bool IsGenerated
        {
            get
            {
                return this.m_IsGenerated;
            }
        }

        public Texture2D Texture
        {
            get
            {
                #if NOT_MIGRATED
                if (!this.m_IsGenerated)
                {
                    this.Generate();
                    return null;
                }
                if (this.m_UniqueRender != null && this.m_UniqueRender.Target == null &&
                    this.m_RenderTask != null && this.m_RenderTask.Result != null)
                {
                    // FIXME: Unsure how this situation occurs; we have a render task that we should
                    // release.
                    this.DiscardTexture();
                }
                if ((this.m_UniqueRender == null || this.m_UniqueRender.Target == null) &&
                    (this.m_RenderTask == null || (this.m_RenderTask.Result == null && this.m_RenderTask.HasResult)))
                {
                    if (UniqueRenderCache.Has(this.m_RawData))
                    {
                        if (UniqueRenderCache.IsWaiting(this.m_RawData))
                        {
                            if (ChunkRenderer.IsTasked(this))
                            {
                                // We're waiting on the render to finish.
                                return null;
                            }
                            else
                            {
                                // The unique render cache thinks we're rendering, but
                                // in reality it's been previously optimized out.  Therefore
                                // we need to restart the rendering task.
                                this.m_RenderTask = ChunkRenderer.PushForRendering(this, null/*Static.GameContext*/);
                            }
                        }
                        else
                        {
                            // The render is ready.
                            this.m_UniqueRender = UniqueRenderCache.Grab(this.m_RawData);
                        }
                    }
                    else
                    {
                        // There is no render even available for this data; start
                        // processing for it.
                        UniqueRenderCache.StoreWaiting(this.m_RawData);
                        this.m_RenderTask = ChunkRenderer.PushForRendering(this, null/*Static.GameContext*/);
                    }
                }
                if (this.m_UniqueRender != null)
                    return this.m_UniqueRender.Target;
                if (!this.m_RenderTask.HasResult)
                    return null;
                // Move into unique render storage.
                this.m_UniqueRender = UniqueRenderCache.Store(this.m_RawData, this.m_RenderTask.Result, this.m_RenderTask.DepthMap);
                this.m_RenderTask = null;
                return this.m_UniqueRender.Target;
                #else
                return null;
                #endif
            }
        }

        public Texture2D DepthMap
        {
            get
            {
                #if NOT_MIGRATED
                if (!this.m_IsGenerated)
                {
                    this.Generate();
                    return null;
                }
                if ((this.m_UniqueRender == null || this.m_UniqueRender.DepthMap == null) &&
                    (this.m_RenderTask == null || (this.m_RenderTask.Result == null && this.m_RenderTask.HasResult)))
                {
                    if (UniqueRenderCache.Has(this.m_RawData))
                    {
                        if (UniqueRenderCache.IsWaiting(this.m_RawData))
                        {
                            // We're waiting on the render to finish.
                            return null;
                        }
                        else
                        {
                            // The render is ready.
                            this.m_UniqueRender = UniqueRenderCache.Grab(this.m_RawData);
                        }
                    }
                    else
                    {
                        // There is no render even available for this data; start
                        // processing for it.
                        UniqueRenderCache.StoreWaiting(this.m_RawData);
                        this.m_RenderTask = ChunkRenderer.PushForRendering(this, null/*Static.GameContext*/);
                    }
                }
                if (this.m_UniqueRender != null)
                    return this.m_UniqueRender.DepthMap;
                if (!this.m_RenderTask.HasResult)
                    return null;
                // Move into unique render storage.
                this.m_UniqueRender = UniqueRenderCache.Store(this.m_RawData, this.m_RenderTask.Result, this.m_RenderTask.DepthMap);
                this.m_RenderTask = null;
                return this.m_UniqueRender.DepthMap;
                #else
                return null;
                #endif
            }
        }

        public void DiscardTexture()
        {
            #if NOT_MIGRATED
            // Force the graphics texture to be discarded.
            if (this.m_RenderTask != null)
            {
                // The texture was rendered, but not pushed into the unique
                // render cache so we free it directly.
                RenderTarget2D target = this.m_RenderTask.Result;
                RenderTarget2D depth = this.m_RenderTask.DepthMap;
                this.m_RenderTask = null;
                if (target != null)
                    target.Dispose();
                if (depth != null)
                    depth.Dispose();
            }
            else if (this.m_UniqueRender != null && (this.m_UniqueRender.Target != null || this.m_UniqueRender.DepthMap != null))
                // Release from the unique render cache.
                UniqueRenderCache.Release(this.m_RawData);

            // Send message about texture being discarded.
            FilteredConsole.WriteLine(FilterCategory.GraphicsMemoryUsage, "Textures discarded for chunk " + this.X + ", " + this.Y + ".");
            #endif
        }

        public Chunk West
        {
            get
            {
                lock (m_AccessLock)
                {
                    Chunk c = this.m_Octree.Get(this.X - (Chunk.Width * Scale.CUBE_X), this.Y, this.Z);
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_DiskLevel, this.m_Octree, this.X - (Chunk.Width * Scale.CUBE_X), this.Y, this.Z);
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
                    Chunk c = this.m_Octree.Get(this.X + (Chunk.Width * Scale.CUBE_X), this.Y, this.Z);
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_DiskLevel, this.m_Octree, this.X + (Chunk.Width * Scale.CUBE_X), this.Y, this.Z);
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
                    Chunk c = this.m_Octree.Get(this.X, this.Y - (Chunk.Height * Scale.CUBE_Y), this.Z);
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_DiskLevel, this.m_Octree, this.X, this.Y - (Chunk.Height * Scale.CUBE_Y), this.Z);
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
                    Chunk c = this.m_Octree.Get(this.X, this.Y + (Chunk.Height * Scale.CUBE_Y), this.Z);
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_DiskLevel, this.m_Octree, this.X, this.Y + (Chunk.Height * Scale.CUBE_Y), this.Z);
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
                    Chunk c = this.m_Octree.Get(this.X, this.Y, this.Z - (Chunk.Depth * Scale.CUBE_Z));
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_DiskLevel, this.m_Octree, this.X, this.Y, this.Z - (Chunk.Depth * Scale.CUBE_Z));
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
                    Chunk c = this.m_Octree.Get(this.X, this.Y, this.Z + (Chunk.Depth * Scale.CUBE_Z));
                    if (c == null)
                        return this.m_ChunkFactory.CreateChunk(this.m_DiskLevel, this.m_Octree, this.X, this.Y, this.Z + (Chunk.Depth * Scale.CUBE_Z));
                    else if (this.m_FilteredFeatures.IsEnabled(Feature.DebugOctreeLookup) && c == this)
                        throw new InvalidOperationException("Relative addressing of chunk resulted in same chunk.");
                    else
                        return c;
                }
            }
        }

        private void Generate()
        {
            if (this.m_IsGenerated || this.m_IsGenerating)
                return;
            this.m_IsGenerating = true;
            ChunkInfo i = new ChunkInfo()
            {
                LevelDisk = this.m_DiskLevel,
                Seed = this.m_Seed,
                Random = new Random(this.m_Seed),
                Bounds = new Cube(this.X / Scale.CUBE_X, this.Y / Scale.CUBE_Y, this.Z / Scale.CUBE_Z,
                    Chunk.Width, Chunk.Height, Chunk.Depth)
            };
            #if NOT_MIGRATED
            ChunkProvider.FillChunk(this, this.m_RawData, this.m_Blocks, i, () =>
                {
                    this.m_IsGenerating = false;
                }, () =>
                {
                    this.m_IsGenerated = true;
                    this.m_IsGenerating = false;
                });
            #endif
        }
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
