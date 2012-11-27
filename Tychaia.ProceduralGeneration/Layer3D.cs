using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    public abstract class Layer3D : Layer
    {
        protected Layer3D(long seed)
            : base(seed)
        {
        }

        protected Layer3D(Layer parent)
            : base(parent)
        {
        }

        protected Layer3D(Layer[] parents)
            : base(parents)
        {
        }

        public override bool Is3D
        {
            get { return true; }
        }

        protected sealed override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            return this.GenerateDataImpl(x, y, 0, width, height, 1);
        }
    }
}
