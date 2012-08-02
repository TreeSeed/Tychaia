using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    public abstract class Layer2D : Layer
    {
        protected Layer2D(long seed)
            : base(seed)
        {
        }

        protected Layer2D(Layer parent)
            : base(parent)
        {
        }

        protected Layer2D(Layer[] parents)
            : base(parents)
        {
        }

        public override bool Is3D
        {
            get { return false; }
        }

        public override bool[] GetParents3DRequired()
        {
            // All 2D layers must have 2D parents.
            string[] parents = this.GetParentsRequired();
            bool[] result = new bool[parents.Length];
            for (int i = 0; i < parents.Length; i++)
                result[i] = false;
            return result;
        }
    }
}
