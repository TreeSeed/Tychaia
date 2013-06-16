//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Collections.Generic;

namespace Tychaia.Threading
{
    public class InlineTaskPipeline<T> : IPipeline<T> where T : class
    {
        private Queue<T> m_InternalQueue;

        public InlineTaskPipeline()
        {
            this.m_InternalQueue = new Queue<T>();
        }

        public void InputConnect()
        {
        }

        public void InputDisconnect()
        {
        }

        public void OutputConnect()
        {
        }

        public void OutputDisconnect()
        {
        }

        public void Put(T value)
        {
            this.m_InternalQueue.Enqueue(value);
        }

        public T Take()
        {
            if (this.m_InternalQueue.Count == 0)
                return null;
            return this.m_InternalQueue.Dequeue();
        }

        public T Take(out bool retrieved)
        {
            if (this.m_InternalQueue.Count == 0)
            {
                retrieved = false;
                return null;
            }

            retrieved = true;
            return this.m_InternalQueue.Dequeue();
        }
    }
}

