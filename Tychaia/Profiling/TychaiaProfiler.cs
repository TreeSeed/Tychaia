//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Protogame;

namespace Tychaia
{
    public class TychaiaProfiler : IProfiler
    {
        private int m_CallCount = 0;
    
        public int FunctionCallsSinceLastReset
        {
            get { return this.m_CallCount; }
        }
    
        public IDisposable Measure(string name, params string[] parameters)
        {
            // This profiler doesn't measure non-function calls.
            return new NullProfilerHandle();
        }
        
        internal void FunctionCalled()
        {
            this.m_CallCount++;
        }
        
        internal void ResetCalls()
        {
            this.m_CallCount = 0;
        }
    }
}

