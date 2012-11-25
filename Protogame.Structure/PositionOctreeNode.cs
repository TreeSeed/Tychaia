using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protogame.Structure
{
    public class PositionOctreeNode<T> where T : class
    {
        private long m_CenterX;
        private long m_CenterY;
        private long m_CenterZ;
        private T m_Value;
        private PositionOctreeNode<T>[] m_Nodes;
        private int m_CurrentDepth;
        private int m_MaximalDepth;

        // 0 = -1, -1, -1
        // 1 = -1, -1,  1
        // 2 = -1,  1, -1
        // 3 = -1,  1,  1
        // 4 =  1, -1, -1
        // 5 =  1, -1,  1
        // 6 =  1,  1, -1
        // 7 =  1,  1,  1

        public PositionOctreeNode(long centerX, long centerY, long centerZ, int currentDepth, int maximalDepth)
        {
            this.m_CenterX = centerX;
            this.m_CenterY = centerY;
            this.m_CenterZ = centerZ;
            this.m_Value = null;
            this.m_CurrentDepth = currentDepth;
            this.m_MaximalDepth = maximalDepth;
            this.m_Nodes = new PositionOctreeNode<T>[8]
            {
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            };
        }

        public T Get(long x, long y, long z)
        {
            PositionOctreeNode<T> current = this;
            while (current != null && current.m_CurrentDepth != current.m_MaximalDepth)
            {
                if (x < current.m_CenterX)
                {
                    if (y < current.m_CenterY)
                    {
                        if (z < current.m_CenterZ)
                            current = current.m_Nodes[0] != null ? current.m_Nodes[0] : null;
                        else
                            current = current.m_Nodes[1] != null ? current.m_Nodes[1] : null;
                    }
                    else
                    {
                        if (z < current.m_CenterZ)
                            current = current.m_Nodes[2] != null ? current.m_Nodes[2] : null;
                        else
                            current = current.m_Nodes[3] != null ? current.m_Nodes[3] : null;
                    }
                }
                else
                {
                    if (y < current.m_CenterY)
                    {
                        if (z < current.m_CenterZ)
                            current = current.m_Nodes[4] != null ? current.m_Nodes[4] : null;
                        else
                            current = current.m_Nodes[5] != null ? current.m_Nodes[5] : null;
                    }
                    else
                    {
                        if (z < current.m_CenterZ)
                            current = current.m_Nodes[6] != null ? current.m_Nodes[6] : null;
                        else
                            current = current.m_Nodes[7] != null ? current.m_Nodes[7] : null;
                    }
                }
            }
            if (current == null)
                return null;
            return current.m_Value;
        }

        private PositionOctreeNode<T> CreateSubnode(long relX, long relY, long relZ)
        {
            long x, y, z;
            long adjust = (Int64.MaxValue / (long)Math.Pow(2, m_CurrentDepth + 1));
            x = this.m_CenterX + relX < 0 ? -adjust : adjust;
            y = this.m_CenterY + relY < 0 ? -adjust : adjust;
            z = this.m_CenterZ + relZ < 0 ? -adjust : adjust;
            return new PositionOctreeNode<T>(x, y, z, this.m_CurrentDepth + 1, this.m_MaximalDepth);
        }

        public void Set(T value, long x, long y, long z)
        {
            PositionOctreeNode<T> current = this;
            while (current != null && current.m_CurrentDepth != current.m_MaximalDepth)
            {
                if (x < current.m_CenterX)
                {
                    if (y < current.m_CenterY)
                    {
                        if (z < current.m_CenterZ)
                        {
                            if (current.m_Nodes[0] == null)
                                current.m_Nodes[0] = current.CreateSubnode(-1, -1, -1);
                            current = current.m_Nodes[0];
                        }
                        else
                        {
                            if (current.m_Nodes[1] == null)
                                current.m_Nodes[1] = current.CreateSubnode(-1, -1, 1);
                            current = current.m_Nodes[1];
                        }
                    }
                    else
                    {
                        if (z < this.m_CenterZ)
                        {
                            if (current.m_Nodes[2] == null)
                                current.m_Nodes[2] = current.CreateSubnode(-1, 1, -1);
                            current = current.m_Nodes[2];
                        }
                        else
                        {
                            if (current.m_Nodes[3] == null)
                                current.m_Nodes[3] = current.CreateSubnode(-1, 1, 1);
                            current = current.m_Nodes[3];
                        }
                    }
                }
                else
                {
                    if (y < this.m_CenterY)
                    {
                        if (z < this.m_CenterZ)
                        {
                            if (current.m_Nodes[4] == null)
                                current.m_Nodes[4] = current.CreateSubnode(1, -1, -1);
                            current = current.m_Nodes[4];
                        }
                        else
                        {
                            if (current.m_Nodes[5] == null)
                                current.m_Nodes[5] = current.CreateSubnode(1, -1, 1);
                            current = current.m_Nodes[5];
                        }
                    }
                    else
                    {
                        if (z < this.m_CenterZ)
                        {
                            if (current.m_Nodes[6] == null)
                                current.m_Nodes[6] = current.CreateSubnode(1, 1, -1);
                            current = current.m_Nodes[6];
                        }
                        else
                        {
                            if (current.m_Nodes[7] == null)
                                current.m_Nodes[7] = current.CreateSubnode(1, 1, 1);
                            current = current.m_Nodes[7];
                        }
                    }
                }
            }

            current.m_Value = value;
        }
    }
}
