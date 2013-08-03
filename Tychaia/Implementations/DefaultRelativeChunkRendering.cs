//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public class DefaultRelativeChunkRendering : IRelativeChunkRendering
    {
        private IChunkSizePolicy m_ChunkSizePolicy;
    
        public DefaultRelativeChunkRendering(IChunkSizePolicy chunkSizePolicy)
        {
            this.m_ChunkSizePolicy = chunkSizePolicy;
        }
    
        public IEnumerable<RelativeRenderInformation> GetRelativeRenderInformation(IGameContext context, Chunk center, Vector3 focus)
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

            int innerHorizontalChunksToRender = (int)Math.Ceiling(context.Camera.Width / (double)this.m_ChunkSizePolicy.ChunkTopWidth);

            /* The total number of vertical chunks that will need to be rendered
             * can be found using the vertical distance from D - X.
             */

            int innerVerticalChunksToRender = (int)Math.Ceiling(context.Camera.Height / (double)this.m_ChunkSizePolicy.ChunkTopHeight);

            /* We need to determine the pixel offset from where the chunk needs to
             * be drawn to the focus point.
             */
            long cx = center.X;
            long cy = center.Y;
            long cz = center.Z;
            double ix = 0;
            double iy = 0;
            ix += (focus.X - cx);
            iy += (focus.X - cx) * 0.75f;
            ix -= (focus.Y - cy);
            iy += (focus.Y - cy) * 0.75f;
            iy -= (focus.Z - cz) / 2f;

            /* We need to store the position where we're drawing the center chunk so
             * that positions of entities in the isometric world can be resolved
             * to the screen.
             */
            var chunkCenterX = context.Camera.Width / 2 - this.m_ChunkSizePolicy.ChunkTopWidth / 2 - (int)ix;
            var chunkCenterY = context.Camera.Height / 2 - (int)iy;

            /* Iterate through the horizontal dimension.
             */

            for (int j = -1; j < innerHorizontalChunksToRender + 1; j++)
            {
                int x = chunkCenterX;
                int y = chunkCenterY;

                /* Now we must start at N and go back leftwise, half of the inner chunks
                 * to render (rounded upward)
                 */

                Chunk c = center;
                if (j < innerHorizontalChunksToRender / 2)
                {
                    for (int i = innerHorizontalChunksToRender / 2; i > j; i--)
                    {
                        /* We need to go from N -> H -> M which is N.Left.Down */
                        c = c.West.South;
                        x -= this.m_ChunkSizePolicy.ChunkTopWidth;
                    }
                }
                else
                {
                    for (int i = innerHorizontalChunksToRender / 2; i < j; i++)
                    {
                        /* We need to go from N -> I -> O which is N.Up.Right */
                        c = c.North.East;
                        x += this.m_ChunkSizePolicy.ChunkTopWidth;
                    }
                }

                /* Now we must select the top chunk (diagonally top-right) of this chunk;
                 * in the case of c == N, this would be D. */

                for (int i = 0; i < innerVerticalChunksToRender / 2; i++)
                {
                    /* We need to go from N -> I which is N.Up */
                    c = c.North;
                    x += this.m_ChunkSizePolicy.ChunkTopWidth / 2;
                    y -= this.m_ChunkSizePolicy.ChunkTopHeight / 2;
                }

                /* Now we traverse downwards, diagonally left through the chunks; in the
                 * case of c == D, the process would be D - X.
                 */

                int oldX = x;
                int oldY = y;
                for (int i = 0; i < innerVerticalChunksToRender; i++)
                {
                    /* Loop -2 to +2 on the Z axis */
                    for (int k = -2; k <= 0; k++)
                    {
                        Chunk zc = c;
                        for (int a = 0; a > k; a--)
                            zc = zc.Down;
                        for (int a = 0; a < k; a++)
                            zc = zc.Up;

                        /* Now we add the current chunk to the render list */
                        RelativeRenderInformation ri = new RelativeRenderInformation();
                        ri.Target = zc;
                        ri.X = x;
                        ri.Y = y - k * (this.m_ChunkSizePolicy.ChunkTextureTopHeight * Chunk.Width + this.m_ChunkSizePolicy.ChunkCubeHeight * Chunk.Depth);
                        renders.Add(ri);
                    }

                    /* We need to go from D -> I which is D.Down */
                    c = c.South;
                    x -= this.m_ChunkSizePolicy.ChunkTopWidth / 2;
                    y += this.m_ChunkSizePolicy.ChunkTopHeight / 2;
                }
                x = oldX;
                y = oldY;
            }

            /* Now return the list of renders as an array */
            return renders;
        }
    }
}

