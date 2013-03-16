using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Protogame.Efficiency
{
    public class OccludingSpriteBatch
    {
        private GraphicsDevice m_GraphicsDevice;
        private DepthSpriteBatch m_OccludableSpriteBatch;
        private DepthSpriteBatch m_OccludingSpriteBatch;
        private Texture2D m_DepthTextureReference = null;

        public OccludingSpriteBatch(GraphicsDevice device)
        {
            this.m_GraphicsDevice = device;

            // Load sprite batches and effects.
            this.m_OccludableSpriteBatch = new DepthSpriteBatch(device);
            this.m_OccludableSpriteBatch.Effect = new SpriteEffect(Resources.LoadOccludableSpriteEffect(device));
            this.m_OccludingSpriteBatch = new DepthSpriteBatch(device);
            this.m_OccludingSpriteBatch.Effect = new SpriteEffect(Resources.LoadOccludingEffect(device));
        }

        /// <summary>
        /// Sets the depth texture target used by the occluding surfaces.  Graphically
        /// expensive operation so do not set this value frequently.
        /// </summary>
        public Texture2D DepthTexture
        {
            set
            {
                this.m_OccludingSpriteBatch.Effect.Parameters["DepthTexture"].SetValue(value);
                this.m_OccludingSpriteBatch.Effect.CurrentTechnique.Passes[0].Apply();
                this.m_DepthTextureReference = value;
            }
            get
            {
                return this.m_DepthTextureReference;
            }
        }

        public void Begin(bool clear = false)
        {
            // Clear surfaces if desired.
            if (clear)
                this.m_GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.Black, 1f, 0);
            
            // Reset matrices.
            this.m_OccludableSpriteBatch.ResetMatrices(this.m_GraphicsDevice.Viewport.Width, this.m_GraphicsDevice.Viewport.Height);
            this.m_OccludingSpriteBatch.ResetMatrices(this.m_GraphicsDevice.Viewport.Width, this.m_GraphicsDevice.Viewport.Height);

            // Check existing depth stencil state.
            DepthStencilState existing = this.m_GraphicsDevice.DepthStencilState;
            if (!existing.DepthBufferEnable || !existing.DepthBufferWriteEnable ||
                existing.DepthBufferFunction != CompareFunction.LessEqual)
            {
                // Set new depth stencil state.
                var stencil = new DepthStencilState();
                stencil.DepthBufferFunction = CompareFunction.LessEqual;
                stencil.DepthBufferEnable = true;
                stencil.DepthBufferWriteEnable = true;
                this.m_GraphicsDevice.DepthStencilState = stencil;
            }
        }

        public void End()
        {
            // Flushes content.
            this.m_OccludingSpriteBatch.Flush();
            this.m_OccludableSpriteBatch.Flush();
        }

        #region Occludable Rendering

        public void DrawOccludable(Texture2D texture, Vector2 position, Color color, float z = 0)
        {
            this.DrawOccludable(texture, position, null, color, z);
        }

        public void DrawOccludable(Texture2D texture, Rectangle destinationRectangle, Color color, float z = 0)
        {
            this.DrawOccludable(texture, destinationRectangle, null, color, z);
        }

        public void DrawOccludable(Texture2D texture, Vector2 position,
            Rectangle? sourceRectangle, Color color, float z = 0)
        {
            this.DrawOccludable(texture,
                new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height),
                sourceRectangle, color, z);
        }

        public void DrawOccludable(Texture2D texture, Rectangle destinationRectangle,
            Rectangle? sourceRectangle, Color color, float z = 0)
        {
            this.m_OccludableSpriteBatch.Draw(texture, sourceRectangle ?? texture.Bounds,
                destinationRectangle, color, -(1 - z));
        }

        #endregion

        #region Occluding Rendering

        public void DrawOccluding(Texture2D texture, Vector2 position, Color color, float z = 0)
        {
            this.DrawOccluding(texture, position, null, color, z);
        }

        public void DrawOccluding(Texture2D texture, Rectangle destinationRectangle, Color color, float z = 0)
        {
            this.DrawOccluding(texture, destinationRectangle, null, color, z);
        }

        public void DrawOccluding(Texture2D texture, Vector2 position,
            Rectangle? sourceRectangle, Color color, float z = 0)
        {
            // Z is inverted due to graphics transformations?
            this.DrawOccluding(texture,
                new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height),
                sourceRectangle, color, z);
        }

        public void DrawOccluding(Texture2D texture, Rectangle destinationRectangle,
            Rectangle? sourceRectangle, Color color, float z = 0)
        {
            // Z is inverted due to graphics transformations?
            this.m_OccludingSpriteBatch.Draw(texture, sourceRectangle ?? texture.Bounds,
                destinationRectangle, color, -(1 - z));
        }

        #endregion
    }
}
