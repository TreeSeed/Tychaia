using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Protogame;

namespace Tychaia.Generators
{
    public static class RenderingBuffers
    {
        public static RenderTarget2D ScreenBuffer = null;
        public static RenderTarget2D DepthBuffer = null;

        public static void Initialize(GameContext context)
        {
            ScreenBuffer = new RenderTarget2D(context.Graphics.GraphicsDevice, context.Window.ClientBounds.Width,
                context.Window.ClientBounds.Height);
            DepthBuffer = new RenderTarget2D(context.Graphics.GraphicsDevice, context.Window.ClientBounds.Width,
                context.Window.ClientBounds.Height);
        }
    }
}
