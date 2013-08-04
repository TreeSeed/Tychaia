using System;
using Protogame;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Tychaia.Threading;
using System.Collections;
using Tychaia.Globals;
using Microsoft.Xna.Framework;

namespace Tychaia
{
    public class ChunkRenderer
    {
        private ICellRenderOrderCalculator m_CellRenderOrderCalculator;
        private IRenderTargetFactory m_RenderTargetFactory;
        private IChunkSizePolicy m_ChunkSizePolicy;
        private IFilteredConsole m_FilteredConsole;
        private IFilteredFeatures m_FilteredFeatures;
    
        private IEnumerator m_Current;
        private InlineTaskPipeline<RenderTask> m_Pipeline;
        
        private static Random m_DebugRandomizer = new Random();
    
        public ChunkRenderer(
            ICellRenderOrderCalculator cellRenderOrderCalculator,
            IRenderTargetFactory renderTargetFactory,
            IChunkSizePolicy chunkSizePolicy,
            IFilteredConsole filteredConsole,
            IFilteredFeatures filteredFeatures)
        {
            this.m_CellRenderOrderCalculator = cellRenderOrderCalculator;
            this.m_RenderTargetFactory = renderTargetFactory;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_FilteredConsole = filteredConsole;
            this.m_FilteredFeatures = filteredFeatures;
            
            this.m_Pipeline = new InlineTaskPipeline<RenderTask>();
        }

        #region State classes

        public class RenderTask
        {
            public bool HasResult;
            public RenderTarget2D Result;
            public RenderTarget2D DepthMap;
            public Chunk Chunk;
            public Dictionary<string, Texture2D> Textures;
        }

        public class RenderState
        {
            public int ZTop;
            public int ZBottom;
            public int CurrentZ;
            public int RenderMode;
            public bool ChunkDepthMapCleared;
            public RenderTarget2D ChunkTarget;
            public RenderTarget2D ChunkDepthMap;
            public SpriteBatch SpriteBatch;
            public RenderTask RenderTask;
            public int[] CellRenderOrder;
        }
        
        #endregion
        
        public void Process(IGameContext gameContext)
        {
            // If we don't have a render state, pick a task off the list.
            if (this.m_Current == null)
            {
                var renderTask = this.m_Pipeline.Take();
                if (renderTask == null)
                    return;
                this.m_Current = this.Render(gameContext, renderTask).GetEnumerator();
            }
            
            // Use the yielding enumerable to process the current task in
            // small parts.  Once our game time increases over 100ms, we
            // break and keep our enumerable around for continuation.
            var start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < 100)
            {
                if (!this.m_Current.MoveNext())
                    break;
            }
            this.m_Current = null;
        }
        
        private IEnumerable<object> Render(IGameContext gameContext, RenderTask task)
        {
            // Create the render targets that we're going to be placing our content onto.
            var chunkTarget = this.m_RenderTargetFactory.Create(
                gameContext.Graphics.GraphicsDevice,
                this.m_ChunkSizePolicy.ChunkTopPixelWidth,
                this.m_ChunkSizePolicy.CellTextureTopPixelHeight * this.m_ChunkSizePolicy.ChunkCellWidth + this.m_ChunkSizePolicy.CellCubePixelHeight * this.m_ChunkSizePolicy.ChunkCellDepth,
                true,
                SurfaceFormat.Bgra5551,
                DepthFormat.None);
            var chunkDepthMap = this.m_RenderTargetFactory.Create(
                gameContext.Graphics.GraphicsDevice,
                this.m_ChunkSizePolicy.ChunkTopPixelWidth,
                this.m_ChunkSizePolicy.CellTextureTopPixelHeight * this.m_ChunkSizePolicy.ChunkCellWidth + this.m_ChunkSizePolicy.CellCubePixelHeight * this.m_ChunkSizePolicy.ChunkCellDepth,
                true,
                SurfaceFormat.Bgra5551,
                DepthFormat.None);
            this.m_FilteredConsole.WriteLine(
                FilterCategory.GraphicsMemoryUsage,
                "Allocated textures for chunk " + task.Chunk.X + ", " + task.Chunk.Y + ", " + task.Chunk.Z + ".");
            
            // Clear the background of the chunk and depth map targets.
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(chunkTarget);
            if (this.m_FilteredFeatures.IsEnabled(Feature.DebugChunkBackground))
            {
                var c = new Color(
                    (float)m_DebugRandomizer.NextDouble(),
                    (float)m_DebugRandomizer.NextDouble(),
                    (float)m_DebugRandomizer.NextDouble()
                );
                gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target, c, 1.0f, 0);
            }
            else
                gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target, Color.Transparent, 1.0f, 0);
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);
            
            // Potentially let the game continue for a bit.
            yield return null;
            
            // Create our sprite batch and calculate the cell render orders.
            var spriteBatch = new SpriteBatch(gameContext.Graphics.GraphicsDevice);
            var cellRenderOrder = this.m_CellRenderOrderCalculator.CalculateCellRenderOrder(
                this.m_ChunkSizePolicy.ChunkCellWidth,
                this.m_ChunkSizePolicy.ChunkCellHeight);
            
            // Potentially let the game continue for a bit.
            yield return null;
            
            // Calculate some relative X and Y positions for the rendering.
            int rcx = this.m_ChunkSizePolicy.CellTextureTopPixelWidth * this.m_ChunkSizePolicy.ChunkCellWidth / 2 - this.m_ChunkSizePolicy.CellTextureTopPixelWidth / 2;
            int rcy = this.m_ChunkSizePolicy.CellCubePixelHeight * this.m_ChunkSizePolicy.ChunkCellDepth + this.m_ChunkSizePolicy.CellTextureTopPixelHeight * this.m_ChunkSizePolicy.ChunkCellWidth / 2;
            int rw = this.m_ChunkSizePolicy.CellTextureTopPixelWidth;
            int rh = this.m_ChunkSizePolicy.CellTextureTopPixelHeight / 2;
            
            // Render the chunk target.
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(chunkTarget);
            spriteBatch.Begin(SpriteSortMode.Immediate, null);
            for (var z = 0; z < this.m_ChunkSizePolicy.ChunkCellDepth; z++)
            {
                for (var i = 0; i < cellRenderOrder.Length; i++)
                {
                    // Calculate the X / Y of the tile in the grid.
                    int x = cellRenderOrder[i] % this.m_ChunkSizePolicy.ChunkCellWidth;
                    int y = cellRenderOrder[i] / this.m_ChunkSizePolicy.ChunkCellHeight;
                    
                    // Calculate the render position on screen.
                    int rx = rcx + (int)((x - y) / 2.0 * rw);
                    int ry = rcy + (x + y) * rh - (rh / 2 * (this.m_ChunkSizePolicy.ChunkCellWidth + this.m_ChunkSizePolicy.ChunkCellHeight)) - (z * this.m_ChunkSizePolicy.CellCubePixelHeight);

                    /*
                    var b = task.Chunk.m_Blocks[x, y, z];
                    if (b == null)
                    {
                        if (this.m_FilteredFeatures.IsEnabled(Feature.DebugChunkTiles))
                            m_CurrentRenderState.SpriteBatch.Draw(
                                task.Textures["tiles.grass"],
                                new Vector2(rx, ry),
                                Color.White
                            );
                        continue;
                    }
                    var t = b.Tile;

                    if (t.Image == null)
                    {
                        if (FilteredFeatures.IsEnabled(Feature.DebugChunkTiles))
                            m_CurrentRenderState.SpriteBatch.Draw(
                                task.Textures["tiles.dirt"],
                                new Vector2(rx, ry),
                                Color.White
                            );
                        continue;
                    }
                    Color col = new Color(1f, 1f, 1f, 1f).ToPremultiplied();
                    if (task.Chunk.X == 0 && task.Chunk.Y == 0 && x == 0 && y == 0)
                        col = new Color(1f, 0f, 0f, 1f).ToPremultiplied();
                    if (FilteredFeatures.IsEnabled(Feature.DebugChunkTiles))
                        m_CurrentRenderState.SpriteBatch.Draw(
                            task.Textures[t.Image],
                            new Vector2(rx, ry),
                            col
                        );
                    if (FilteredFeatures.IsEnabled(Feature.RenderCellTops))
                    {
                        m_CurrentRenderState.SpriteBatch.Draw(
                            task.Textures[t.Image + ".isometric.top"],
                            new Rectangle(rx, ry, TileIsometricifier.TILE_TOP_WIDTH, TileIsometricifier.TILE_TOP_HEIGHT),
                            null,
                            col,
                            0,
                            new Vector2(0, 0),
                            SpriteEffects.None,
                            0 // TODO: Use this to correct rendering artifacts.
                        );
                    }
                    if (FilteredFeatures.IsEnabled(Feature.RenderCellSides))
                    {
                        m_CurrentRenderState.SpriteBatch.Draw(
                            task.Textures[t.Image + ".isometric.sideL"],
                            new Rectangle(rx, ry + TileIsometricifier.TILE_TOP_HEIGHT / 2,
                                TileIsometricifier.TILE_SIDE_WIDTH, TileIsometricifier.TILE_SIDE_HEIGHT),
                            null,
                            col,
                            0,
                            new Vector2(0, 0),
                            SpriteEffects.None,
                            0 // TODO: Use this to correct rendering artifacts.
                        );
                        m_CurrentRenderState.SpriteBatch.Draw(
                            task.Textures[t.Image + ".isometric.sideR"],
                            new Rectangle(rx + TileIsometricifier.TILE_TOP_WIDTH / 2, ry + TileIsometricifier.TILE_TOP_HEIGHT / 2,
                                TileIsometricifier.TILE_SIDE_WIDTH, TileIsometricifier.TILE_SIDE_HEIGHT),
                            null,
                            col,
                            0,
                            new Vector2(0, 0),
                            SpriteEffects.None,
                            0 // TODO: Use this to correct rendering artifacts.
                        );
                    }
                    */

                }
            
                // Potentially let the game continue for a bit.
                if (z % 10 == 0)
                {
                    spriteBatch.End();
                    gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);
                    yield return null;
                    gameContext.Graphics.GraphicsDevice.SetRenderTarget(chunkTarget);
                    spriteBatch.Begin(SpriteSortMode.Immediate, null);
                }
            }
            spriteBatch.End();
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);
            
            /*
            
            
                m_GraphicsDevice.SetRenderTarget(m_CurrentRenderState.ChunkTarget);
                m_CurrentRenderState.SpriteBatch.Begin(SpriteSortMode.Immediate, null);
                int count = 0;
                int zcount = 0;
                while (m_CurrentRenderState.CurrentZ < m_CurrentRenderState.ZTop && gt.ElapsedGameTime.TotalMilliseconds < Performance.RENDERING_MILLISECONDS)
                {
                    int z = m_CurrentRenderState.CurrentZ;

                    int rcx = TileIsometricifier.TILE_TOP_WIDTH * Chunk.Width / 2 - TileIsometricifier.TILE_TOP_WIDTH / 2;
                    int rcy = TileIsometricifier.TILE_CUBE_HEIGHT * Chunk.Depth + TileIsometricifier.TILE_TOP_HEIGHT * Chunk.Width / 2 - DEBUG_ZOFFSET;
                    int rw = TileIsometricifier.TILE_TOP_WIDTH;
                    int rh = TileIsometricifier.TILE_TOP_HEIGHT / 2;
                    for (int i = 0; i < m_CurrentRenderState.CellRenderOrder.Length; i++)
                    {
                        // Calculate the X / Y of the tile in the grid.
                        int x = m_CurrentRenderState.CellRenderOrder[i] % RenderWidth;
                        int y = m_CurrentRenderState.CellRenderOrder[i] / RenderWidth;

                        // Calculate the render position on screen.
                        int rx = rcx + (int)((x - y) / 2.0 * rw);
                        int ry = rcy + (x + y) * rh - (rh / 2 * (RenderWidth + RenderHeight)) - (z * TileIsometricifier.TILE_CUBE_HEIGHT);

                        Block b = task.Chunk.m_Blocks[x, y, z];
                        if (b == null)
                        {
                            if (FilteredFeatures.IsEnabled(Feature.DebugChunkTiles))
                                m_CurrentRenderState.SpriteBatch.Draw(
                                    task.Textures["tiles.grass"],
                                    new Vector2(rx, ry),
                                    Color.White
                                );
                            continue;
                        }
                        Tile t = b.Tile;

                        if (t.Image == null)
                        {
                            if (FilteredFeatures.IsEnabled(Feature.DebugChunkTiles))
                                m_CurrentRenderState.SpriteBatch.Draw(
                                    task.Textures["tiles.dirt"],
                                    new Vector2(rx, ry),
                                    Color.White
                                );
                            continue;
                        }
                        Color col = new Color(1f, 1f, 1f, 1f).ToPremultiplied();
                        if (task.Chunk.X == 0 && task.Chunk.Y == 0 && x == 0 && y == 0)
                            col = new Color(1f, 0f, 0f, 1f).ToPremultiplied();
                        if (FilteredFeatures.IsEnabled(Feature.DebugChunkTiles))
                            m_CurrentRenderState.SpriteBatch.Draw(
                                task.Textures[t.Image],
                                new Vector2(rx, ry),
                                col
                            );
                        if (FilteredFeatures.IsEnabled(Feature.RenderCellTops))
                        {
                            m_CurrentRenderState.SpriteBatch.Draw(
                                task.Textures[t.Image + ".isometric.top"],
                                new Rectangle(rx, ry, TileIsometricifier.TILE_TOP_WIDTH, TileIsometricifier.TILE_TOP_HEIGHT),
                                null,
                                col,
                                0,
                                new Vector2(0, 0),
                                SpriteEffects.None,
                                0 // TODO: Use this to correct rendering artifacts.
                            );
                        }
                        if (FilteredFeatures.IsEnabled(Feature.RenderCellSides))
                        {
                            m_CurrentRenderState.SpriteBatch.Draw(
                                task.Textures[t.Image + ".isometric.sideL"],
                                new Rectangle(rx, ry + TileIsometricifier.TILE_TOP_HEIGHT / 2,
                                    TileIsometricifier.TILE_SIDE_WIDTH, TileIsometricifier.TILE_SIDE_HEIGHT),
                                null,
                                col,
                                0,
                                new Vector2(0, 0),
                                SpriteEffects.None,
                                0 // TODO: Use this to correct rendering artifacts.
                            );
                            m_CurrentRenderState.SpriteBatch.Draw(
                                task.Textures[t.Image + ".isometric.sideR"],
                                new Rectangle(rx + TileIsometricifier.TILE_TOP_WIDTH / 2, ry + TileIsometricifier.TILE_TOP_HEIGHT / 2,
                                    TileIsometricifier.TILE_SIDE_WIDTH, TileIsometricifier.TILE_SIDE_HEIGHT),
                                null,
                                col,
                                0,
                                new Vector2(0, 0),
                                SpriteEffects.None,
                                0 // TODO: Use this to correct rendering artifacts.
                            );
                        }

                        count++;
                    }

                    m_CurrentRenderState.CurrentZ++;
                    zcount++;
                }

                FilteredConsole.WriteLine(FilterCategory.OptimizationTiming, "Rendered " + zcount + " levels, " + count + " cells to texture target in " + gt.ElapsedGameTime.Milliseconds + "ms.");
                m_CurrentRenderState.SpriteBatch.End();
                m_GraphicsDevice.SetRenderTarget(null);
                
                */
            
            
            yield return null;
            yield return null;
        }
        

        //private Random m_DebugRandomizer = new Random();

#if FALSE

        #region Rendering Subsystem

            }
            else if (m_CurrentRenderState.RenderMode == 1 /* depth map */ &&
                FilteredFeatures.IsEnabled(Feature.IsometricOcclusion))
            {
                m_GraphicsDevice.SetRenderTarget(m_CurrentRenderState.ChunkDepthMap);
                if (!m_CurrentRenderState.ChunkDepthMapCleared)
                {
                    m_CurrentRenderState.SpriteBatch.Begin(SpriteSortMode.Immediate, null);
                    m_GraphicsDevice.Clear(ClearOptions.Target, Color.Transparent, 1.0f, 0);
                    m_CurrentRenderState.SpriteBatch.End();
                    m_CurrentRenderState.ChunkDepthMapCleared = true;
                }
                BlendState bs = new BlendState();
                bs.AlphaBlendFunction = BlendFunction.Max;
                bs.AlphaSourceBlend = Blend.One;
                bs.AlphaDestinationBlend = Blend.One;
                bs.ColorBlendFunction = BlendFunction.Max;
                bs.ColorSourceBlend = Blend.One;
                bs.ColorDestinationBlend = Blend.One;
                m_CurrentRenderState.SpriteBatch.Begin(SpriteSortMode.Immediate, bs, null, null, null, gameContext.Effects["IsometricDepthMap"]);
                gameContext.Effects["IsometricDepthMap"].Parameters["RotationMode"].SetValue(RenderToNE);
                gameContext.Effects["IsometricDepthMap"].CurrentTechnique.Passes[0].Apply();
                int count = 0;
                int zcount = 0;
                while (m_CurrentRenderState.CurrentZ < m_CurrentRenderState.ZTop && gt.ElapsedGameTime.TotalMilliseconds < Performance.RENDERING_MILLISECONDS)
                {
                    int z = m_CurrentRenderState.CurrentZ;

                    int rcx = TileIsometricifier.TILE_TOP_WIDTH * Chunk.Width / 2 - TileIsometricifier.TILE_TOP_WIDTH / 2;
                    int rcy = TileIsometricifier.TILE_CUBE_HEIGHT * Chunk.Depth + TileIsometricifier.TILE_TOP_HEIGHT * Chunk.Width / 2 - DEBUG_ZOFFSET;
                    int rw = TileIsometricifier.TILE_TOP_WIDTH;
                    int rh = TileIsometricifier.TILE_TOP_HEIGHT / 2;
                    for (int i = 0; i < m_CurrentRenderState.CellRenderOrder.Length; i++)
                    {
                        // Calculate the X / Y of the tile in the grid.
                        int x = m_CurrentRenderState.CellRenderOrder[i] % RenderWidth;
                        int y = m_CurrentRenderState.CellRenderOrder[i] / RenderWidth;

                        // Calculate the "depth" of the tile.
                        int depth = x + y + z;

                        // Calculate the render position on screen.
                        int rx = rcx + (int)((x - y) / 2.0 * rw);
                        int ry = rcy + (x + y) * rh - (rh / 2 * (RenderWidth + RenderHeight)) - (z * TileIsometricifier.TILE_CUBE_HEIGHT);

                        Block b = task.Chunk.m_Blocks[x, y, z];
                        if (b == null)
                            continue;
                        Tile t = b.Tile;

                        if (t.Image == null)
                            continue;
                        gameContext.Effects["IsometricDepthMap"].Parameters["CellDepth"].SetValue((float)Math.Min(depth / 255f, 1.0f));
                        gameContext.Effects["IsometricDepthMap"].CurrentTechnique.Passes[0].Apply();
                        m_CurrentRenderState.SpriteBatch.Draw(
                            task.Textures[t.Image + ".isometric.top"],
                            new Rectangle(rx, ry, TileIsometricifier.TILE_TOP_WIDTH, TileIsometricifier.TILE_TOP_HEIGHT),
                            null,
                            Color.White,
                            0,
                            new Vector2(0, 0),
                            SpriteEffects.None,
                            0
                        );
                        m_CurrentRenderState.SpriteBatch.Draw(
                            task.Textures[t.Image + ".isometric.sideL"],
                            new Rectangle(rx, ry + 12, TileIsometricifier.TILE_SIDE_WIDTH, TileIsometricifier.TILE_SIDE_HEIGHT),
                            null,
                            Color.White,
                            0,
                            new Vector2(0, 0),
                            SpriteEffects.None,
                            0
                        );
                        m_CurrentRenderState.SpriteBatch.Draw(
                            task.Textures[t.Image + ".isometric.sideR"],
                            new Rectangle(rx + 16, ry + 12, TileIsometricifier.TILE_SIDE_WIDTH, TileIsometricifier.TILE_SIDE_HEIGHT),
                            null,
                            Color.White,
                            0,
                            new Vector2(0, 0),
                            SpriteEffects.None,
                            0
                        );
                        count++;
                    }

                    m_CurrentRenderState.CurrentZ++;
                    zcount++;
                }

                FilteredConsole.WriteLine(FilterCategory.OptimizationTiming, "Rendered " + zcount + " levels, " + count + " cells to texture target in " + gt.ElapsedGameTime.TotalMilliseconds + "ms.");
                m_CurrentRenderState.SpriteBatch.End();
                m_GraphicsDevice.SetRenderTarget(null);
            }

            if (m_CurrentRenderState.CurrentZ == m_CurrentRenderState.ZTop)
            {
                if (m_CurrentRenderState.RenderMode < 1)
                {
                    m_CurrentRenderState.CurrentZ = m_CurrentRenderState.ZBottom;
                    m_CurrentRenderState.RenderMode++;
                }
                else
                {
                    m_CurrentRenderState.RenderTask.Result = m_CurrentRenderState.ChunkTarget;
                    m_CurrentRenderState.RenderTask.DepthMap = m_CurrentRenderState.ChunkDepthMap;
                    m_CurrentRenderState.RenderTask.HasResult = true;
                    m_CurrentRenderState = null;
                }
            }
        }

        #endregion

        #region Optimization Subsystem

        private const int m_LastRenderedBuffer = 0;
        private List<Chunk> m_LoadedChunks = new List<Chunk>();
        private List<Chunk> m_NeededChunks = new List<Chunk>();

        public int LastRenderedCountOnScreen
        {
            private get;
            set;
        }

        public bool IsTasked(Chunk chunk)
        {
            foreach (RenderTask rt in m_Tasks)
            {
                if (rt.Chunk == chunk)
                    return true;
            }
            return false;
        }

        public void DiscardUnusedChunks()
        {
            /* If the chunk wasn't in the last used list, we no longer care
             * to render it to a texture, so discard from there.
             */

            if (FilteredFeatures.IsEnabled(Feature.OptimizeChunkRendering))
            {
                int discarded = 0;
                foreach (RenderTask rt in m_Tasks.ToArray())
                {
                    if (!m_NeededChunks.Contains(rt.Chunk))
                    {
                        m_Tasks.Remove(rt);
                        discarded++;
                    }
                }

                if (discarded > 0)
                {
                    FilteredConsole.WriteLine(FilterCategory.Optimization, "SKIPPED RENDERING " + discarded + " UNNEEDED CHUNKS!");
                    discarded = 0;
                }
            }

            /* We can't keep every chunk's texture loaded into memory or
             * else we quickly run out of graphics RAM to store everything.
             */

            if (FilteredFeatures.IsEnabled(Feature.DiscardChunkTextures))
            {
                int discarded = 0;
                while (m_LoadedChunks.Count > LastRenderedCountOnScreen + m_LastRenderedBuffer)
                {
                    m_LoadedChunks[0].DiscardTexture();
                    m_LoadedChunks.RemoveAt(0);
                    discarded++;
                }

                if (discarded > 0)
                    FilteredConsole.WriteLine(FilterCategory.Optimization, "DISCARDED " + discarded + " TEXTURES FROM MEMORY!");
            }
        }

        public void MarkUsed(Chunk chunk)
        {
            // Move this chunk to the end of the list.
            if (m_LoadedChunks.Contains(chunk))
                m_LoadedChunks.Remove(chunk);
            m_LoadedChunks.Add(chunk);
        }

        public void ResetNeeded()
        {
            // Empty the needed chunk list.
            m_NeededChunks.Clear();
        }

        public bool HasNeeded(Chunk chunk)
        {
            // Returns whether the specified chunk is needed.
            return m_NeededChunks.Contains(chunk);
        }

        public void MarkNeeded(Chunk chunk)
        {
            // Add this chunk to the needed chunks list.
            m_NeededChunks.Add(chunk);
        }

        #endregion

        #region Tasking Subsystem

        private GraphicsDevice m_GraphicsDevice;
        private List<RenderTask> m_Tasks = new List<RenderTask>();

        public void Initialize(GraphicsDevice device)
        {
            if (device == null)
                throw new ArgumentNullException("device");
            m_GraphicsDevice = device;
        }

        public void ProcessSingle(GameTime gt, IGameContext gameContext)
        {
            RenderTask rt;
            if (m_Tasks.Count == 0 && m_CurrentRenderState == null)
                return;
            if (m_GraphicsDevice == null)
                return;
            if (m_CurrentRenderState != null)
                rt = m_CurrentRenderState.RenderTask;
            else
            {
                rt = m_Tasks[0];
                m_Tasks.RemoveAt(0);
            }

            RenderTilesToTexture(rt, gt, gameContext);
        }

        public RenderTask PushForRendering(Chunk chunk, IGameContext gameContext)
        {
            RenderTask rt = new RenderTask()
            {
                HasResult = false,
                Result = null,
                DepthMap = null,
                Chunk = chunk,
                Textures = gameContext.Textures
            };
            m_Tasks.Add(rt);
            return rt;
        }

        #endregion

        #endif
    }
}

