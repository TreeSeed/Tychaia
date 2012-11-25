using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protogame.Structure
{
    public class PositionOctree<T> where T : class
    {
        public PositionOctreeNode<T> m_RootNode = null;

        public PositionOctree()
        {
            this.m_RootNode = new PositionOctreeNode<T>(0, 0, 0, 0, 64);
        }

        public T Find(long x, long y, long z)
        {
            return this.m_RootNode.Get(x, y, z);
        }

        public void Insert(T value, long x, long y, long z)
        {
            this.m_RootNode.Set(value, x, y, z);
        }
    }
}
