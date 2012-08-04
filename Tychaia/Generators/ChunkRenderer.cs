using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Tychaia.Globals;
using Protogame;
using Microsoft.Xna.Framework;

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

        public static RenderTarget2D RenderTilesToTexture(Chunk current, GameContext context)
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

            int DEBUG_ZOFFSET = TileIsometricifier.TILE_CUBE_HEIGHT * Settings.ChunkDepth - 200;

            int ztop = Settings.ChunkDepth;
            int zbottom = 0;
            RenderTarget2D chunkTarget = new RenderTarget2D(
                context.Graphics.GraphicsDevice,
                TileIsometricifier.TILE_TOP_WIDTH * Chunk.Width,
                TileIsometricifier.TILE_TOP_HEIGHT * Chunk.Width + TileIsometricifier.TILE_CUBE_HEIGHT * Settings.ChunkDepth,
                true,
                context.Graphics.GraphicsDevice.DisplayMode.Format,
                DepthFormat.Depth24);
            context.Graphics.GraphicsDevice.SetRenderTarget(chunkTarget);
            context.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
            using (SpriteBatch sprite = new SpriteBatch(context.Graphics.GraphicsDevice))
            {
                sprite.Begin(SpriteSortMode.Deferred, null);

                int[] render = GetCellRenderOrder(RenderToNE);
                for (int z = zbottom; z < ztop; z++)
                {
                    int rcx = TileIsometricifier.TILE_TOP_WIDTH * Chunk.Width / 2 - TileIsometricifier.TILE_TOP_WIDTH / 2;
                    int rcy = TileIsometricifier.TILE_CUBE_HEIGHT * Settings.ChunkDepth + TileIsometricifier.TILE_TOP_HEIGHT * Chunk.Width / 2 - DEBUG_ZOFFSET;
                    int rw = TileIsometricifier.TILE_TOP_WIDTH;
                    int rh = TileIsometricifier.TILE_TOP_HEIGHT / 2;
                    for (int i = 0; i < render.Length; i++)
                    {
                        // Calculate the X / Y of the tile in the grid.
                        int x = render[i] % RenderWidth;
                        int y = render[i] / RenderWidth;

                        // Calculate the render position on screen.
                        int rx = rcx + (int)((x - y) / 2.0 * rw);// (int)(x / ((RenderWidth + 1) / 2.0) * rw);
                        int ry = rcy + (x + y) * rh - (rh / 2 * (RenderWidth + RenderHeight)) - (z * TileIsometricifier.TILE_CUBE_HEIGHT);

                        Block b = current.m_Blocks[x, y, z];
                        if (b == null)
                            continue;
                        Tile t = b.Tile;
                        int sx = 0;
                        int sw = TileIsometricifier.TILE_TOP_WIDTH;

                        /*Tile p = context.World.Tileset[x, y, z - 1];
                        if (p != null && !(p is TransparentTile))
                            continue;
                        Tile t = context.World.Tileset[x, y, z];
                        if (t == null) continue;
                        Tile l = context.World.Tileset[x, y + 1, z - 1];
                        Tile r = context.World.Tileset[x + 1, y, z - 1];
                        bool leftObscured = (z != zbottom && l != null && !(l is TransparentTile));
                        bool rightObscured = (z != zbottom && r != null && !(r is TransparentTile));
                        int sx = 0;
                        int sw = t.Width;
                        if (leftObscured)
                        { sx += 16; sw -= 16; }
                        if (rightObscured)
                        { sw -= 16; }
                        string img = target ? t.BackImage : t.Image;*/
                        if (t.Image == null) continue;
                        //float f = ((context.World.RenderDepthDownRange - c) / (float)context.World.RenderDepthDownRange);
                        sprite.Draw(
                            context.Textures[t.Image + ".isometric.top"],
                            new Rectangle(rx, ry, TileIsometricifier.TILE_TOP_WIDTH, TileIsometricifier.TILE_TOP_HEIGHT),
                            //new Rectangle(rx, ry, TileIsometricifier.TILE_TOP_WIDTH, TileIsometricifier.TILE_TOP_HEIGHT),
                            null, //new Rectangle(0, 0, TileIsometricifier.TILE_TOP_WIDTH, TileIsometricifier.TILE_TOP_HEIGHT),
                            new Color(1f, 1f, 1f, 1f).ToPremultiplied(),
                            0,
                            new Vector2(0, 0),
                            SpriteEffects.None,
                            0 // TODO: Use this to correct rendering artifacts.
                            );
                        sprite.Draw(
                            context.Textures[t.Image + ".isometric.sideL"],
                            new Rectangle(rx, ry + 12, TileIsometricifier.TILE_SIDE_WIDTH, TileIsometricifier.TILE_SIDE_HEIGHT),
                            null, //new Rectangle(sx, 0, sw, TileIsometricifier.TILE_TOP_HEIGHT),
                            new Color(1f, 1f, 1f, 1f).ToPremultiplied(),
                            0,
                            new Vector2(0, 0),
                            SpriteEffects.None,
                            0 // TODO: Use this to correct rendering artifacts.
                            );
                        sprite.Draw(
                             context.Textures[t.Image + ".isometric.sideR"],
                             new Rectangle(rx + 16, ry + 12, TileIsometricifier.TILE_SIDE_WIDTH, TileIsometricifier.TILE_SIDE_HEIGHT),
                             //new Rectangle(rx + sx + 16, ry + 12, sw, TileIsometricifier.TILE_TOP_HEIGHT),
                             null, //new Rectangle(sx, 0, sw, TileIsometricifier.TILE_TOP_HEIGHT),
                             new Color(1f, 1f, 1f, 1f).ToPremultiplied(),
                             0,
                             new Vector2(0, 0),
                             SpriteEffects.None,
                             0 // TODO: Use this to correct rendering artifacts.
                             );
                    }
                }
                sprite.End();
                context.Graphics.GraphicsDevice.SetRenderTarget(null);
                return chunkTarget;
            }
        }
    }
}
