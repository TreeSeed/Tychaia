// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Protogame;

namespace Tychaia
{
    public class TychaiaProfiler : IProfiler
    {
        private static TychaiaProfiler SingletonProtection;
    
        private int m_CallCount;
        internal Dictionary<string, double> m_MeasureCosts;
        
        public TychaiaProfiler()
        {
            if (SingletonProtection != null)
                throw new InvalidOperationException();
            SingletonProtection = this;
            this.m_MeasureCosts = new Dictionary<string, double>();
        }

        public int FunctionCallsSinceLastReset
        {
            get { return this.m_CallCount; }
        }

        public IDisposable Measure(string name, params string[] parameters)
        {
            if (name.StartsWith("tychaia-"))
                return new TychaiaProfilerHandle(this, name.Substring(8));
            else
                return new NullProfilerHandle();
        }
        
        public class TychaiaProfilerHandle : IDisposable
        {
            private TychaiaProfiler m_Profiler;
            private string m_Name;
            private Stopwatch m_Stopwatch;
        
            public TychaiaProfilerHandle(TychaiaProfiler profiler, string name)
            {
                this.m_Profiler = profiler;
                this.m_Name = name;
                this.m_Stopwatch = new Stopwatch();
                this.m_Stopwatch.Start();
            }
            
            public void Dispose()
            {
                this.m_Stopwatch.Stop();
                if (!this.m_Profiler.m_MeasureCosts.ContainsKey(this.m_Name))
                    this.m_Profiler.m_MeasureCosts[this.m_Name] = 0;
                this.m_Profiler.m_MeasureCosts[this.m_Name] += this.m_Stopwatch.Elapsed.TotalMilliseconds * 1000;
            }
        }
        
        public void StartRenderStats()
        {
            this.m_MeasureCosts = new Dictionary<string, double>();
        }
        
        public Dictionary<string, double> GetRenderStats()
        {
            return this.m_MeasureCosts;
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