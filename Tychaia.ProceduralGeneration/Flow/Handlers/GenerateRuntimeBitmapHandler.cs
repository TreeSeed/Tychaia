// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Tychaia.Globals;

namespace Tychaia.ProceduralGeneration.Flow.Handlers
{
    public class GenerateRuntimeBitmapHandler
    {
        private ICurrentWorldSeedProvider m_CurrentWorldSeedProvider;
        private IRenderingLocationProvider m_RenderingLocationProvider;
        private IAlgorithmFlowImageGeneration m_AlgorithmFlowImageGeneration;

        public GenerateRuntimeBitmapHandler(
            ICurrentWorldSeedProvider currentWorldSeedProvider,
            IRenderingLocationProvider renderingLocationProvider,
            IAlgorithmFlowImageGeneration algorithmFlowImageGeneration)
        {
            this.m_CurrentWorldSeedProvider = currentWorldSeedProvider;
            this.m_RenderingLocationProvider = renderingLocationProvider;
            this.m_AlgorithmFlowImageGeneration = algorithmFlowImageGeneration;
        }

        public void Handle(StorageLayer layer, Action<FlowProcessingResponse> put)
        {
            HandlerHelper.SendStartMessage(
                "Generating...",
                FlowProcessingRequestType.GenerateRuntimeBitmap,
                layer,
                put);

            var runtime = this.m_AlgorithmFlowImageGeneration.RegenerateImageForLayer(
                layer,
                this.m_CurrentWorldSeedProvider.Seed,
                this.m_RenderingLocationProvider.X,
                this.m_RenderingLocationProvider.Y,
                this.m_RenderingLocationProvider.Z,
                64, 
                64, 
                64);

            put(new FlowProcessingResponse
            {
                RequestType = FlowProcessingRequestType.GenerateRuntimeBitmap,
                IsStartNotification = false,
                Results = new object[] { layer, runtime }
            });
        }
    }
}
