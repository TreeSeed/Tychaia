//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General2D)]
    [FlowDesignerCategory(FlowCategory.Manipulation)]
    [FlowDesignerName("Value Add")]
    public class AlgorithmAdd : Algorithm<int, int, int>
    {
        [DataMember]
        [DefaultValue(20)]
        [Description("Estimate maximum value.")]
        public int EstimateMax
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(false)]
        [Description("This layer is 2d.")]
        public bool Layer2d
        {
            get;
            set;
        }

        public override bool Is2DOnly
        {
            get { return Layer2d; }
        }

        public override string[] InputNames
        {
            get { return new string[] { "Input A", "Input B" }; }
        }

        public AlgorithmAdd()
        {
            this.EstimateMax = 20;
            this.Layer2d = false;
        }

        public override void ProcessCell(IRuntimeContext context, int[] inputA, int[] inputB, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = inputA[(i + ox) + (j + oy) * width + (k + oz) * width * height] + inputB[(i + ox) + (j + oy) * width + (k + oz) * width * height];
        }
        
        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            int a;

            double divvalue = (double)this.EstimateMax;
                
            if (divvalue > 255)
                divvalue = 255;
            else if (divvalue < 1)
                divvalue = 1;

            a = (int)(value * ((double)255 / divvalue));

            if (a > 255)
                a = 255;

            return Color.FromArgb(a, a, a);
        }
    }
}

