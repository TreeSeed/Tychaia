// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Threading;
using Tychaia.Globals;
using Tychaia.ProceduralGeneration.Flow.Handlers;

namespace Tychaia.ProceduralGeneration.Flow
{
    public class FlowProcessingRequestHandler : IFlowProcessingRequestHandler
    {
        private ICurrentWorldSeedProvider m_CurrentWorldSeedProvider;
        private volatile IFlowProcessingPipeline m_ProcessingPipeline;
        private Thread m_ProcessingThread;
        private IRenderingLocationProvider m_RenderingLocationProvider;
        private IAlgorithmFlowImageGeneration m_AlgorithmFlowImageGeneration;
        private IStorageAccess m_StorageAccess;

        public FlowProcessingRequestHandler(
            ICurrentWorldSeedProvider currentWorldSeedProvider,
            IRenderingLocationProvider renderingLocationProvider,
            IAlgorithmFlowImageGeneration algorithmFlowImageGeneration,
            IStorageAccess storageAccess)
        {
            Console.WriteLine("Request handler created.");
            this.m_ProcessingThread = new Thread(this.Run);
            this.m_ProcessingThread.IsBackground = true;
            this.m_CurrentWorldSeedProvider = currentWorldSeedProvider;
            this.m_RenderingLocationProvider = renderingLocationProvider;
            this.m_AlgorithmFlowImageGeneration = algorithmFlowImageGeneration;
            this.m_StorageAccess = storageAccess;
        }

        public void SetPipelineAndStart(IFlowProcessingPipeline pipeline)
        {
            this.m_ProcessingPipeline = pipeline;
            this.m_ProcessingThread.Start();
        }

        public void Run()
        {
            var generateRuntimeBitmapHandler = new GenerateRuntimeBitmapHandler(
                this.m_CurrentWorldSeedProvider,
                this.m_RenderingLocationProvider,
                this.m_AlgorithmFlowImageGeneration);
            var generatePerformanceResultsHandler = new GeneratePerformanceResultsHandler(
                this.m_CurrentWorldSeedProvider,
                this.m_RenderingLocationProvider,
                this.m_StorageAccess,
                this.m_AlgorithmFlowImageGeneration);
            this.m_ProcessingPipeline.OutputPipeline.InputConnect();
            this.m_ProcessingPipeline.InputPipeline.OutputConnect();

            while (true)
            {
                var request = this.m_ProcessingPipeline.InputPipeline.Take();
                switch (request.RequestType)
                {
                    case FlowProcessingRequestType.GenerateRuntimeBitmap:
                        generateRuntimeBitmapHandler.Handle(
                            (StorageLayer)request.Parameters[0],
                            x => this.m_ProcessingPipeline.OutputPipeline.Put(x));
                        break;
                    case FlowProcessingRequestType.GeneratePerformanceResults:
                        generatePerformanceResultsHandler.Handle(
                            (StorageLayer)request.Parameters[0],
                            x => this.m_ProcessingPipeline.OutputPipeline.Put(x));
                        break;
                }
            }
        }
    }
}
