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
    [FlowDesignerCategory(FlowCategory.Land)]
    [FlowDesignerName("Initial Land")]
    public class AlgorithmInitialLand : Algorithm<int>
    {
        [DataMember]
        [DefaultValue(0.9)]
        [Description("The value between 0.0 and 1.0 above which the cell is treated as land.")]
        public double LandLimit
        {
            get;
            set;
        }
        
        [DataMember]
        [DefaultValue(true)]
        [Description("Whether to guarantee land at the global (0, 0) position.")]
        public bool GuaranteeStartingPoint
        {
            get;
            set;
        }

        public override int RequiredBorder
        {
            get
            {
                return 0;
            }
        }

        public override void ProcessCell(IRuntimeContext context, int[] output, long x, long y, long z, int width, int height, int depth)
        {
            if (this.GuaranteeStartingPoint && x == 0 && y == 0)
                output[x + y * width + z * width * height] = 1;
            else if (context.GetRandomDouble(x, y, z, context.Modifier) > this.LandLimit)
                output[x + y * width + z * width * height] = 1;
            else
                output[x + y * width + z * width * height] = 0;
        }
    }
}

