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

        public Cube(int x, int y, int z, int width, int height, int depth)
        {
            this.m_BoundingBox = new BoundingBox(new Vector3(x, y, z), new Vector3(x + width, y + height, z + depth));
        }

        public int X
        {
            get
            {
                return (int)this.m_BoundingBox.Min.X;
            }
            set
            {
                this.m_BoundingBox.Min.X = value;
            }
        }

        public int Y
        {
            get
            {
                return (int)this.m_BoundingBox.Min.Y;
            }
            set
            {
                this.m_BoundingBox.Min.Y = value;
            }
        }

        public int Z
        {
            get
            {
                return (int)this.m_BoundingBox.Min.Z;
            }
            set
            {
                this.m_BoundingBox.Min.Z = value;
            }
        }

        public int Width
        {
            get
            {
                return (int)this.m_BoundingBox.Max.X - (int)this.m_BoundingBox.Min.X;
            }
            set
            {
                this.m_BoundingBox.Max.X = value + this.m_BoundingBox.Min.X;
            }
        }

        public int Height
        {
            get
            {
                return (int)this.m_BoundingBox.Max.Y - (int)this.m_BoundingBox.Min.Y;
            }
            set
            {
                this.m_BoundingBox.Max.Y = value + this.m_BoundingBox.Min.Y;
            }
        }

        public int Depth
        {
            get
            {
                return (int)this.m_BoundingBox.Max.Z - (int)this.m_BoundingBox.Min.Z;
            }
            set
            {
                this.m_BoundingBox.Max.Z = value + this.m_BoundingBox.Min.Z;
            }
        }
    }
}
