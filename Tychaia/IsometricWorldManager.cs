using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tychaia.Generators;
using Tychaia.Globals;

namespace Tychaia
{
    public class IsometricWorldManager : WorldManager
    {
        public int ZLevel
        {
            get;
            set;
        }

        public Chunk Chunk
        {
            get;
            set;
        }

        #region Relative Rendering System

        private struct RelativeRenderInformation
        {
            public Chunk Target;
            public int X;
            public int Y;
        }

        private const int UNKNOWN_Y_OFFSET = -16;

        private IEnumerable<RelativeRenderInformation> GetRelativeRenderInformation(GameContext context, Chunk center)
        {
            List<RelativeRenderInformation> renders = new List<RelativeRenderInformation>();

            /* We need to render our isometric chunks surrounding the
             * center chunk, but we only want to render what we have to.
             * We will end up with a pattern like:
             * 
             * A   B   C   D   E   F
             *   G   H   I   J   K
             * L   M   N   O   P   Q
             *   R   S   T   U   V
             * W   X   Y   Z   1   2
             * 
             * Thus the minimum number of chunks to be rendered horizontally
             * can be determined by the distance between G - K
             */

            int innerHorizontalChunksToRender = (int)Math.Ceiling(context.Camera.Width / (double)TileIsometricifier.CHUNK_TOP_WIDTH) + 2;

            /* The total number of vertical chunks that will need to be rendered
             * can be found using the vertical distance from D - X.
             */

            int innerVerticalChunksToRender = (int)Math.Ceiling(context.Camera.Height / (double)TileIsometricifier.CHUNK_TOP_HEIGHT) + 3;

            /* Iterate through the horizontal dimension.
             */

            for (int j = -1; j < innerHorizontalChunksToRender; j++)
            {
                int x = context.Camera.Width / 2;
                int y = context.Camera.Height / 2 + UNKNOWN_Y_OFFSET;

                /* Now we must start at N and go back leftwise, half of the inner chunks
                 * to render (rounded upward)
                 */

                Chunk c = center;
                if (j < innerHorizontalChunksToRender / 2)
                {
                    for (int i = innerHorizontalChunksToRender / 2; i > j; i--)
                    {
                        /* We need to go from N -> H -> M which is N.Left.Down */
                        c = c.Left.Down;
                        x -= TileIsometricifier.CHUNK_TOP_WIDTH;
                    }
                }
                else
                {
                    for (int i = innerHorizontalChunksToRender / 2; i < j; i++)
                    {
                        /* We need to go from N -> I -> O which is N.Up.Right */
                        c = c.Up.Right;
                        x += TileIsometricifier.CHUNK_TOP_WIDTH;
                    }
                }

                /* Now we must select the top chunk (diagonally top-right) of this chunk;
                 * in the case of c == N, this would be D. */

                for (int i = 0; i < innerVerticalChunksToRender / 2; i++)
                {
                    /* We need to go from N -> I which is N.Up */
                    c = c.Up;
                    x += TileIsometricifier.CHUNK_TOP_WIDTH / 2;
                    y -= TileIsometricifier.CHUNK_TOP_HEIGHT / 2;
                }

                /* Now we traverse downwards, diagonally left through the chunks; in the
                 * case of c == D, the process would be D - X.
                 */

                int oldX = x;
                int oldY = y;
                for (int i = 0; i < innerVerticalChunksToRender; i++)
                {
                    /* Now we add the current chunk to the render list */
                    RelativeRenderInformation ri = new RelativeRenderInformation();
                    ri.Target = c;
                    ri.X = x;
                    ri.Y = y;
                    renders.Add(ri);

                    /* We need to go from D -> I which is D.Down */
                    c = c.Down;
                    x -= TileIsometricifier.CHUNK_TOP_WIDTH / 2;
                    y += TileIsometricifier.CHUNK_TOP_HEIGHT / 2;
                }
                x = oldX;
                y = oldY;
            }

            /* Now return the list of renders as an array */
            return renders;
        }

        #endregion

        protected override void PreBegin(GameContext context)
        {
            // Process a single texture block if the FPS is higher than 30.
            if (context.GameTime.ElapsedGameTime.Milliseconds < 100)
            {
                ChunkProvider.ProcessSingle();
                ChunkRenderer.ProcessSingle(context.GameTime);
            }
        }

        protected override void DrawTilesBelow(GameContext context)
        {
            // Ensure we have a chunk manager to source chunks from.
            if (!(context.World is RPGWorld))
                return;
            ChunkManager cm = (context.World as RPGWorld).ChunkManager;
            if (cm == null)
                return;
            if (this.Chunk == null)
                this.Chunk = cm.ZerothChunk;

            // Determine our Z offset.
            int zoffset = -(Settings.ChunkDepth - this.ZLevel) * TileIsometricifier.TILE_CUBE_HEIGHT;

            // Render chunks.
            ChunkRenderer.ResetNeeded();
            IEnumerable<RelativeRenderInformation> renders = this.GetRelativeRenderInformation(context, this.Chunk);
            foreach (RelativeRenderInformation ri in renders)
            {
                //if (ri.Target == cm.ZerothChunk)
                //    continue;
                Texture2D tex = ri.Target.Texture;
                ChunkRenderer.MarkNeeded(ri.Target);
                if (tex != null)
                {
                    ChunkRenderer.MarkUsed(ri.Target);
                    context.SpriteBatch.Draw(tex, new Vector2(ri.X, ri.Y + zoffset), Color.White);
                }
                else
                    Console.WriteLine("No texture yet for chunk to render at " + ri.X + ", " + ri.Y + ".");
            }
            ChunkRenderer.LastRenderedCountOnScreen = renders.Count();
            /*
            Chunk c = null;
            Chunk cl = cm.ZerothChunk;
            int gx = -TileIsometricifier.CHUNK_TOP_WIDTH / 2 + TileIsometricifier.CHUNK_TOP_WIDTH / 2 * 3 - 60;
            int gy = -TileIsometricifier.CHUNK_TOP_HEIGHT / 2 * 4 - 90;
            int x = gx;
            int y = gy;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (c == null)
                        c = cl;
                    else
                        c = c.Right;
                    Texture2D tex = c.Texture;
                    if (tex != null)
                        context.SpriteBatch.Draw(tex, new Vector2(x, y), Color.White);
                    x += TileIsometricifier.CHUNK_TOP_WIDTH / 2;
                    y += TileIsometricifier.CHUNK_TOP_HEIGHT / 2;
                }
                x = gx - (i + 1) * TileIsometricifier.CHUNK_TOP_WIDTH / 2;
                y = gy + (i + 1) * TileIsometricifier.CHUNK_TOP_HEIGHT / 2;
                cl = cl.Down;
                c = null;
            }*/
        }

        protected override void DrawTilesAbove(GameContext context)
        {
        }
    }
}
