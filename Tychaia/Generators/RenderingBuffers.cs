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
    public static class RenderingBuffers
    {
        public static RenderTarget2D ScreenBuffer = null;
        public static RenderTarget2D DepthBuffer = null;

        public static void Initialize(IGameContext gameContext)
        {
            ScreenBuffer = RenderTargetFactory.Create(gameContext.Graphics.GraphicsDevice, gameContext.Window.ClientBounds.Width,
                gameContext.Window.ClientBounds.Height);
            DepthBuffer = RenderTargetFactory.Create(gameContext.Graphics.GraphicsDevice, gameContext.Window.ClientBounds.Width,
                gameContext.Window.ClientBounds.Height);

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
