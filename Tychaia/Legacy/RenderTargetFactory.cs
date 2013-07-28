using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Tychaia
{
    public static class RenderTargetFactory
    {
        //private static List<WeakReference> m_RenderTargets;
        public static int RenderTargetsUsed
        {
            get;
            private set;
        }

        public static long RenderTargetMemory
        {
            get;
            private set;
        }

        private static void rt_Disposing(object sender, EventArgs e)
        {
            RenderTarget2D rt = sender as RenderTarget2D;
            RenderTargetsUsed -= 1;
            RenderTargetMemory -= GetFormatSize(rt.Format) * rt.Width * rt.Height;
        }

        private static void rt_Assign(RenderTarget2D rt)
        {
            RenderTargetsUsed += 1;
            RenderTargetMemory += GetFormatSize(rt.Format) * rt.Width * rt.Height;
        }

        private static int GetFormatSize(SurfaceFormat surfaceFormat)
        {
            int BITS_IN_BYTE = 8;
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

        public static RenderTarget2D Create(GraphicsDevice graphicsDevice, int width, int height)
        {
            RenderTarget2D rt = new RenderTarget2D(graphicsDevice, width, height);
            rt.Disposing += new EventHandler<EventArgs>(rt_Disposing);
            rt_Assign(rt);
            return rt;
        }

        public static RenderTarget2D Create(GraphicsDevice graphicsDevice, int width, int height, bool mipMap,
            SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat)
        {
            RenderTarget2D rt = new RenderTarget2D(graphicsDevice, width, height, mipMap,
                preferredFormat, preferredDepthFormat);
            rt.Disposing += new EventHandler<EventArgs>(rt_Disposing);
            rt_Assign(rt);
            return rt;
        }

        public static RenderTarget2D Create(GraphicsDevice graphicsDevice, int width, int height, bool mipMap,
            SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
        {
            RenderTarget2D rt = new RenderTarget2D(graphicsDevice, width, height, mipMap,
                preferredFormat, preferredDepthFormat, preferredMultiSampleCount, usage);
            rt.Disposing += new EventHandler<EventArgs>(rt_Disposing);
            rt_Assign(rt);
            return rt;
        }
    }
}
