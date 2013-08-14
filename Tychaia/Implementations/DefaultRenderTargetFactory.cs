// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Tychaia
{
    public class DefaultRenderTargetFactory : IRenderTargetFactory
    {
        public int RenderTargetsUsed { get; private set; }

        public long RenderTargetMemory { get; private set; }

        public RenderTarget2D Create(GraphicsDevice graphicsDevice, int width, int height)
        {
            var rt = new RenderTarget2D(graphicsDevice, width, height);
            rt.Disposing += this.rt_Disposing;
            this.rt_Assign(rt);
            return rt;
        }

        public RenderTarget2D Create(GraphicsDevice graphicsDevice, int width, int height, bool mipMap,
            SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat)
        {
            var rt = new RenderTarget2D(graphicsDevice, width, height, mipMap,
                preferredFormat, preferredDepthFormat);
            rt.Disposing += this.rt_Disposing;
            this.rt_Assign(rt);
            return rt;
        }

        public RenderTarget2D Create(GraphicsDevice graphicsDevice, int width, int height, bool mipMap,
            SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, int preferredMultiSampleCount,
            RenderTargetUsage usage)
        {
            var rt = new RenderTarget2D(graphicsDevice, width, height, mipMap,
                preferredFormat, preferredDepthFormat, preferredMultiSampleCount, usage);
            rt.Disposing += this.rt_Disposing;
            this.rt_Assign(rt);
            return rt;
        }

        private void rt_Disposing(object sender, EventArgs e)
        {
            var rt = sender as RenderTarget2D;
            this.RenderTargetsUsed -= 1;
            this.RenderTargetMemory -= this.GetFormatSize(rt.Format) * rt.Width * rt.Height;
        }

        private void rt_Assign(Texture2D rt)
        {
            this.RenderTargetsUsed += 1;
            this.RenderTargetMemory += this.GetFormatSize(rt.Format) * rt.Width * rt.Height;
        }

        private int GetFormatSize(SurfaceFormat surfaceFormat)
        {
            var BITS_IN_BYTE = 8;
            switch (surfaceFormat)
            {
                case SurfaceFormat.Alpha8:
                    return 8 / BITS_IN_BYTE;
                case SurfaceFormat.Bgr565:
                case SurfaceFormat.Bgra5551:
                case SurfaceFormat.Bgra4444:
                case SurfaceFormat.NormalizedByte2:
                case SurfaceFormat.HalfSingle:
                    return 16 / BITS_IN_BYTE;
                case SurfaceFormat.Color:
                case SurfaceFormat.NormalizedByte4:
                case SurfaceFormat.Rgba1010102:
                case SurfaceFormat.Rg32:
                case SurfaceFormat.Single:
                case SurfaceFormat.HalfVector2:
                    return 32 / BITS_IN_BYTE;
                case SurfaceFormat.Rgba64:
                case SurfaceFormat.Vector2:
                case SurfaceFormat.HalfVector4:
                    return 64 / BITS_IN_BYTE;
                case SurfaceFormat.Vector4:
                    return 128 / BITS_IN_BYTE;
                case SurfaceFormat.Dxt1:
                case SurfaceFormat.Dxt3:
                case SurfaceFormat.Dxt5:
                case SurfaceFormat.HdrBlendable:
                    return 32 / BITS_IN_BYTE; // Estimated value.
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
