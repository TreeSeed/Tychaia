using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protogame.Structure
{
    public class PositionOctreeNode<T> where T : class, IPositionOctreeNode
    {
        public long CenterX { get; private set; }
        public long CenterY { get; private set; }
        public long CenterZ { get; private set; }
        public T Value { get; set; }
        public PositionOctreeNode<T>[] Nodes { get; set; }
        public int CurrentDepth { get; private set; }
        public int MaximalDepth { get; private set; }

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
            this.CenterX = centerX;
            this.CenterY = centerY;
            this.CenterZ = centerZ;
            this.Value = null;
            this.CurrentDepth = currentDepth;
            this.MaximalDepth = maximalDepth;
            this.Nodes = new PositionOctreeNode<T>[8]
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
            if (this.CurrentDepth == this.MaximalDepth)
                return this.Value;
            if (x < this.CenterX)
            {
                if (y < this.CenterY)
                {
                    if (z < this.CenterZ)
                        return this.Nodes[0] != null ? this.Nodes[0].Get(x, y, z) : null;
                    else
                        return this.Nodes[1] != null ? this.Nodes[1].Get(x, y, z) : null;
                }
                else
                {
                    if (z < this.CenterZ)
                        return this.Nodes[2] != null ? this.Nodes[2].Get(x, y, z) : null;
                    else
                        return this.Nodes[3] != null ? this.Nodes[3].Get(x, y, z) : null;
                }
            }
            else
            {
                if (y < this.CenterY)
                {
                    if (z < this.CenterZ)
                        return this.Nodes[4] != null ? this.Nodes[4].Get(x, y, z) : null;
                    else
                        return this.Nodes[5] != null ? this.Nodes[5].Get(x, y, z) : null;
                }
                else
                {
                    if (z < this.CenterZ)
                        return this.Nodes[6] != null ? this.Nodes[6].Get(x, y, z) : null;
                    else
                        return this.Nodes[7] != null ? this.Nodes[7].Get(x, y, z) : null;
                }
            }
        }

        private PositionOctreeNode<T> CreateSubnode(long relX, long relY, long relZ)
        {
            long x, y, z;
            long adjust = (Int64.MaxValue / (long)Math.Pow(2, CurrentDepth + 1));
            x = this.CenterX + relX < 0 ? -adjust : adjust;
            y = this.CenterY + relY < 0 ? -adjust : adjust;
            z = this.CenterZ + relZ < 0 ? -adjust : adjust;
            return new PositionOctreeNode<T>(x, y, z, this.CurrentDepth + 1, this.MaximalDepth);
        }

        public void Set(T value)
        {
            if (this.CurrentDepth == this.MaximalDepth)
            {
                this.Value = value;
                return;
            }
            if (value.X < this.CenterX)
            {
                if (value.Y < this.CenterY)
                {
                    if (value.Z < this.CenterZ)
                    {
                        if (this.Nodes[0] == null)
                            this.Nodes[0] = this.CreateSubnode(-1, -1, -1);
                        this.Nodes[0].Set(value);
                    }
                    else
                    {
                        if (this.Nodes[1] == null)
                            this.Nodes[1] = this.CreateSubnode(-1, -1, 1);
                        this.Nodes[1].Set(value);
                    }
                }
                else
                {
                    if (value.Z < this.CenterZ)
                    {
                        if (this.Nodes[2] == null)
                            this.Nodes[2] = this.CreateSubnode(-1, 1, -1);
                        this.Nodes[2].Set(value);
                    }
                    else
                    {
                        if (this.Nodes[3] == null)
                            this.Nodes[3] = this.CreateSubnode(-1, 1, 1);
                        this.Nodes[3].Set(value);
                    }
                }
            }
            else
            {
                if (value.Y < this.CenterY)
                {
                    if (value.Z < this.CenterZ)
                    {
                        if (this.Nodes[4] == null)
                            this.Nodes[4] = this.CreateSubnode(1, -1, -1);
                        this.Nodes[4].Set(value);
                    }
                    else
                    {
                        if (this.Nodes[5] == null)
                            this.Nodes[5] = this.CreateSubnode(1, -1, 1);
                        this.Nodes[5].Set(value);
                    }
                }
                else
                {
                    if (value.Z < this.CenterZ)
                    {
                        if (this.Nodes[6] == null)
                            this.Nodes[6] = this.CreateSubnode(1, 1, -1);
                        this.Nodes[6].Set(value);
                    }
                    else
                    {
                        if (this.Nodes[7] == null)
                            this.Nodes[7] = this.CreateSubnode(1, 1, 1);
                        this.Nodes[7].Set(value);
                    }
                }
            }
        }
    }
}
