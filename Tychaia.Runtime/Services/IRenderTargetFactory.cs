// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework.Graphics;

namespace Tychaia
{
    public interface IRenderTargetFactory
    {
        int RenderTargetsUsed { get; }
        long RenderTargetMemory { get; }

        RenderTarget2D Create(GraphicsDevice graphicsDevice, int width, int height);

        RenderTarget2D Create(
            GraphicsDevice graphicsDevice,
            int width,
            int height,
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat);

        RenderTarget2D Create(
            GraphicsDevice graphicsDevice,
            int width,
            int height,
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat,
            int preferredMultiSampleCount,
            RenderTargetUsage usage);
    }
}
