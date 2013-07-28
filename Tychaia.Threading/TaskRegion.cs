//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Protogame;
using System.Threading;
using System;

namespace Tychaia.Threading
{
    public class TaskRegion<TProcessor, TResult>
        where TProcessor : class, IRegionProcessor<TResult>, new()
    {
        bool m_IsThreaded;
        IPipeline<TaskRegionEntry<TResult>> m_Pipeline;
        PositionOctree<TaskRegionEntry<TResult>> m_Region = new PositionOctree<TaskRegionEntry<TResult>>();
        Thread m_Thread;
        DateTime m_LastProcess;
        TProcessor m_Processor;

        public TaskRegion(bool threaded)
        {
            this.m_IsThreaded = threaded;
            if (this.m_IsThreaded)
                this.m_Pipeline = new ThreadedTaskPipeline<TaskRegionEntry<TResult>>();
            else
            {
                if (this.m_Processor == null)
                    this.m_Processor = new TProcessor();
                this.m_Pipeline = new InlineTaskPipeline<TaskRegionEntry<TResult>>();
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
                this.m_Processor.Process(entry);
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

        public TProcessor Processor
        {
            get { return this.m_Processor; }
        }

        private void _RunThread()
        {
            // Only run while Process() was called on the main thread in the
            // last 5 seconds.
            this.m_Pipeline.OutputConnect();
            this.m_Processor = new TProcessor();
            while ((DateTime.Now - this.m_LastProcess).TotalSeconds < 5)
            {
                bool retrieved;
                var value = this.m_Pipeline.Take(out retrieved);
                if (retrieved)
                {
                    this.m_Processor.Process(value);
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

        public TResult this[long x, long y, long z]
        {
            get
            {
                var entry = this.m_Region.Find(x, y, z);
                return entry == null ? default(TResult) : entry.Result;
            }
        }
    }
}

