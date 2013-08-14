// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Threading;
using Protogame;

namespace Tychaia.Threading
{
    public class TaskRegion<TProcessor, TResult>
        where TProcessor : class, IRegionProcessor<TResult>, new()
    {
        private readonly bool m_IsThreaded;
        private readonly IPipeline<TaskRegionEntry<TResult>> m_Pipeline;

        private readonly PositionOctree<TaskRegionEntry<TResult>> m_Region =
            new PositionOctree<TaskRegionEntry<TResult>>();

        private DateTime m_LastProcess;
        private Thread m_Thread;

        public TaskRegion(bool threaded)
        {
            this.m_IsThreaded = threaded;
            if (this.m_IsThreaded)
                this.m_Pipeline = new ThreadedTaskPipeline<TaskRegionEntry<TResult>>();
            else
            {
                if (this.Processor == null)
                    this.Processor = new TProcessor();
                this.m_Pipeline = new InlineTaskPipeline<TaskRegionEntry<TResult>>();
            }
        }

        public TProcessor Processor { get; private set; }

        public TResult this[long x, long y, long z]
        {
            get
            {
                var entry = this.m_Region.Find(x, y, z);
                return entry == null ? default(TResult) : entry.Result;
            }
        }

        ~TaskRegion()
        {
            if (this.m_Thread != null)
                this.m_Thread.Abort();
        }

        public void Process()
        {
            if (!this.m_IsThreaded)
            {
                bool retrieved;
                var entry = this.m_Pipeline.Take(out retrieved);
                if (!retrieved)
                    return;
                this.Processor.Process(entry);
            }
            else
            {
                this.m_LastProcess = DateTime.Now;
                if (this.m_Thread == null || !this.m_Thread.IsAlive)
                {
                    this.m_Thread = new Thread(this._RunThread);
                    this.m_Thread.Start();
                }
            }
        }

        private void _RunThread()
        {
            // Only run while Process() was called on the main thread in the
            // last 5 seconds.
            this.m_Pipeline.OutputConnect();
            this.Processor = new TProcessor();
            while ((DateTime.Now - this.m_LastProcess).TotalSeconds < 5)
            {
                bool retrieved;
                var value = this.m_Pipeline.Take(out retrieved);
                if (retrieved)
                {
                    this.Processor.Process(value);
                }
            }
            this.m_Pipeline.OutputDisconnect();
        }

        public void ComputeRegion(
            long centerX, long centerY, long centerZ,
            long width, long height, long depth)
        {
            for (var x = centerX - width / 2; x < centerX + width / 2; x++)
            {
                for (var y = centerY - height / 2; y < centerY + height / 2; y++)
                {
                    for (var z = centerZ - depth / 2; z < centerZ + depth / 2; z++)
                    {
                        var value = this.m_Region.Find(x, y, z);
                        if (value == null)
                        {
                            var entry = new TaskRegionEntry<TResult>
                            {
                                X = x,
                                Y = y,
                                Z = z,
                                Result = default(TResult)
                            };
                            this.m_Region.Insert(entry, x, y, z);
                            this.m_Pipeline.Put(entry);
                        }
                    }
                }
            }
        }
    }
}
