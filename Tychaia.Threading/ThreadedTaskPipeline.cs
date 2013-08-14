// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Threading;

namespace Tychaia.Threading
{
    /// <summary>
    /// A thread-safe pipeline capable of accepting tasks from one thread
    /// and collecting them on another.
    /// </summary>
    public class ThreadedTaskPipeline<T> : IPipeline<T>
    {
        private volatile TaskPipelineEntry<T> m_Head;
        private int? m_InputThread;
        private int? m_OutputThread;

        /// <summary>
        /// Creates a new TaskPipeline with the current thread being
        /// considered to be the input side of the pipeline.  The
        /// output thread should call Connect().
        /// </summary>
        public ThreadedTaskPipeline(bool autoconnect = true)
        {
            this.m_InputThread = autoconnect ? (int?) Thread.CurrentThread.ManagedThreadId : null;
            this.m_OutputThread = null;
        }

        /// <summary>
        /// Connects the current thread as the input of the pipeline.
        /// </summary>
        public void InputConnect()
        {
            if (this.m_InputThread != null)
                throw new InvalidOperationException("TaskPipeline can only have one input thread connected.");
            this.m_InputThread = Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// Disconnects the current thread as the input of the pipeline.
        /// </summary>
        public void InputDisconnect()
        {
            if (this.m_InputThread != Thread.CurrentThread.ManagedThreadId)
                throw new InvalidOperationException("Only the input thread may disconnect from TaskPipeline.");
            this.m_InputThread = null;
        }

        /// <summary>
        /// Connects the current thread as the output of the pipeline.
        /// </summary>
        public void OutputConnect()
        {
            if (this.m_OutputThread != null)
                throw new InvalidOperationException("TaskPipeline can only have one output thread connected.");
            this.m_OutputThread = Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// Disconnects the current thread as the output of the pipeline.
        /// </summary>
        public void OutputDisconnect()
        {
            if (this.m_OutputThread != Thread.CurrentThread.ManagedThreadId)
                throw new InvalidOperationException("Only the output thread may disconnect from TaskPipeline.");
            this.m_OutputThread = null;
        }

        /// <summary>
        /// Puts an item into the queue to be processed.
        /// </summary>
        /// <param name="value">Value.</param>
        public void Put(T value)
        {
            if (this.m_InputThread == null)
                throw new InvalidOperationException("The input thread has not been connected yet.");
            if (this.m_InputThread != Thread.CurrentThread.ManagedThreadId)
                throw new InvalidOperationException("Only the input thread may place items into TaskPipeline.");

            lock (this)
            {
                var head = this.m_Head;
                while (head != null && head.Next != null)
                    head = head.Next;
                if (head == null)
                {
                    this.m_Head = new TaskPipelineEntry<T> { Value = value };
                }
                else
                {
                    head.Next = new TaskPipelineEntry<T> { Value = value };
                }
            }
        }

        /// <summary>
        /// Takes the next item from the pipeline, or blocks until an item
        /// is recieved.
        /// </summary>
        /// <returns>The next item.</returns>
        public T Take()
        {
            if (this.m_OutputThread == null)
                throw new InvalidOperationException("The output thread has not been connected yet.");
            if (this.m_OutputThread != Thread.CurrentThread.ManagedThreadId)
                throw new InvalidOperationException("Only the output thread may retrieve items from TaskPipeline.");

            // Return if no value.
            var spin = new SpinWait();
            while (this.m_Head == null)
                spin.SpinOnce();

            T value;
            lock (this)
            {
                value = this.m_Head.Value;
                this.m_Head = this.m_Head.Next;
            }
            return value;
        }

        /// <summary>
        /// Takes the next item from the pipeline, or blocks until an item
        /// is recieved.
        /// </summary>
        /// <returns>The next item.</returns>
        public T Take(out bool retrieved)
        {
            if (this.m_OutputThread != Thread.CurrentThread.ManagedThreadId)
                throw new InvalidOperationException("Only the output thread may retrieve items from TaskPipeline.");

            // Return if no value.
            if (this.m_Head == null)
            {
                retrieved = false;
                return default(T);
            }

            T value;
            lock (this)
            {
                value = this.m_Head.Value;
                this.m_Head = this.m_Head.Next;
            }
            retrieved = true;
            return value;
        }
    }
}

#pragma warning restore 420
