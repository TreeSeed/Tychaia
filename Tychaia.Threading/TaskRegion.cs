//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

namespace Tychaia.Threading
{
    public class TaskRegion<TProcessor, TResult>
    {
        bool m_IsThreaded;
        IPipeline<object> m_Pipeline;

        public TaskRegion(bool threaded)
        {
            this.m_IsThreaded = threaded;
            if (this.m_IsThreaded)
                this.m_Pipeline = new ThreadedTaskPipeline();
            else
                this.m_Pipeline = new InlineTaskPipeline();
        }
    }
}

