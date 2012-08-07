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

            /* We need to determine the pixel offset from where the chunk needs to
             * be drawn to the focus point.
             */
            int cx = this.Chunk.GlobalX;
            int cy = this.Chunk.GlobalY;
            double ix = 0;
            double iy = 0;
            ix += (this.m_CurrentX - cx);
            iy += (this.m_CurrentX - cx) * 0.75f;
            ix -= (this.m_CurrentY - cy);
            iy += (this.m_CurrentY - cy) * 0.75f;

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
                for (int i = 0; i < innerVerticalChunksToRender + 2; i++)
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

        #region Camera Control

        /// <summary>
        /// The X position on the screen of the current chunk.
        /// </summary>
        private int m_ChunkCenterX = 0;

        /// <summary>
        /// The Y position on the screen of the current chunk.
        /// </summary>
        private int m_ChunkCenterY = 0;

        /// <summary>
        /// The X position in 3D space where we are focusing the camera.
        /// </summary>
        private double m_CurrentX = 0;

        /// <summary>
        /// The Y position in 3D space where we are focusing the camera.
        /// </summary>
        private double m_CurrentY = 0;

        /// <summary>
        /// The Z position in 3D space where we are focusing the camera.
        /// </summary>
        private double m_CurrentZ = 0;

        /// <summary>
        /// Translates a point in the 3D isometric world to a 2D point
        /// on the screen.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="z">The Z position.</param>
        /// <returns>The position on the screen.</returns>
        public Vector2 TranslatePoint(float x, float y, float z)
        {
            float cx = this.Chunk.GlobalX;
            float cy = this.Chunk.GlobalY;
            float xx = this.m_ChunkCenterX;
            float yy = this.m_ChunkCenterY;
            xx += (x - cx);
            yy += (x - cx) * 0.75f;
            xx -= (y - cy);
            yy += (y - cy) * 0.75f;
            yy -= z / 2f;
            return new Vector2(xx, yy);
        }

        /// <summary>
        /// Checks the current relative position, applying the relative X, Y and Z positions
        /// to it and adjusting the center chunk as needed.
        /// </summary>
        public void Pan(double x, double y, double z)
        {
            double newX = this.m_CurrentX + x;
            double newY = this.m_CurrentY + y;

            // Skip if there is no active chunk.
            if (this.Chunk == null)
                return;

            // Pan current chunk.
            while (newX < this.Chunk.GlobalX)
                this.Chunk = this.Chunk.Left;
            while (newX > this.Chunk.GlobalX + Chunk.Width * Scale.CUBE_X)
                this.Chunk = this.Chunk.Right;
            while (newY < this.Chunk.GlobalY)
                this.Chunk = this.Chunk.Up;
            while (newY > this.Chunk.GlobalY + Chunk.Height * Scale.CUBE_Y)
                this.Chunk = this.Chunk.Down;

            this.m_CurrentX += x;
            this.m_CurrentY += y;
            this.m_CurrentZ += z;
        }

        /// <summary>
        /// Sets the current focus of the screen such the center of the screen
        /// is focused on the 3D point in the isometric world.
        /// </summary>
        public void Focus(double x, double y, double z)
        {
            this.Pan(x - this.m_CurrentX, y - this.m_CurrentY, z - this.m_CurrentZ);
        }

        #endregion

        #region Rendering

        protected override void HandleRenderOfEntity(GameContext context, IEntity a)
        {
            if (a is ChunkEntity)
            {
                // Ensure we have a chunk manager to source chunks from.
                if (!(context.World is RPGWorld))
                    return;
                ChunkManager cm = (context.World as RPGWorld).ChunkManager;
                if (cm == null)
                    return;
                if (this.Chunk == null)
                    this.Chunk = cm.ZerothChunk;

                // Special handling for entities in the 3D world.
                ChunkEntity ce = a as ChunkEntity;
                Vector2 pos = this.TranslatePoint(ce.X, ce.Y, ce.Z);

                // Draw image.
                this.DrawSpriteAt(
                    context,
                    pos.X - ce.ImageOffsetX,
                    pos.Y - ce.ImageOffsetY,
                    context.Textures[ce.Image].Width,
                    context.Textures[ce.Image].Height,
                    ce.Image,
                    ce.Color,
                    false);
            }
            else
                base.HandleRenderOfEntity(context, a);
        }

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

            // Validate chunk connectivity.
            this.Chunk.Validate();

            // Determine our Z offset.
            int zoffset = -(Settings.ChunkDepth - this.ZLevel) * TileIsometricifier.TILE_CUBE_HEIGHT;

            // Render chunks.
            ChunkRenderer.ResetNeeded();
            IEnumerable<RelativeRenderInformation> renders = this.GetRelativeRenderInformation(context, this.Chunk);
            foreach (RelativeRenderInformation ri in renders)
            {
                if (ri.Target == this.Chunk)
                {
                    this.m_ChunkCenterX = ri.X + TileIsometricifier.CHUNK_TOP_WIDTH / 2;
                    this.m_ChunkCenterY = ri.Y;
                }
                Texture2D tex = ri.Target.Texture;
                ChunkRenderer.MarkNeeded(ri.Target);
                if (tex != null)
                {
                    ChunkRenderer.MarkUsed(ri.Target);
                    context.SpriteBatch.Draw(tex, new Vector2(ri.X, ri.Y + zoffset), Color.White);
                }
                else
                    FilteredConsole.WriteLine(FilterCategory.Rendering, "No texture yet for chunk to render at " + ri.X + ", " + ri.Y + ".");
            }
            ChunkRenderer.LastRenderedCountOnScreen = renders.Count();
        }

        protected override void DrawTilesAbove(GameContext context)
        {
        }

        #endregion
    }
}
