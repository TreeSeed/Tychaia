// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Manipulation)]
    [FlowDesignerName("Value Clamp")]
    public class AlgorithmClamp : Algorithm<int, int>
    {
        public AlgorithmClamp()
        {
            this.Minimum = 0;
            this.Maximum = 100;
            this.ClampMinimum = false;
            this.ClampMaximum = false;
            this.Layer2D = false;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum value.")]
        public int Minimum { get; set; }

        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum value.")]
        public int Maximum { get; set; }

        [DataMember]
        [DefaultValue(false)]
        [Description("Clamp minimum?")]
        public bool ClampMinimum { get; set; }

        [DataMember]
        [DefaultValue(false)]
        [Description("Clamp maximum?")]
        public bool ClampMaximum { get; set; }

        [DataMember]
        [DefaultValue(false)]
        [Description("Is this a 2D layer?")]
        public bool Layer2D { get; set; }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public override string[] InputNames
        {
            get { return new[] { "Input" }; }
        }

        public override void ProcessCell(
            IRuntimeContext context, int[] input, int[] output,
            long x, long y, long z,
            int i, int j, int k,
            int width, int height, int depth,
            int ox, int oy, int oz)
        {
            var value = input[(i + ox) + (j + oy) * width + (k + oz) * width * height];
            if (this.ClampMinimum)
                if (value < this.Minimum)
                    value = this.Minimum;
            if (this.ClampMaximum)
                if (value > this.Maximum)
                    value = this.Maximum;
            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = value;
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
