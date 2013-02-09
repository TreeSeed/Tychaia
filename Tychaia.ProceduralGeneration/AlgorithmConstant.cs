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
    [FlowDesignerName("Constant Value")]
    public class AlgorithmConstant : Algorithm<int>
    {
        [DataMember]
        [DefaultValue(true)]
        [Description("The constant value to return.")]
        public int Constant
        {
            get;
            set;
        }
        
        public override bool Is2DOnly
        {
            get { return false; }
        }

        public override void ProcessCell(IRuntimeContext context, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth)
        {
            output[i + j * width + k * width * height] = this.Constant;
        }
        
        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return System.Drawing.Color.Gray;
        }
    }
}

