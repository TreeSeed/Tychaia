using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tychaia
{
    public class Cube
    {
        private BoundingBox m_BoundingBox;

        public Cube(long x, long y, long z, long width, long height, long depth)
        {
            this.m_BoundingBox = new BoundingBox(new Vector3(x, y, z), new Vector3(x + width, y + height, z + depth));
        }

        public long X
        {
            get
            {
                return (long)this.m_BoundingBox.Min.X;
            }
            set
            {
                this.m_BoundingBox.Min.X = value;
            }
        }

        public long Y
        {
            get
            {
                return (long)this.m_BoundingBox.Min.Y;
            }
            set
            {
                this.m_BoundingBox.Min.Y = value;
            }
        }

        public long Z
        {
            get
            {
                return (long)this.m_BoundingBox.Min.Z;
            }
            set
            {
                this.m_BoundingBox.Min.Z = value;
            }
        }

        public long Width
        {
            get
            {
                return (long)this.m_BoundingBox.Max.X - (long)this.m_BoundingBox.Min.X;
            }
            set
            {
                this.m_BoundingBox.Max.X = value + this.m_BoundingBox.Min.X;
            }
        }

        public long Height
        {
            get
            {
                return (long)this.m_BoundingBox.Max.Y - (long)this.m_BoundingBox.Min.Y;
            }
            set
            {
                this.m_BoundingBox.Max.Y = value + this.m_BoundingBox.Min.Y;
            }
        }

        public long Depth
        {
            get
            {
                return (long)this.m_BoundingBox.Max.Z - (long)this.m_BoundingBox.Min.Z;
            }
            set
            {
                this.m_BoundingBox.Max.Z = value + this.m_BoundingBox.Min.Z;
            }
        }
    }
}
