//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Collections.Generic;
using Protogame;

namespace Tychaia
{
    public class DefaultRelativeChunkRendering : IRelativeChunkRendering
    {
        public IEnumerable<RelativeRenderInformation> GetRelativeRenderInformation(IGameContext context, Chunk center)
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

            int HORIZ_EXTRA = 2;
            int VERT_EXTRA = 2;
            int VERT_EXTRA_EXTRA = 2;

            int innerHorizontalChunksToRender = (int)Math.Ceiling(context.Camera.Width / (double)TileIsometricifier.CHUNK_TOP_WIDTH) + HORIZ_EXTRA;

            /* The total number of vertical chunks that will need to be rendered
             * can be found using the vertical distance from D - X.
             */

            int innerVerticalChunksToRender = (int)Math.Ceiling(context.Camera.Height / (double)TileIsometricifier.CHUNK_TOP_HEIGHT) + VERT_EXTRA;

            #if NOT_MIGRATED

            /* We need to determine the pixel offset from where the chunk needs to
             * be drawn to the focus point.
             */
            long cx = this.Chunk.X;
            long cy = this.Chunk.Y;
            long cz = this.Chunk.Z;
            double ix = 0;
            double iy = 0;
            ix += (this.m_CurrentX - cx);
            iy += (this.m_CurrentX - cx) * 0.75f;
            ix -= (this.m_CurrentY - cy);
            iy += (this.m_CurrentY - cy) * 0.75f;
            iy -= (this.m_CurrentZ - cz) / 2f;

            /* We need to store the position where we're drawing the center chunk so
             * that positions of entities in the isometric world can be resolved
             * to the screen.
             */
            this.m_ChunkCenterX = context.Camera.Width / 2 - TileIsometricifier.CHUNK_TOP_WIDTH / 2 - (int)ix;
            this.m_ChunkCenterY = context.Camera.Height / 2 + UNKNOWN_Y_OFFSET - (int)iy;

            /* Iterate through the horizontal dimension.
             */

            for (int j = -1; j < innerHorizontalChunksToRender + 1; j++)
            {
                int x = this.m_ChunkCenterX;
                int y = this.m_ChunkCenterY;

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
                        x -= TileIsometricifier.CHUNK_TOP_WIDTH;
                    }
                }
                else
                {
                    for (int i = innerHorizontalChunksToRender / 2; i < j; i++)
                    {
                        /* We need to go from N -> I -> O which is N.Up.Right */
                        c = c.North.East;
                        x += TileIsometricifier.CHUNK_TOP_WIDTH;
                    }
                }

                /* Now we must select the top chunk (diagonally top-right) of this chunk;
                 * in the case of c == N, this would be D. */

                for (int i = 0; i < innerVerticalChunksToRender / 2; i++)
                {
                    /* We need to go from N -> I which is N.Up */
                    c = c.North;
                    x += TileIsometricifier.CHUNK_TOP_WIDTH / 2;
                    y -= TileIsometricifier.CHUNK_TOP_HEIGHT / 2;
                }

                /* Now we traverse downwards, diagonally left through the chunks; in the
                 * case of c == D, the process would be D - X.
                 */

                int oldX = x;
                int oldY = y;
                for (int i = 0; i < innerVerticalChunksToRender + VERT_EXTRA_EXTRA; i++)
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
                        ri.Y = y - k * (TileIsometricifier.TILE_TOP_HEIGHT * Chunk.Width + TileIsometricifier.TILE_CUBE_HEIGHT * Chunk.Depth);
                        renders.Add(ri);
                    }

                    /* We need to go from D -> I which is D.Down */
                    c = c.South;
                    x -= TileIsometricifier.CHUNK_TOP_WIDTH / 2;
                    y += TileIsometricifier.CHUNK_TOP_HEIGHT / 2;
                }
                x = oldX;
                y = oldY;
            }
            #endif

            /* Now return the list of renders as an array */
            return renders;
        }
    }
}

