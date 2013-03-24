//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using Tychaia.ProceduralGeneration.Biomes;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Output)]
    [FlowDesignerName("Value To 3D")]
    public class AlgorithmValueTo3D : Algorithm<int, int>
    {        
        [DataMember]
        [DefaultValue(64)]
        [Description("Estimate maximum value.")]
        public int EstimateMax
        {
            get;
            set;
        }

        public override string[] InputNames
        {
            get { return new string[] { "Value" }; }
        }

        public override bool Is2DOnly
        {
            get { return false; }
        }

        public AlgorithmValueTo3D()
        {
            this.EstimateMax = 64;
        }

        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            if (input[(i + ox) + (j + oy) * width + (0 + oz) * width * height] >= z) 
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = (int)z;
            else
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = -1;
        }
        
        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (value == -1)
                return Color.Transparent;

            int a;

            double divvalue = (double)this.EstimateMax;
                
            if (divvalue > 255)
                divvalue = 255;
            else if (divvalue < 1)
                divvalue = 1;
                
            a = (int)(value * ((double)255 / divvalue));
         
            if (a < 0)
                a = 0;
            if (a > 255)
                a = 255;

            if (a == 0)
                return Color.Transparent;
            return Color.FromArgb(a, a, a);
        }
    }

}

