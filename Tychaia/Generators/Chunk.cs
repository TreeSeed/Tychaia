using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using System.Threading;
using Microsoft.Xna.Framework;
using Tychaia.Title;
using Microsoft.Xna.Framework.Graphics;
using Tychaia.Globals;
using Protogame.Structure;

namespace Tychaia.Generators
{
    public class Chunk : SpatialNode
    {
        public const int CHUNK_SIZE = 16;

        public const int Width = 8;
        public const int Height = 8;
        public const int Depth = 8;

        public Block[, ,] m_Blocks = null;
        public int[] m_RawData = null;
        private static object m_AccessLock = new object();
        private object m_BlocksLock = new object();
        private int m_Seed = TitleWorld.m_StaticSeed; // All chunks are generated from the same seed.
        private bool m_IsGenerating = false;
        private bool m_IsGenerated = false;
        private ChunkRenderer.RenderTask m_RenderTask = null;
        private UniqueRenderCache.UniqueRender m_UniqueRender = null;
        private ChunkOctree m_Octree = null;

        public Chunk(ChunkOctree octree, long x, long y, long z)
        {
            this.m_Octree = octree;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.m_Blocks = new Block[Chunk.Width, Chunk.Height, Chunk.Depth];
            this.m_RawData = new int[Chunk.Width * Chunk.Height * Chunk.Depth];
            this.Generate();
            FilteredConsole.WriteLine(FilterCategory.ChunkValidation, "Chunk created for " + x + ", " + y + ", " + z + ".");
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
                                this.m_RenderTask = ChunkRenderer.PushForRendering(this, Static.GameContext);
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
                        this.m_RenderTask = ChunkRenderer.PushForRendering(this, Static.GameContext);
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
            }
        }

        public Texture2D DepthMap
        {
            get
            {
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
                        this.m_RenderTask = ChunkRenderer.PushForRendering(this, Static.GameContext);
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
            }
        }

        public void DiscardTexture()
        {
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
        }

        public Chunk West
        {
            get
            {
                lock (m_AccessLock)
                {
                    Chunk c = this.m_Octree.Get(this.X - (Chunk.Width * Scale.CUBE_X), this.Y, this.Z);
                    if (c == null)
                        return new Chunk(this.m_Octree, this.X - (Chunk.Width * Scale.CUBE_X), this.Y, this.Z);
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
                        return new Chunk(this.m_Octree, this.X + (Chunk.Width * Scale.CUBE_X), this.Y, this.Z);
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
                    Chunk c = this.m_Octree.Get(this.X, this.Y - (Chunk.Width * Scale.CUBE_X), this.Z);
                    if (c == null)
                        return new Chunk(this.m_Octree, this.X, this.Y - (Chunk.Width * Scale.CUBE_X), this.Z);
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
                    Chunk c = this.m_Octree.Get(this.X, this.Y + (Chunk.Width * Scale.CUBE_X), this.Z);
                    if (c == null)
                        return new Chunk(this.m_Octree, this.X, this.Y + (Chunk.Width * Scale.CUBE_X), this.Z);
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
                    Chunk c = this.m_Octree.Get(this.X, this.Y, this.Z - (Chunk.Width * Scale.CUBE_X));
                    if (c == null)
                        return new Chunk(this.m_Octree, this.X, this.Y, this.Z - (Chunk.Width * Scale.CUBE_X));
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
                    Chunk c = this.m_Octree.Get(this.X, this.Y, this.Z + (Chunk.Width * Scale.CUBE_X));
                    if (c == null)
                        return new Chunk(this.m_Octree, this.X, this.Y, this.Z + (Chunk.Width * Scale.CUBE_X));
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
                Seed = this.m_Seed,
                Random = new Random(this.m_Seed),
                Bounds = new Cube(this.X / Scale.CUBE_X, this.Y / Scale.CUBE_Y, this.Z / Scale.CUBE_Z, 
                    Chunk.Width, Chunk.Height, Chunk.Depth)
            };
            ChunkProvider.FillChunk(this, this.m_RawData, this.m_Blocks, i, () =>
                {
                    this.m_IsGenerating = false;
                }, () =>
                {
                    this.m_IsGenerated = true;
                    this.m_IsGenerating = false;
                });
        }
    }

    public class ChunkInfo
    {
        public ChunkInfo()
        {
            this.Objects = new List<object>();
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
