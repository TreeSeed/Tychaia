// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.Threading;

namespace Tychaia.ProceduralGeneration.Flow
{
    public interface IFlowProcessingPipeline
    {
        IPipeline<FlowProcessingRequest> InputPipeline { get; }
        IPipeline<FlowProcessingResponse> OutputPipeline { get; }
    }
}
