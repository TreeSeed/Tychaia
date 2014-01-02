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

        private MxDispatcher m_MxDispatcher;

        private int m_ReceiveNetworkVal;

        private int m_SendNetworkVal;

        public TychaiaProfiler()
        {
            if (SingletonProtection != null)
            {
                throw new InvalidOperationException();
            }

            SingletonProtection = this;
            this.MeasureCosts = new Dictionary<string, double>();
        }

        public int FunctionCallsSinceLastReset
        {
            get
            {
                return this.m_CallCount;
            }
        }

        public double LastFrameLength { get; set; }

        internal Dictionary<string, double> MeasureCosts { get; private set; }

        public void AttachNetworkDispatcher(MxDispatcher dispatcher)
        {
            this.m_MxDispatcher = dispatcher;
            this.m_MxDispatcher.ReliableSendProgress += this.MxDispatcherOnReliableSendProgress;
            this.m_MxDispatcher.ReliableReceivedProgress += this.MxDispatcherOnReliableReceivedProgress;
            this.m_MxDispatcher.MessageReceived += this.MxDispatcherOnMessageReceived;
            this.m_MxDispatcher.MessageAcknowledged += this.MxDispatcherOnMessageAcknowledged;
        }

        private void MxDispatcherOnMessageAcknowledged(object sender, MxMessageEventArgs mxMessageEventArgs)
        {
            if (mxMessageEventArgs.Client.IsReliable)
            {
                this.m_SendNetworkVal = 0;
            }
        }

        private void MxDispatcherOnMessageReceived(object sender, MxMessageEventArgs mxMessageEventArgs)
        {
            if (mxMessageEventArgs.Client.IsReliable)
            {
                this.m_ReceiveNetworkVal = 0;
            }
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

        public void DetachNetworkDispatcher(MxDispatcher dispatcher)
        {
            if (this.m_MxDispatcher != dispatcher)
            {
                return;
            }

            this.m_MxDispatcher.ReliableSendProgress -= this.MxDispatcherOnReliableSendProgress;
            this.m_MxDispatcher.ReliableReceivedProgress -= this.MxDispatcherOnReliableReceivedProgress;
            this.m_MxDispatcher.MessageReceived -= this.MxDispatcherOnMessageReceived;
            this.m_MxDispatcher.MessageAcknowledged -= this.MxDispatcherOnMessageAcknowledged;
            this.m_MxDispatcher = null;
        }

        public int GetReceiveNetworkOps()
        {
            return this.m_ReceiveNetworkVal;
        }

        public Dictionary<string, double> GetRenderStats()
        {
            return this.MeasureCosts;
        }

        public int GetSendNetworkOps()
        {
            return this.m_SendNetworkVal;
        }

        public IDisposable Measure(string name, params string[] parameters)
        {
            if (name.StartsWith("tychaia-"))
            {
                return new TychaiaProfilerHandle(this, name.Substring(8));
            }

            return new NullProfilerHandle();
        }

        public void StartRenderStats()
        {
            this.MeasureCosts = new Dictionary<string, double>();
            this.m_LastStart = DateTime.Now;
        }

        internal void FunctionCalled()
        {
            this.m_CallCount++;
        }

        internal void ResetCalls()
        {
            this.m_CallCount = 0;
        }

        private void MxDispatcherOnReliableReceivedProgress(object sender, MxReliabilityTransmitEventArgs e)
        {
            this.m_ReceiveNetworkVal = e.CurrentFragments;
        }

        private void MxDispatcherOnReliableSendProgress(object sender, MxReliabilityTransmitEventArgs e)
        {
            this.m_SendNetworkVal = e.CurrentFragments;
        }

        public class TychaiaProfilerHandle : IDisposable
        {
            private readonly string m_Name;

            private readonly TychaiaProfiler m_Profiler;

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
                {
                    this.m_Profiler.MeasureCosts[this.m_Name] = 0;
                }

                this.m_Profiler.MeasureCosts[this.m_Name] += this.m_Stopwatch.Elapsed.TotalMilliseconds * 1000;
            }
        }
    }
}