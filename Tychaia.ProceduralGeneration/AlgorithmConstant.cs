//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General2D)]
    [FlowDesignerCategory(FlowCategory.Initials)]
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

        [DataMember]
        [Description("The color to use when representing this value in the flow editor.")]
        public Color Color
        {
            get;
            set;
        }
        
        public override bool Is2DOnly
        {
            get { return false; }
        }

        public override void ProcessCell(IRuntimeContext context, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = this.Constant;
        }
        
        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.Color;
        }
    }
}

