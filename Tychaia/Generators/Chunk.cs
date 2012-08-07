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

        public const int Width = 16;
        public const int Height = 16;
        public const int Depth = 256;

        public Block[, ,] m_Blocks = null;
        private static object m_AccessLock = new object();
        private object m_BlocksLock = new object();
        private int m_Seed = TitleWorld.m_StaticSeed; // All chunks are generated from the same seed.
        private Chunk m_Left = null;
        private Chunk m_Right = null;
        private Chunk m_Up = null;
        private Chunk m_Down = null;
        private bool m_IsGenerating = false;
        private bool m_IsGenerated = false;
        private ChunkRenderer.RenderTask m_RenderTask = null;

        public Chunk(int x, int y)
        {
            this.GlobalX = x;
            this.GlobalY = y;
            this.m_Blocks = new Block[Chunk.Width, Chunk.Height, Chunk.Depth];
            this.Generate();
            FilteredConsole.WriteLine(FilterCategory.ChunkValidation, "Chunk created for " + x + ", " + y + ".");
        }

        public int GlobalX;
        public int GlobalY;

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
                if (this.m_RenderTask == null || (this.m_RenderTask.Result == null && this.m_RenderTask.HasResult))
                    this.m_RenderTask = ChunkRenderer.PushForRendering(this, Static.GameContext);
                if (!this.m_RenderTask.HasResult)
                    return null;
                return this.m_RenderTask.Result;
            }
        }

        public void DiscardTexture()
        {
            // Force the graphics texture to be discarded.
            RenderTarget2D target = this.m_RenderTask.Result;
            this.m_RenderTask = null;
            target.Dispose();
        }

        public Chunk Left
        {
            get
            {
                lock (m_AccessLock)
                {
                    if (this.m_Left == null)
                    {
                        Chunk c = this.FindChunk(this.GlobalX - (Chunk.Width * Scale.CUBE_X), this.GlobalY);
                        if (c == null)
                            this.m_Left = new Chunk(this.GlobalX - (Chunk.Width * Scale.CUBE_X), this.GlobalY);
                        else
                            this.m_Left = c;
                        this.m_Left.m_Right = this;
                    }
                    return this.m_Left;
                }
            }
        }

        public Chunk Right
        {
            get
            {
                lock (m_AccessLock)
                {
                    if (this.m_Right == null)
                    {
                        Chunk c = this.FindChunk(this.GlobalX + (Chunk.Width * Scale.CUBE_X), this.GlobalY);
                        if (c == null)
                            this.m_Right = new Chunk(this.GlobalX + (Chunk.Width * Scale.CUBE_X), this.GlobalY);
                        else
                            this.m_Right = c;
                        this.m_Right.m_Left = this;
                    }
                    return this.m_Right;
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
                        Chunk c = this.FindChunk(this.GlobalX, this.GlobalY - (Chunk.Height * Scale.CUBE_Y));
                        if (c == null)
                            this.m_Up = new Chunk(this.GlobalX, this.GlobalY - (Chunk.Height * Scale.CUBE_Y));
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
                        Chunk c = this.FindChunk(this.GlobalX, this.GlobalY + (Chunk.Height * Scale.CUBE_Y));
                        if (c == null)
                            this.m_Down = new Chunk(this.GlobalX, this.GlobalY + (Chunk.Height * Scale.CUBE_Y));
                        else
                            this.m_Down = c;
                        this.m_Down.m_Up = this;
                    }
                    return this.m_Down;
                }
            }
        }

        private Chunk FindChunk(int x, int y, List<Chunk> visited = null)
        {
            if (visited == null)
                visited = new List<Chunk>();
            visited.Add(this);
            if (this.GlobalX == x && this.GlobalY == y)
                return this;
            if (this.m_Down != null)
                if (!visited.Contains(this.m_Down))
                {
                    Chunk c = this.m_Down.FindChunk(x, y, visited);
                    if (c != null)
                        return c;
                }
            if (this.m_Left != null)
                if (!visited.Contains(this.m_Left))
                {
                    Chunk c = this.m_Left.FindChunk(x, y, visited);
                    if (c != null)
                        return c;
                }
            if (this.m_Right != null)
                if (!visited.Contains(this.m_Right))
                {
                    Chunk c = this.m_Right.FindChunk(x, y, visited);
                    if (c != null)
                        return c;
                }
            if (this.m_Up != null)
                if (!visited.Contains(this.m_Up))
                {
                    Chunk c = this.m_Up.FindChunk(x, y, visited);
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
            if (this.m_Down != null)
            {
                if (this.m_Down.m_Up != this)
                    FilteredConsole.WriteLine(FilterCategory.ChunkValidation, "Chunk validation failed for down -> up -> down.");
                else if (!visited.Contains(this.m_Down))
                    this.m_Down.Validate(visited);
            }
            if (this.m_Left != null)
            {
                if (this.m_Left.m_Right != this)
                    FilteredConsole.WriteLine(FilterCategory.ChunkValidation, "Chunk validation failed for left -> right -> left.");
                else if (!visited.Contains(this.m_Left))
                    this.m_Left.Validate(visited);
            }
            if (this.m_Right != null)
            {
                if (this.m_Right.m_Left != this)
                    FilteredConsole.WriteLine(FilterCategory.ChunkValidation, "Chunk validation failed for right -> left -> right.");
                else if (!visited.Contains(this.m_Right))
                    this.m_Right.Validate(visited);
            }
            if (this.m_Up != null)
            {
                if (this.m_Up.m_Down != this)
                    FilteredConsole.WriteLine(FilterCategory.ChunkValidation, "Chunk validation failed for up -> down -> up.");
                else if (!visited.Contains(this.m_Up))
                    this.m_Up.Validate(visited);
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
                Bounds = new Rectangle(this.GlobalX / Scale.CUBE_X, this.GlobalY / Scale.CUBE_Y, Chunk.Width, Chunk.Height)
            };
            ChunkProvider.FillChunk(this, this.m_Blocks, i, () =>
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

        public Rectangle Bounds
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
