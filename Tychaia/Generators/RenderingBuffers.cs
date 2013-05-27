using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Protogame;
using Microsoft.Xna.Framework;

namespace Tychaia.Generators
{
    public static class RenderingBuffers
    {
        public static RenderTarget2D ScreenBuffer = null;
        public static RenderTarget2D DepthBuffer = null;

        public static void Initialize(GameContext context)
        {
            ScreenBuffer = RenderTargetFactory.Create(context.Graphics.GraphicsDevice, context.Window.ClientBounds.Width,
                context.Window.ClientBounds.Height);
            DepthBuffer = RenderTargetFactory.Create(context.Graphics.GraphicsDevice, context.Window.ClientBounds.Width,
                context.Window.ClientBounds.Height);

            // Forcibly clear the targets to make them transparent.  Under at least Linux,
            // the textures aren't initialized to anything, so they contain garbage graphics
            // data.
            context.Graphics.GraphicsDevice.SetRenderTarget(ScreenBuffer);
            context.Graphics.GraphicsDevice.Clear(Color.Transparent);
            context.Graphics.GraphicsDevice.SetRenderTarget(DepthBuffer);
            context.Graphics.GraphicsDevice.Clear(Color.Transparent);
            context.Graphics.GraphicsDevice.SetRenderTarget(null);
        }
    }
}
