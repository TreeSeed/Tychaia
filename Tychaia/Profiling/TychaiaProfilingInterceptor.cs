//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Ninject.Extensions.Interception;

namespace Tychaia
{
    public class TychaiaProfilingInterceptor : IInterceptor
    {
        private TychaiaProfiler m_Profiler;
    
        public TychaiaProfilingInterceptor(TychaiaProfiler profiler)
        {
            this.m_Profiler = profiler;
        }

        public void Intercept(IInvocation invocation)
        {
            this.m_Profiler.FunctionCalled();
            invocation.Proceed();
        }
    }
}

