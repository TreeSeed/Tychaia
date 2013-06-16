//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Threading;
using Tychaia.ProceduralGeneration.Flow.Handlers;

namespace Tychaia.ProceduralGeneration.Flow
{
    public class FlowProcessingRequestHandler : IFlowProcessingRequestHandler
    {
        private Thread m_ProcessingThread;
        private volatile IFlowProcessingPipeline m_ProcessingPipeline;

        public FlowProcessingRequestHandler()
        {
            System.Console.WriteLine("Request handler created.");
            this.m_ProcessingThread = new Thread(this.Run);
            this.m_ProcessingThread.IsBackground = true;
        }

        public void SetPipelineAndStart(IFlowProcessingPipeline pipeline)
        {
            this.m_ProcessingPipeline = pipeline;
            this.m_ProcessingThread.Start();
        }

        public void Run()
        {
            var generateRuntimeBitmapHandler = new GenerateRuntimeBitmapHandler();
            var generatePerformanceResultsHandler = new GeneratePerformanceResultsHandler();
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

