// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
namespace Tychaia.ProceduralGeneration.Flow
{
    public class FlowProcessingResponse
    {
        public bool IsStartNotification;
        public FlowProcessingRequestType RequestType;
        public object[] Results;
    }
}