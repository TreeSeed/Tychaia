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
    public class AlgorithmGradientInitial : Algorithm<int>
    {
        public override int RequiredXBorder { get { return this.XBorder; } }
        
        public int XBorder { get; set; }

        public override bool Is2DOnly
        {
            get { return false; }
        }

        public override void ProcessCell(IRuntimeContext context, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth)
        {
            output[i + j * width + k * width * height] = (int)(x + y * 256);
        }
        
        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (value <= 0)
                return System.Drawing.Color.FromArgb(0, 0, 255);
            else
                return System.Drawing.Color.FromArgb(Math.Max(Math.Min(value / 256, 255), 0), Math.Max(Math.Min(value % 256, 255), 0), 0);
        }
    }
}

