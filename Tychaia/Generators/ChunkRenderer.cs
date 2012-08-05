﻿using System;
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

        private static int[][] CellRenderOrder = new int[4][] { null, null, null, null };
        private const int RenderToNE = 0;
        private const int RenderToNW = 1;
        private const int RenderToSE = 2;
        private const int RenderToSW = 3;
        private const int RenderWidth = 16;
        private const int RenderHeight = 16;

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
                { x = 0; y = atk; }
                else
                { x = atk - maxy; y = maxy; }
                while (y > atk / 2)
                    result[count++] = y-- * RenderWidth + x++;

                // Attack from the right.
                if (atk < maxx)
                { x = atk; y = 0; }
                else
                { x = maxx; y = atk - maxx; }
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
            public int CurrentZ;
            public int ZTop;
            public int ZBottom;
            public RenderTarget2D ChunkTarget;
            public SpriteBatch SpriteBatch;
            public RenderTask RenderTask;
            public int[] CellRenderOrder;
        }

        private static RenderState m_CurrentRenderState = null;

        private static void RenderTilesToTexture(RenderTask task, GameTime gt)
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
                rs.ZTop = Settings.ChunkDepth;
                rs.ZBottom = 0;
                rs.ChunkTarget = new RenderTarget2D(
                    m_GraphicsDevice,
                    TileIsometricifier.TILE_TOP_WIDTH * Chunk.Width,
                    TileIsometricifier.TILE_TOP_HEIGHT * Chunk.Width + TileIsometricifier.TILE_CUBE_HEIGHT * Settings.ChunkDepth,
                    true,
                    m_GraphicsDevice.DisplayMode.Format,
                    DepthFormat.Depth24);
                m_GraphicsDevice.SetRenderTarget(rs.ChunkTarget);
                m_GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
                rs.SpriteBatch = new SpriteBatch(m_GraphicsDevice);
                rs.CurrentZ = rs.ZBottom;
                rs.RenderTask = task;
                rs.CellRenderOrder = GetCellRenderOrder(RenderToNE);
                m_CurrentRenderState = rs;
            }

            m_CurrentRenderState.SpriteBatch.Begin(SpriteSortMode.Immediate, null);
            int count = 0;
            int zcount = 0;
            while (m_CurrentRenderState.CurrentZ < m_CurrentRenderState.ZTop && gt.ElapsedGameTime.Milliseconds < 100)
            {
                int z = m_CurrentRenderState.CurrentZ;

                int rcx = TileIsometricifier.TILE_TOP_WIDTH * Chunk.Width / 2 - TileIsometricifier.TILE_TOP_WIDTH / 2;
                int rcy = TileIsometricifier.TILE_CUBE_HEIGHT * Settings.ChunkDepth + TileIsometricifier.TILE_TOP_HEIGHT * Chunk.Width / 2 - DEBUG_ZOFFSET;
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
                        continue;
                    Tile t = b.Tile;

                    if (t.Image == null) continue;
                    m_CurrentRenderState.SpriteBatch.Draw(
                        task.Textures[t.Image + ".isometric.top"],
                        new Rectangle(rx, ry, TileIsometricifier.TILE_TOP_WIDTH, TileIsometricifier.TILE_TOP_HEIGHT),
                        null,
                        new Color(1f, 1f, 1f, 1f).ToPremultiplied(),
                        0,
                        new Vector2(0, 0),
                        SpriteEffects.None,
                        0 // TODO: Use this to correct rendering artifacts.
                        );
                    m_CurrentRenderState.SpriteBatch.Draw(
                        task.Textures[t.Image + ".isometric.sideL"],
                        new Rectangle(rx, ry + 12, TileIsometricifier.TILE_SIDE_WIDTH, TileIsometricifier.TILE_SIDE_HEIGHT),
                        null,
                        new Color(1f, 1f, 1f, 1f).ToPremultiplied(),
                        0,
                        new Vector2(0, 0),
                        SpriteEffects.None,
                        0 // TODO: Use this to correct rendering artifacts.
                        );
                    m_CurrentRenderState.SpriteBatch.Draw(
                        task.Textures[t.Image + ".isometric.sideR"],
                        new Rectangle(rx + 16, ry + 12, TileIsometricifier.TILE_SIDE_WIDTH, TileIsometricifier.TILE_SIDE_HEIGHT),
                        null,
                        new Color(1f, 1f, 1f, 1f).ToPremultiplied(),
                        0,
                        new Vector2(0, 0),
                        SpriteEffects.None,
                        0 // TODO: Use this to correct rendering artifacts.
                        );
                    count++;
                }

                m_CurrentRenderState.CurrentZ++;
                zcount++;
            }

            Console.WriteLine("Rendered " + zcount + " levels, " + count + " cells to texture target in " + gt.ElapsedGameTime.Milliseconds + "ms.");
            m_CurrentRenderState.SpriteBatch.End();
            m_GraphicsDevice.SetRenderTarget(null);

            if (m_CurrentRenderState.CurrentZ == m_CurrentRenderState.ZTop)
            {
                m_CurrentRenderState.RenderTask.Result = m_CurrentRenderState.ChunkTarget;
                m_CurrentRenderState.RenderTask.HasResult = true;
                m_CurrentRenderState = null;
            }
        }

        #endregion

        #region Optimization Subsystem

        private const int m_LastRenderedBuffer = 5;
        private static List<Chunk> m_LoadedChunks = new List<Chunk>();
        private static List<Chunk> m_NeededChunks = new List<Chunk>();

        public static int LastRenderedCountOnScreen
        {
            private get;
            set;
        }

        public static void DiscardUnusedChunks()
        {
            /* If the chunk wasn't in the last used list, we no longer care
             * to render it to a texture, so discard from there.
             */

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
                Console.WriteLine("SKIPPED RENDERING " + discarded + " UNNEEDED CHUNKS!");
                discarded = 0;
            }

            /* We can't keep every chunk's texture loaded into memory or
             * else we quickly run out of graphics RAM to store everything.
             */

            while (m_LoadedChunks.Count > LastRenderedCountOnScreen + m_LastRenderedBuffer)
            {
                m_LoadedChunks[0].DiscardTexture();
                m_LoadedChunks.RemoveAt(0);
                discarded++;
            }

            if (discarded > 0)
                Console.WriteLine("DISCARDED " + discarded + " TEXTURES FROM MEMORY!");
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

        public static void ProcessSingle(GameTime gt)
        {
            RenderTask rt;
            if (m_Tasks.Count == 0)
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

            RenderTilesToTexture(rt, gt);
        }

        public static RenderTask PushForRendering(Chunk chunk, GameContext context)
        {
            RenderTask rt = new RenderTask()
            {
                HasResult = false,
                Result = null,
                Chunk = chunk,
                Textures = context.Textures
            };
            m_Tasks.Add(rt);
            return rt;
        }

        #endregion
    }
}
