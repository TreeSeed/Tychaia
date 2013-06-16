//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Drawing;
using Ninject;
using Tychaia.Globals;

namespace Tychaia.ProceduralGeneration.Flow.Handlers
{
    public class GenerateRuntimeBitmapHandler
    {
        public void Handle(StorageLayer layer, Action<FlowProcessingResponse> put)
        {
            var seedProvider = IoC.Kernel.Get<ICurrentWorldSeedProvider>();

            HandlerHelper.SendStartMessage(
                "Generating...",
                FlowProcessingRequestType.GenerateRuntimeBitmap,
                layer,
                put);

            var provider = IoC.Kernel.Get<IRenderingLocationProvider>();
            var runtime = AlgorithmFlowImageGeneration.RegenerateImageForLayer(
                layer,
                seedProvider.Seed,
                provider.X,
                provider.Y,
                provider.Z,
                64, 64, 64);

            put(new FlowProcessingResponse
            {
                RequestType = FlowProcessingRequestType.GenerateRuntimeBitmap,
                IsStartNotification = false,
                Results = new object[] { layer, runtime }
            });
        }
    }
}

