using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Tychaia.Globals;
using Protogame;
using Microsoft.Xna.Framework;
using System.Collections.Concurrent;
using System.Threading;

namespace Tychaia.Generators
{
    public static class ChunkRenderer
    {
        #region Cell Render Ordering

        private static int[][] CellRenderOrder = new int[4][]
        {
            null,
            null,
            null,
            null
        };
        public const int RenderToNE = 0;
        public const int RenderToNW = 1;
        public const int RenderToSE = 2;
        public const int RenderToSW = 3;
        private const int RenderWidth = Chunk.Width;
        private const int RenderHeight = Chunk.Height;

        private static int[] CalculateCellRenderOrder(int targetDir)
        {
            /*               North
             *        0  1  2  3  4  5  6 
             *        1  2  3  4  5  6  7 
             *        2  3  4  5  6  7  8
             *  East  3  4  5  6  7  8  9  West
             *        4  5  6  7  8  9  10
             *        5  6  7  8  9  10 11
             *        6  7  8  9  10 11 12
             *               South
             *  
             * Start value is always 0.
             * Last value is (MaxX + MaxY).
             * This is the AtkValue.
             * 
             * We attack from the left side of the render first
             * with (X: 0, Y: AtkValue) until Y would be less than
             * half of AtkValue.
             * 
             * We then attack from the right side of the render
             * with (X: AtkValue, Y: 0) until X would be less than
             * half of AtkValue - 1.
             * 
             * If we are attacking from the left, but Y is now
             * greater than MaxY, then we are over half-way and are
             * now starting at the bottom of the grid.
             * 
             * In this case, we start with (X: AtkValue - MaxY, Y: MaxY)
             * and continue until we reach the same conditions that
             * apply normally.  The same method applies to the right hand
             * side where we start with (X: MaxX, Y: AtkValue - MaxX).
             *
             */

            if (targetDir != RenderToNE)
                throw new InvalidOperationException();

            int[] result = new int[RenderWidth * RenderHeight];
            int count = 0;
            int start = 0;
            int maxx = RenderWidth - 1;
            int maxy = RenderHeight - 1;
            int last = maxx + maxy;
            int x, y;

            for (int atk = start; atk <= last; atk++)
            {
                // Attack from the left.
                if (atk < maxy)
                {
                    x = 0;
                    y = atk;
                }
                else
                {
                    x = atk - maxy;
                    y = maxy;
                }
                while (y > atk / 2)
                    result[count++] = y-- * RenderWidth + x++;

                // Attack from the right.
                if (atk < maxx)
                {
                    x = atk;
                    y = 0;
                }
                else
                {
                    x = maxx;
                    y = atk - maxx;
                }
                while (y <= atk / 2)
                    result[count++] = y++ * RenderWidth + x--;
            }

            return result;
        }

        private static int[] GetCellRenderOrder(int cameraDirection)
        {
            if (CellRenderOrder[cameraDirection] == null)
                CellRenderOrder[cameraDirection] = CalculateCellRenderOrder(cameraDirection);
            return CellRenderOrder[cameraDirection];
        }

        #endregion

        #region Rendering Subsystem

        private class RenderState
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

        private static RenderState m_CurrentRenderState = null;
        private static Random m_DebugRandomizer = new Random();

        private static void RenderTilesToTexture(RenderTask task, GameTime gt, GameContext context)
        {
            /* Our world is laid out in memory in terms of X / Y, but
             * we are rendering isometric, which means that the rendering
             * order for tiles must be like so:
             * 
             *               North
             *        1  3  5  9  13 19 25
             *        2  6  10 14 20 26 32
             *        4  8  15 21 27 33 37
             *  East  7  12 18 28 34 38 42  West
             *        11 17 24 31 39 43 45
             *        16 23 30 36 41 46 48
             *        22 29 35 40 44 47 49
             *               South
             *  
             * We also need to account for situations where the user rotates
             * the isometric view.
             */

            /*
             *                      North
             *         0    0.5  1     1.5  2    2.5  3
             *        -0.5  0    0.5   1    1.5  2    2.5
             *        -1   -0.5  0     0.5  1    1.5  2
             *  East  -1.5 -1   -0.5   0    0.5  1    1.5  West
             *        -2   -1.5 -1    -0.5  0    0.5  1
             *        -2.5 -2   -1.5  -1   -0.5  0    0.5
             *        -3   -2.5 -2    -1.5 -1   -0.5  0
             *                      South
             *                      
             *  v = (x - y) / 2.0
             */

            int DEBUG_ZOFFSET = 0;//TileIsometricifier.TILE_CUBE_HEIGHT * Settings.ChunkDepth - 200;

            if (m_CurrentRenderState == null)
            {
                RenderState rs = new RenderState();
                rs.ZTop = Chunk.Depth;
                rs.ZBottom = 0;
                rs.ChunkTarget = RenderTargetFactory.Create(
                    m_GraphicsDevice,
                    TileIsometricifier.TILE_TOP_WIDTH * Chunk.Width,
                    TileIsometricifier.TILE_TOP_HEIGHT * Chunk.Width + TileIsometricifier.TILE_CUBE_HEIGHT * Chunk.Depth + TileIsometricifier.CHUNK_HEIGHT_ALLOWANCE,
                    true,
                    SurfaceFormat.Bgra5551,
                    DepthFormat.None);
                rs.ChunkDepthMap = RenderTargetFactory.Create(
                    m_GraphicsDevice,
                    TileIsometricifier.TILE_TOP_WIDTH * Chunk.Width,
                    TileIsometricifier.TILE_TOP_HEIGHT * Chunk.Width + TileIsometricifier.TILE_CUBE_HEIGHT * Chunk.Depth + TileIsometricifier.CHUNK_HEIGHT_ALLOWANCE,
                    true,
                    SurfaceFormat.Bgra5551,
                    DepthFormat.None);
                FilteredConsole.WriteLine(FilterCategory.GraphicsMemoryUsage, "Allocated textures for chunk " + task.Chunk.X + ", " + task.Chunk.Y + ", " + task.Chunk.Z + ".");
                m_GraphicsDevice.SetRenderTarget(rs.ChunkTarget);
                if (FilteredFeatures.IsEnabled(Feature.DebugChunkBackground))
                {
                    Color c = new Color(
                        (float)m_DebugRandomizer.NextDouble(),
                        (float)m_DebugRandomizer.NextDouble(),
                        (float)m_DebugRandomizer.NextDouble()
                    );
                    m_GraphicsDevice.Clear(ClearOptions.Target, c, 1.0f, 0);
                }
                else
                    m_GraphicsDevice.Clear(ClearOptions.Target, Color.Transparent, 1.0f, 0);
                rs.ChunkDepthMapCleared = false;
                rs.SpriteBatch = new SpriteBatch(m_GraphicsDevice);
                rs.CurrentZ = rs.ZBottom;
                rs.RenderTask = task;
                rs.CellRenderOrder = GetCellRenderOrder(RenderToNE);
                rs.RenderMode = 0;
                m_CurrentRenderState = rs;
            }

            if (m_CurrentRenderState.RenderMode == 0 /* chunk texture */)
            {
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
                m_CurrentRenderState.SpriteBatch.Begin(SpriteSortMode.Immediate, bs, null, null, null, context.Effects["IsometricDepthMap"]);
                context.Effects["IsometricDepthMap"].Parameters["RotationMode"].SetValue(RenderToNE);
                context.Effects["IsometricDepthMap"].CurrentTechnique.Passes[0].Apply();
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
                        context.Effects["IsometricDepthMap"].Parameters["CellDepth"].SetValue((float)Math.Min(depth / 255f, 1.0f));
                        context.Effects["IsometricDepthMap"].CurrentTechnique.Passes[0].Apply();
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
        private static List<Chunk> m_LoadedChunks = new List<Chunk>();
        private static List<Chunk> m_NeededChunks = new List<Chunk>();

        public static int LastRenderedCountOnScreen
        {
            private get;
            set;
        }

        public static bool IsTasked(Chunk chunk)
        {
            foreach (RenderTask rt in m_Tasks)
            {
                if (rt.Chunk == chunk)
                    return true;
            }
            return false;
        }

        public static void DiscardUnusedChunks()
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

        public static void MarkUsed(Chunk chunk)
        {
            // Move this chunk to the end of the list.
            if (m_LoadedChunks.Contains(chunk))
                m_LoadedChunks.Remove(chunk);
            m_LoadedChunks.Add(chunk);
        }

        public static void ResetNeeded()
        {
            // Empty the needed chunk list.
            m_NeededChunks.Clear();
        }

        public static bool HasNeeded(Chunk chunk)
        {
            // Returns whether the specified chunk is needed.
            return m_NeededChunks.Contains(chunk);
        }

        public static void MarkNeeded(Chunk chunk)
        {
            // Add this chunk to the needed chunks list.
            m_NeededChunks.Add(chunk);
        }

        #endregion

        #region Tasking Subsystem

        public class RenderTask
        {
            public bool HasResult;
            public RenderTarget2D Result;
            public RenderTarget2D DepthMap;
            public Chunk Chunk;
            public Dictionary<string, Texture2D> Textures;
        }

        private static GraphicsDevice m_GraphicsDevice;
        private static List<RenderTask> m_Tasks = new List<RenderTask>();

        public static void Initialize(GraphicsDevice device)
        {
            if (device == null)
                throw new ArgumentNullException("device");
            m_GraphicsDevice = device;
        }

        public static void ProcessSingle(GameTime gt, GameContext context)
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

            RenderTilesToTexture(rt, gt, context);
        }

        public static RenderTask PushForRendering(Chunk chunk, GameContext context)
        {
            RenderTask rt = new RenderTask()
            {
                HasResult = false,
                Result = null,
                DepthMap = null,
                Chunk = chunk,
                Textures = context.Textures
            };
            m_Tasks.Add(rt);
            return rt;
        }

        #endregion
    }
}
