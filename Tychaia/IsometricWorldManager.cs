using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tychaia.Generators;
using Tychaia.Globals;
using Protogame.Efficiency;
using Tychaia.Game;

namespace Tychaia
{
    public class IsometricWorldManager : WorldManager
    {
        private OccludingSpriteBatch m_OccludingSpriteBatch = null;

        public int ZLevel
        {
            get;
            set;
        }

        private Chunk Chunk
        {
            get;
            set;
        }

        public ChunkOctree Octree
        {
            get;
            private set;
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

            int HORIZ_EXTRA = 2;
            int VERT_EXTRA = 2;
            int VERT_EXTRA_EXTRA = 2;

            int innerHorizontalChunksToRender = (int)Math.Ceiling(context.Camera.Width / (double)TileIsometricifier.CHUNK_TOP_WIDTH) + HORIZ_EXTRA;

            /* The total number of vertical chunks that will need to be rendered
             * can be found using the vertical distance from D - X.
             */

            int innerVerticalChunksToRender = (int)Math.Ceiling(context.Camera.Height / (double)TileIsometricifier.CHUNK_TOP_HEIGHT) + VERT_EXTRA;

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
            float cx = this.Chunk.X;
            float cy = this.Chunk.Y;
            float cz = this.Chunk.Z;
            float xx = this.m_ChunkCenterX;
            float yy = this.m_ChunkCenterY;
            xx += (x - cx);
            yy += (x - cx) * 0.75f;
            xx -= (y - cy);
            yy += (y - cy) * 0.75f;
            yy -= (z - cz) / 2f;
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
            double newZ = this.m_CurrentZ + z;

            // Skip if there is no octree.
            if (this.Octree == null)
                return;

            // Pan current chunk.
            //this.Chunk = this.Octree.Get((long)newX, (long)newY, (long)newZ);
            while (newX < this.Chunk.X)
                this.Chunk = this.Chunk.West;
            while (newX > this.Chunk.X + Chunk.Width * Scale.CUBE_X)
                this.Chunk = this.Chunk.East;
            while (newY < this.Chunk.Y)
                this.Chunk = this.Chunk.North;
            while (newY > this.Chunk.Y + Chunk.Height * Scale.CUBE_Y)
                this.Chunk = this.Chunk.South;
            while (newZ < this.Chunk.Z)
                this.Chunk = this.Chunk.Down;
            while (newZ > this.Chunk.Z + Chunk.Depth * Scale.CUBE_Z)
                this.Chunk = this.Chunk.Up;

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
            if (!FilteredFeatures.IsEnabled(Feature.RenderEntities))
                return;

            if (a is ChunkEntity)
            {
                // Ensure we have a chunk manager to source chunks from.
                if (!(context.World is RPGWorld))
                    return;
                this.Octree = (context.World as RPGWorld).ChunkOctree;
                if (this.Octree == null)
                    return;
                if (this.Chunk == null)
                    this.Chunk = this.Octree.Get(0, 0, 0);

                // Special handling for entities in the 3D world.
                ChunkEntity ce = a as ChunkEntity;
                Vector2 pos = this.TranslatePoint(ce.X, ce.Y, ce.Z);

                // Set depth information.
                if (RenderingBuffers.DepthBuffer != null)
                {
                    // Draw image with depth.
                    float depth = ((
                        ((int)((ce.X < 0) ? Chunk.Width : 0) + (ce.X / Scale.CUBE_X) % Chunk.Width) +
                        ((int)((ce.Y < 0) ? Chunk.Height : 0) + (ce.Y / Scale.CUBE_Y) % Chunk.Height) +
                        ((int)((ce.Z < 0) ? Chunk.Depth : 0) + ((ce.Z / Scale.CUBE_Z) - 1) % Chunk.Depth)) / 255f);
                    this.m_OccludingSpriteBatch.DrawOccludable(
                        context.Textures[ce.Image],
                        new Rectangle((int)(pos.X - ce.ImageOffsetX), (int)(pos.Y - ce.ImageOffsetY),
                            context.Textures[ce.Image].Width, context.Textures[ce.Image].Height),
                        ce.Color.ToPremultiplied(),
                        depth
                        );
                }
                else
                {
                    // Draw image normally.
                    context.SpriteBatch.Draw(
                        context.Textures[ce.Image],
                        new Rectangle((int)(pos.X - ce.ImageOffsetX), (int)(pos.Y - ce.ImageOffsetY),
                            context.Textures[ce.Image].Width, context.Textures[ce.Image].Height),
                        ce.Color.ToPremultiplied()
                        );
                }
            }
            else
                // Render using the default settings.
                base.HandleRenderOfEntity(context, a);
        }

        protected override void PreBegin(GameContext context)
        {
            // Process a single texture block if the FPS is higher than 30.
            //if (context.GameTime.ElapsedGameTime.Milliseconds < 100)
            //{
                ChunkProvider.ProcessSingle();
                ChunkRenderer.ProcessSingle(context.GameTime, context);
            //}

            // Ensure we have an occluding sprite batch.
            if (this.m_OccludingSpriteBatch == null)
                this.m_OccludingSpriteBatch = new OccludingSpriteBatch(context.Graphics.GraphicsDevice);
            this.m_OccludingSpriteBatch.Begin(true);
        }

        protected override void DrawTilesBelow(GameContext context)
        {
            // Ensure we have a chunk manager to source chunks from.
            if (!(context.World is RPGWorld))
                return;
            this.Octree = (context.World as RPGWorld).ChunkOctree;
            if (this.Octree == null)
                return;
            if (this.Chunk == null)
                this.Chunk = this.Octree.Get(0, 0, 0);

            // Determine our Z offset.
            int zoffset = -(Chunk.Depth - this.ZLevel) * TileIsometricifier.TILE_CUBE_HEIGHT;

            // Get rendering information.
            ChunkRenderer.ResetNeeded();
            IEnumerable<RelativeRenderInformation> renders = this.GetRelativeRenderInformation(context, this.Chunk);
            ChunkRenderer.LastRenderedCountOnScreen = renders.Count();

            // Render chunks.
            if (FilteredFeatures.IsEnabled(Feature.DepthBuffer))
            {
                context.EndSpriteBatch();
                context.Graphics.GraphicsDevice.SetRenderTarget(RenderingBuffers.ScreenBuffer);
                context.StartSpriteBatch();
            }
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
                    if (FilteredFeatures.IsEnabled(Feature.DepthBuffer))
                        context.SpriteBatch.Draw(tex, new Vector2(ri.X, ri.Y + zoffset), Color.White);
                    else
                    {
                        if (FilteredFeatures.IsEnabled(Feature.RenderingBuffers))
                            context.Graphics.GraphicsDevice.SetRenderTarget(RenderingBuffers.ScreenBuffer);
                        context.SpriteBatch.Draw(tex, new Vector2(ri.X, ri.Y + zoffset), Color.White);
                    }
                    FilteredConsole.WriteLine(FilterCategory.RenderingActive, "Rendering chunk at " + ri.X + ", " + ri.Y + ".");
                }
                else
                    FilteredConsole.WriteLine(FilterCategory.Rendering, "No texture yet for chunk to render at " + ri.X + ", " + ri.Y + ".");
            }

            // Render depth maps.
            if (FilteredFeatures.IsEnabled(Feature.DepthBuffer))
            {
                context.EndSpriteBatch();
                context.Graphics.GraphicsDevice.SetRenderTarget(RenderingBuffers.DepthBuffer);
                context.StartSpriteBatch();
                foreach (RelativeRenderInformation ri in renders)
                {
                    Texture2D depth = ri.Target.DepthMap;
                    if (depth != null)
                    {
                        ChunkRenderer.MarkUsed(ri.Target);
                        if (FilteredFeatures.IsEnabled(Feature.DepthBuffer))
                        {
                            context.SpriteBatch.Draw(depth, new Vector2(ri.X, ri.Y + zoffset), Color.White);
                        }
                    }
                }
            }

            // Finish drawing.
            context.EndSpriteBatch();
            context.Graphics.GraphicsDevice.SetRenderTarget(null);
            context.StartSpriteBatch();
        }

        protected override void DrawTilesAbove(GameContext context)
        {
            // Draw the current rendering buffers.
            if (FilteredFeatures.IsEnabled(Feature.IsometricOcclusion))
            {
                if (this.m_OccludingSpriteBatch.DepthTexture != RenderingBuffers.DepthBuffer &&
                    RenderingBuffers.DepthBuffer != null)
                    this.m_OccludingSpriteBatch.DepthTexture = RenderingBuffers.DepthBuffer;
                if (RenderingBuffers.ScreenBuffer != null &&
                    FilteredFeatures.IsEnabled(Feature.RenderWorld))
                    this.m_OccludingSpriteBatch.DrawOccluding(RenderingBuffers.ScreenBuffer, Vector2.Zero, Color.White);
                this.m_OccludingSpriteBatch.End();
            }
            else
            {
                if (FilteredFeatures.IsEnabled(Feature.RenderWorld))
                {
                    if (FilteredFeatures.IsEnabled(Feature.RenderingBuffers))
                        context.SpriteBatch.Draw(RenderingBuffers.ScreenBuffer, Vector2.Zero, Color.White);
                }
            }
        }

        #endregion
    }
}
