// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Protogame;

namespace Tychaia
{
    public class TychaiaProfiler : IProfiler
    {
        private static TychaiaProfiler SingletonProtection;
    
        private int m_CallCount;
        private DateTime m_LastStart = DateTime.Now;
        private int m_NetworkOps;

        public TychaiaProfiler()
        {
            if (SingletonProtection != null)
                throw new InvalidOperationException();
            SingletonProtection = this;
            this.MeasureCosts = new Dictionary<string, double>();
        }
        
        public double LastFrameLength
        {
            get;
            set;
        }

        public int FunctionCallsSinceLastReset
        {
            get { return this.m_CallCount; }
        }

        internal Dictionary<string, double> MeasureCosts { get; private set; }
        
        public IDisposable Measure(string name, params string[] parameters)
        {
            if (name.StartsWith("tychaia-"))
                return new TychaiaProfilerHandle(this, name.Substring(8));
            else
                return new NullProfilerHandle();
        }

        public void StartRenderStats()
        {
            this.MeasureCosts = new Dictionary<string, double>();
            this.m_LastStart = DateTime.Now;
        }

        public void CheckSlowFrames()
        {
            var span = DateTime.Now - this.m_LastStart;
            this.LastFrameLength = span.TotalMilliseconds;
            if (span.TotalMilliseconds > (1 / 45f) * 1000f)
            {
                // We just had a slow frame.  Output the statistics to the console.
                Console.WriteLine("=============================");
                Console.WriteLine("WARNING: SLOW FRAME DETECTED!");
                Console.WriteLine("TOTAL TIME: " + span.TotalMilliseconds + "ms");
                
                foreach (var kv in this.GetRenderStats().OrderByDescending(x => x.Value))
                {
                    Console.WriteLine(kv.Key + ": " + kv.Value + "us");
                }
                
                Console.WriteLine("=============================");
            }
        }
        
        public Dictionary<string, double> GetRenderStats()
        {
            return this.MeasureCosts;
        }
        
        public int GetNetworkOps()
        {
            return this.m_NetworkOps;
        }

        internal void FunctionCalled()
        {
            this.m_CallCount++;
        }

        internal void ResetCalls()
        {
            this.m_CallCount = 0;
            this.m_NetworkOps = 0;
        }
        
        public class TychaiaProfilerHandle : IDisposable
        {
            private readonly TychaiaProfiler m_Profiler;
            private readonly string m_Name;
            private readonly Stopwatch m_Stopwatch;
        
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
                if (!this.m_Profiler.MeasureCosts.ContainsKey(this.m_Name))
                    this.m_Profiler.MeasureCosts[this.m_Name] = 0;
                this.m_Profiler.MeasureCosts[this.m_Name] += this.m_Stopwatch.Elapsed.TotalMilliseconds * 1000;
            }
        }
    }
}
