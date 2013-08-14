// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Threading;

#pragma warning disable 420

namespace Tychaia.Threading
{
    /// <summary>
    /// A thread-safe pipeline capable of accepting tasks from one thread
    /// and collecting them on another.
    /// </summary>
    /// <remarks>
    /// http://stackoverflow.com/questions/15571620/issue-with-threaded-queue-implementation-in-net
    /// </remarks>
    [Obsolete("This implementation has threading issues!", true)]
    public class LockfreeThreadedTaskPipeline<T> : IPipeline<T>
    {
        private volatile TaskPipelineEntry<T> m_Head;
        private int? m_InputThread;
        private int? m_OutputThread;

        /// <summary>
        /// Creates a new TaskPipeline with the current thread being
        /// considered to be the input side of the pipeline.  The
        /// output thread should call Connect().
        /// </summary>
        public LockfreeThreadedTaskPipeline(bool autoconnect = true)
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
            if (this.m_InputThread != Thread.CurrentThread.ManagedThreadId)
                throw new InvalidOperationException("Only the input thread may place items into TaskPipeline.");

            retry:

            var head = this.m_Head;
            while (head != null && head.Next != null)
                head = head.Next;
            if (head == null)
            {
                if (Interlocked.CompareExchange(ref this.m_Head, new TaskPipelineEntry<T> { Value = value }, null) !=
                    null)
                    goto retry;
            }
            else
            {
                if (Interlocked.CompareExchange(ref head.Next, new TaskPipelineEntry<T> { Value = value }, null) != null)
                    goto retry;
            }
        }

        /// <summary>
        /// Takes the next item from the pipeline, or blocks until an item
        /// is recieved.
        /// </summary>
        /// <returns>The next item.</returns>
        public T Take()
        {
            if (this.m_OutputThread != Thread.CurrentThread.ManagedThreadId)
                throw new InvalidOperationException("Only the output thread may retrieve items from TaskPipeline.");

            // Wait until there is an item to take.
            var spin = new SpinWait();
            while (this.m_Head == null)
                spin.SpinOnce();

            // Return the item and exchange the current head with
            // the next item, all in an atomic operation.
            var head = this.m_Head;
            retry:
            var next = head.Next;
            if (Interlocked.CompareExchange(ref head.Next, TaskPipelineEntry<T>.Sentinel, next) != next)
                goto retry;
            this.m_Head = next;
            return head.Value;
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

            // Wait until there is an item to take.
            var spin = new SpinWait();
            while (this.m_Head == null)
                spin.SpinOnce();

            // Return the item and exchange the current head with
            // the next item, all in an atomic operation.
            var head = this.m_Head;
            retry:
            var next = head.Next;
            if (Interlocked.CompareExchange(ref head.Next, TaskPipelineEntry<T>.Sentinel, next) != next)
                goto retry;
            this.m_Head = next;
            retrieved = true;
            return head.Value;
        }
    }
}

#pragma warning restore 420
