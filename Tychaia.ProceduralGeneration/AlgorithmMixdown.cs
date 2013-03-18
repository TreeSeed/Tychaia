//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Drawing;
using Protogame.Noise;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Manipulation)]
    [FlowDesignerName("Value Mixdown")]
    public class AlgorithmMixdown : Algorithm<int, int, int>
    {
        [DataMember]
        [DefaultValue(1.0)]
        [Description("Input A Worth.")]
        public double InputAWorth
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(1.0)]
        [Description("Input A Worth.")]
        public double InputBWorth
        {
            get;
            set;
        }
        
        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the resulting layer.")]
        public int MinValue
        {
            get;
            set;
        }
        
        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum integer value in the resulting layer.")]
        public int MaxValue
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

        public AlgorithmMixdown()
        {
            this.InputAWorth = 1.0;
            this.InputBWorth = 1.0;
            this.MinValue = 0;
            this.MaxValue = 100;
            this.Layer2d = false;
        }

        public override void ProcessCell(IRuntimeContext context, int[] inputA, int[] inputB, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = (int)(((inputA[(i + ox) + (j + oy) * width + (k + oz) * width * height] * InputAWorth) + (inputB[(i + ox) + (j + oy) * width + (k + oz) * width * height] * InputBWorth)) / (InputAWorth + InputBWorth));

            if (output[(i + ox) + (j + oy) * width + (k + oz) * width * height] > this.MaxValue)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = this.MaxValue;
            else if (output[(i + ox) + (j + oy) * width + (k + oz) * width * height] < this.MinValue)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = this.MinValue;
        }
        
        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            int a = (int)(255 * (value / (double)(this.MaxValue - this.MinValue)));

            return Color.FromArgb(a, a, a);
        }
    }
}

