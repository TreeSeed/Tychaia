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

namespace Tychaia.Generators
{
    public class Chunk
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
        private Chunk m_West = null;
        private Chunk m_East = null;
        private Chunk m_North = null;
        private Chunk m_South = null;
        private Chunk m_Up = null;
        private Chunk m_Down = null;
        private bool m_IsGenerating = false;
        private bool m_IsGenerated = false;
        private ChunkRenderer.RenderTask m_RenderTask = null;
        private UniqueRenderCache.UniqueRender m_UniqueRender = null;

        public Chunk(int x, int y, int z)
        {
            this.GlobalX = x;
            this.GlobalY = y;
            this.GlobalZ = z;
            this.m_Blocks = new Block[Chunk.Width, Chunk.Height, Chunk.Depth];
            this.m_RawData = new int[Chunk.Width * Chunk.Height * Chunk.Depth];
            this.Generate();
            FilteredConsole.WriteLine(FilterCategory.ChunkValidation, "Chunk created for " + x + ", " + y + ", " + z + ".");
        }

        public int GlobalX;
        public int GlobalY;
        public int GlobalZ;

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
            FilteredConsole.WriteLine(FilterCategory.GraphicsMemoryUsage, "Textures discarded for chunk " + this.GlobalX + ", " + this.GlobalY + ".");
        }

        public Chunk West
        {
            get
            {
                lock (m_AccessLock)
                {
                    if (this.m_West == null)
                    {
                        Chunk c = this.FindChunk(this.GlobalX - (Chunk.Width * Scale.CUBE_X), this.GlobalY, this.GlobalZ);
                        if (c == null)
                            this.m_West = new Chunk(this.GlobalX - (Chunk.Width * Scale.CUBE_X), this.GlobalY, this.GlobalZ);
                        else
                            this.m_West = c;
                        this.m_West.m_East = this;
                    }
                    return this.m_West;
                }
            }
        }

        public Chunk East
        {
            get
            {
                lock (m_AccessLock)
                {
                    if (this.m_East == null)
                    {
                        Chunk c = this.FindChunk(this.GlobalX + (Chunk.Width * Scale.CUBE_X), this.GlobalY, this.GlobalZ);
                        if (c == null)
                            this.m_East = new Chunk(this.GlobalX + (Chunk.Width * Scale.CUBE_X), this.GlobalY, this.GlobalZ);
                        else
                            this.m_East = c;
                        this.m_East.m_West = this;
                    }
                    return this.m_East;
                }
            }
        }

        public Chunk North
        {
            get
            {
                lock (m_AccessLock)
                {
                    if (this.m_North == null)
                    {
                        Chunk c = this.FindChunk(this.GlobalX, this.GlobalY - (Chunk.Height * Scale.CUBE_Y), this.GlobalZ);
                        if (c == null)
                            this.m_North = new Chunk(this.GlobalX, this.GlobalY - (Chunk.Height * Scale.CUBE_Y), this.GlobalZ);
                        else
                            this.m_North = c;
                        this.m_North.m_South = this;
                    }
                    return this.m_North;
                }
            }
        }

        public Chunk South
        {
            get
            {
                lock (m_AccessLock)
                {
                    if (this.m_South == null)
                    {
                        Chunk c = this.FindChunk(this.GlobalX, this.GlobalY + (Chunk.Height * Scale.CUBE_Y), this.GlobalZ);
                        if (c == null)
                            this.m_South = new Chunk(this.GlobalX, this.GlobalY + (Chunk.Height * Scale.CUBE_Y), this.GlobalZ);
                        else
                            this.m_South = c;
                        this.m_South.m_North = this;
                    }
                    return this.m_South;
                }
            }
        }

        public Chunk Up
        {
            get
            {
                lock (m_AccessLock)
                {
                    if (this.m_Up == null)
                    {
                        Chunk c = this.FindChunk(this.GlobalX, this.GlobalY, this.GlobalZ + (Chunk.Depth * Scale.CUBE_Z));
                        if (c == null)
                            this.m_Up = new Chunk(this.GlobalX, this.GlobalY, this.GlobalZ + (Chunk.Depth * Scale.CUBE_Z));
                        else
                            this.m_Up = c;
                        this.m_Up.m_Down = this;
                    }
                    return this.m_Up;
                }
            }
        }

        public Chunk Down
        {
            get
            {
                lock (m_AccessLock)
                {
                    if (this.m_Down == null)
                    {
                        Chunk c = this.FindChunk(this.GlobalX, this.GlobalY, this.GlobalZ - (Chunk.Depth * Scale.CUBE_Z));
                        if (c == null)
                            this.m_Down = new Chunk(this.GlobalX, this.GlobalY, this.GlobalZ - (Chunk.Depth * Scale.CUBE_Z));
                        else
                            this.m_Down = c;
                        this.m_Down.m_Up = this;
                    }
                    return this.m_Down;
                }
            }
        }

        private Chunk FindChunk(int x, int y, int z, List<Chunk> visited = null)
        {
            if (visited == null)
                visited = new List<Chunk>();
            visited.Add(this);
            if (this.GlobalX == x && this.GlobalY == y && this.GlobalZ == z)
                return this;
            if (this.m_South != null)
                if (!visited.Contains(this.m_South))
                {
                    Chunk c = this.m_South.FindChunk(x, y, z, visited);
                    if (c != null)
                        return c;
                }
            if (this.m_West != null)
                if (!visited.Contains(this.m_West))
                {
                    Chunk c = this.m_West.FindChunk(x, y, z, visited);
                    if (c != null)
                        return c;
                }
            if (this.m_East != null)
                if (!visited.Contains(this.m_East))
                {
                    Chunk c = this.m_East.FindChunk(x, y, z, visited);
                    if (c != null)
                        return c;
                }
            if (this.m_North != null)
                if (!visited.Contains(this.m_North))
                {
                    Chunk c = this.m_North.FindChunk(x, y, z, visited);
                    if (c != null)
                        return c;
                }
            if (this.m_Up != null)
                if (!visited.Contains(this.m_Up))
                {
                    Chunk c = this.m_Up.FindChunk(x, y, z, visited);
                    if (c != null)
                        return c;
                }
            if (this.m_Down != null)
                if (!visited.Contains(this.m_Down))
                {
                    Chunk c = this.m_Down.FindChunk(x, y, z, visited);
                    if (c != null)
                        return c;
                }
            return null;
        }

        public void Validate(List<Chunk> visited = null)
        {
            if (visited == null)
                visited = new List<Chunk>();
            visited.Add(this);
            if (this.m_South != null)
            {
                if (this.m_South.m_North != this)
                    FilteredConsole.WriteLine(FilterCategory.ChunkValidation, "Chunk validation failed for down -> up -> down.");
                else if (!visited.Contains(this.m_South))
                    this.m_South.Validate(visited);
            }
            if (this.m_West != null)
            {
                if (this.m_West.m_East != this)
                    FilteredConsole.WriteLine(FilterCategory.ChunkValidation, "Chunk validation failed for left -> right -> left.");
                else if (!visited.Contains(this.m_West))
                    this.m_West.Validate(visited);
            }
            if (this.m_East != null)
            {
                if (this.m_East.m_West != this)
                    FilteredConsole.WriteLine(FilterCategory.ChunkValidation, "Chunk validation failed for right -> left -> right.");
                else if (!visited.Contains(this.m_East))
                    this.m_East.Validate(visited);
            }
            if (this.m_North != null)
            {
                if (this.m_North.m_South != this)
                    FilteredConsole.WriteLine(FilterCategory.ChunkValidation, "Chunk validation failed for up -> down -> up.");
                else if (!visited.Contains(this.m_North))
                    this.m_North.Validate(visited);
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
                Bounds = new Cube(this.GlobalX / Scale.CUBE_X, this.GlobalY / Scale.CUBE_Y, this.GlobalZ / Scale.CUBE_Z, 
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
