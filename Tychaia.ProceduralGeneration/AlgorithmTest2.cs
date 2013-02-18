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
    [FlowDesignerName("Passthrough 2")]
    public class AlgorithmTest2 : Algorithm<int, int>
    {
        public override int RequiredXBorder { get { return this.XBorder; } }
        public override int RequiredYBorder { get { return this.YBorder; } }

        public int XBorder { get; set; }
        public int YBorder { get; set; }

        public override string[] InputNames
        {
            get { return new string[] { "Input" }; }
        }

        public override bool Is2DOnly
        {
            get { return false; }
        }

        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int oi, int oj, int ok, int owidth, int oheight, int odepth)
        {
            output[i + j * width + k * width * height] = input[oi + oj * owidth + ok * owidth * oheight];
        }
        
        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}

