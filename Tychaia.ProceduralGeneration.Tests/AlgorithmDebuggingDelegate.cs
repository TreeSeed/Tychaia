//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerCategory(FlowCategory.Debugging)]
    [FlowDesignerName("Initial Gradient")]
    public class AlgorithmDebuggingDelegate : Algorithm<int>
    {
        public override bool Is2DOnly
        {
            get { return false; }
        }

        public Func<long, long, long, bool> ValueShouldBePlacedAt
        {
            get;
            set;
        }

        public override void ProcessCell(IRuntimeContext context, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            if (this.ValueShouldBePlacedAt == null)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = 0;
            else
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = this.ValueShouldBePlacedAt(x, y, z) ? 1 : 0;
        }
        
        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (value == 0)
                return System.Drawing.Color.FromArgb(0, 0, 0);
            else
                return System.Drawing.Color.FromArgb(255, 0, 0);
        }
    }
}

