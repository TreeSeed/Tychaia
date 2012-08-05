using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using System.Threading;
using Microsoft.Xna.Framework;
using Tychaia.Title;
using Microsoft.Xna.Framework.Graphics;

namespace Tychaia.Generators
{
    public class Chunk
    {
        public const int CHUNK_SIZE = 16;

        public const int Width = 16;
        public const int Height = 16;
        public const int Depth = 256;

        public Block[,,] m_Blocks = null;
        private Texture2D m_Texture = null;
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
                if (this.m_Left == null)
                {
                    if (this.m_Up != null && this.m_Up.m_Left != null && this.m_Up.m_Left.m_Down != null)
                        this.m_Left = this.m_Up.m_Left.m_Down;
                    else if (this.m_Down != null && this.m_Down.m_Left != null && this.m_Down.m_Left.m_Up != null)
                        this.m_Left = this.m_Down.m_Left.m_Up;
                    else
                        this.m_Left = new Chunk(this.GlobalX - Chunk.Width, this.GlobalY);
                    this.m_Left.m_Right = this;
                }
                return this.m_Left;
            }
        }

        public Chunk Right
        {
            get
            {
                if (this.m_Right == null)
                {
                    if (this.m_Up != null && this.m_Up.m_Right != null && this.m_Up.m_Right.m_Down != null)
                        this.m_Right = this.m_Up.m_Right.m_Down;
                    else if (this.m_Down != null && this.m_Down.m_Right != null && this.m_Down.m_Right.m_Up != null)
                        this.m_Right = this.m_Down.m_Right.m_Up;
                    else
                        this.m_Right = new Chunk(this.GlobalX + Chunk.Width, this.GlobalY);
                    this.m_Right.m_Left = this;
                }
                return this.m_Right;
            }
        }

        public Chunk Up
        {
            get
            {
                if (this.m_Up == null)
                {
                    if (this.m_Left != null && this.m_Left.m_Up != null && this.m_Left.m_Up.m_Right != null)
                        this.m_Up = this.m_Left.m_Up.m_Right;
                    else if (this.m_Right != null && this.m_Right.m_Up != null && this.m_Right.m_Up.m_Left != null)
                        this.m_Up = this.m_Right.m_Up.m_Left;
                    else
                        this.m_Up = new Chunk(this.GlobalX, this.GlobalY - Chunk.Height);
                    this.m_Up.m_Down = this;
                }
                return this.m_Up;
            }
        }

        public Chunk Down
        {
            get
            {
                if (this.m_Down == null)
                {
                    if (this.m_Left != null && this.m_Left.m_Down != null && this.m_Left.m_Down.m_Right != null)
                        this.m_Down = this.m_Left.m_Down.m_Right;
                    else if (this.m_Right != null && this.m_Right.m_Down != null && this.m_Right.m_Down.m_Left != null)
                        this.m_Down = this.m_Right.m_Down.m_Left;
                    else
                        this.m_Down = new Chunk(this.GlobalX, this.GlobalY + Chunk.Height);
                    this.m_Down.m_Up = this;
                }
                return this.m_Down;
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
                Bounds = new Rectangle(this.GlobalX, this.GlobalY, Chunk.Width, Chunk.Height)
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
