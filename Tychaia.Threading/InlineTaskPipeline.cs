//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Collections.Generic;

namespace Tychaia.Threading
{
    public class InlineTaskPipeline<T> : IPipeline<T>
    {
        private Queue<T> m_InternalQueue;

        public InlineTaskPipeline()
        {
            this.m_InternalQueue = new Queue<T>();
        }

        public void Connect()
        {
        }

        public void Put(T value)
        {
            this.m_InternalQueue.Enqueue(value);
        }

        public T Take()
        {
            return this.m_InternalQueue.Dequeue();
        }
    }
}

