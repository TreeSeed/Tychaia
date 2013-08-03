//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;

namespace Tychaia
{
    public class DefaultRenderingBuffers : IRenderingBuffers
    {
        private IRenderTargetFactory m_RenderTargetFactory;

        public RenderTarget2D ScreenBuffer { get; private set; }
        public RenderTarget2D DepthBuffer { get; private set; }
        
        public DefaultRenderingBuffers(
            IRenderTargetFactory renderTargetFactory)
        {
            this.m_RenderTargetFactory = renderTargetFactory;
        }
        
        public void Initialize(IGameContext gameContext)
        {
            if (this.ScreenBuffer == null)
            {
                this.ScreenBuffer = this.m_RenderTargetFactory.Create(
                    gameContext.Graphics.GraphicsDevice,
                    gameContext.Window.ClientBounds.Width,
                    gameContext.Window.ClientBounds.Height);
            }
            if (this.DepthBuffer == null)
            {
                this.DepthBuffer = this.m_RenderTargetFactory.Create(
                    gameContext.Graphics.GraphicsDevice,
                    gameContext.Window.ClientBounds.Width,
                    gameContext.Window.ClientBounds.Height);
            }

            // Forcibly clear the targets to make them transparent.  Under at least Linux,
            // the textures aren't initialized to anything, so they contain garbage graphics
            // data.
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(ScreenBuffer);
            gameContext.Graphics.GraphicsDevice.Clear(Color.Transparent);
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(DepthBuffer);
            gameContext.Graphics.GraphicsDevice.Clear(Color.Transparent);
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);
        }
    }
}

