//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Tychaia.ProceduralGeneration;
using Tychaia.ProceduralGeneration.Flow;
using Tychaia.Threading;
using Tychaia.Globals;
using Ninject;

namespace TychaiaWorldGenViewerAlgorithm
{
    public class FlowProcessingPipeline : IFlowProcessingPipeline
    {
        public IPipeline<FlowProcessingRequest> InputPipeline { get; private set; }
        public IPipeline<FlowProcessingResponse> OutputPipeline { get; private set; }

        private FlowForm m_Form = null;

        public FlowProcessingPipeline(IFlowProcessingRequestHandler flowProcessingRequestHandler)
        {
            this.InputPipeline = new ThreadedTaskPipeline<FlowProcessingRequest>(false);
            this.OutputPipeline = new ThreadedTaskPipeline<FlowProcessingResponse>(false);

            flowProcessingRequestHandler.SetPipelineAndStart(this);
        }

        public void FormConnect(FlowForm form)
        {
            this.m_Form = form;
            this.InputPipeline.InputConnect();
            this.OutputPipeline.OutputConnect();
        }

        public void FormCheck()
        {
            bool retrieved;
            var response = this.OutputPipeline.Take(out retrieved);
            if (retrieved)
            {
                switch (response.RequestType)
                {
                    case FlowProcessingRequestType.GenerateRuntimeBitmap:
                        if (response.IsStartNotification)
                            this.m_Form.OnGenerateRuntimeBitmapStart(
                                (StorageLayer)response.Results[0],
                                (System.Drawing.Bitmap)response.Results[1]);
                        else
                            this.m_Form.OnGenerateRuntimeBitmapResponse(
                                (StorageLayer)response.Results[0],
                                (System.Drawing.Bitmap)response.Results[1]);
                        break;
                    case FlowProcessingRequestType.GeneratePerformanceResults:
                        if (response.IsStartNotification)
                            this.m_Form.OnGeneratePerformanceResultsStart(
                                (StorageLayer)response.Results[0],
                                (System.Drawing.Bitmap)response.Results[1]);
                        else
                            this.m_Form.OnGeneratePerformanceResultsResponse(
                                (StorageLayer)response.Results[0],
                                (System.Drawing.Bitmap)response.Results[1],
                                (System.Drawing.Bitmap)response.Results[2]);
                        break;
                }
            }
        }
    }
}

