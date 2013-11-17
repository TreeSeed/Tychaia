// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Drawing;
using System.Drawing.Text;

namespace Tychaia.ProceduralGeneration.Flow
{
    public class AlgorithmFlowImageGeneration : IAlgorithmFlowImageGeneration
    {
        private const int RenderWidth = 64;
        private const int RenderHeight = 64;
        private const int RenderDepth = 64;

        private readonly IStorageAccess m_StorageAccess;
        private readonly IIsometricBitmapRenderer m_IsometricBitmapRenderer;

        public AlgorithmFlowImageGeneration(
            IStorageAccess storageAccess,
            IIsometricBitmapRenderer isometricBitmapRenderer)
        {
            this.m_StorageAccess = storageAccess;
            this.m_IsometricBitmapRenderer = isometricBitmapRenderer;
        }

        public Bitmap RegenerateImageForLayer(
            StorageLayer layer,
            long seed,
            long ox, long oy, long oz,
            int width, int height, int depth,
            bool compiled = false)
        {
            try
            {
                var runtime = this.m_StorageAccess.ToRuntime(layer);
                runtime.SetSeed(seed);
                if (compiled)
                {
                    try
                    {
                        return this.m_IsometricBitmapRenderer.GenerateImage(
                            runtime,
                            x => runtime.Algorithm.GetColorForValue(this.m_StorageAccess.FromRuntime(runtime), x),
                            ox, oy, oz,
                            width, height, runtime.Algorithm.Is2DOnly ? 1 : depth);
                        /*return Regenerate3DImageForLayer(
                            runtime,
                            ox, oy, oz,
                            width, height, depth,
                            this.m_StorageAccess.ToCompiled(runtime));*/
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                return this.m_IsometricBitmapRenderer.GenerateImage(
                    runtime,
                    x => runtime.Algorithm.GetColorForValue(this.m_StorageAccess.FromRuntime(runtime), x),
                    ox, oy, oz,
                    width, height, runtime.Algorithm.Is2DOnly ? 1 : depth);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}
