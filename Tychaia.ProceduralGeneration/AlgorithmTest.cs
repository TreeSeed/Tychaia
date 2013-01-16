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
    [FlowDesignerCategory(FlowCategory.General)]
    [FlowDesignerName("Test Input")]
    public class AlgorithmTest : Algorithm<int, int>
    {
        public override int RequiredXBorder { get { return 2; } }
        public override int RequiredYBorder { get { return 2; } }

        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth)
        {
            output[i + j * width + k * width * height] =
                input[(i + 1) + (j + 1) * width + k * width * height];
        }
    }
}

