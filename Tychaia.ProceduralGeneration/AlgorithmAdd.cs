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
    [FlowDesignerName("Value Add")]
    public class AlgorithmAdd : Algorithm<int, int, int>
    {
        public AlgorithmAdd()
        {
            this.EstimateMax = 20;
            this.Layer2d = true;
        }

        [DataMember]
        [DefaultValue(20)]
        [Description("Estimate maximum value.")]
        public int EstimateMax { get; set; }

        [DataMember]
        [DefaultValue(true)]
        [Description("This layer is 2d.")]
        public bool Layer2d { get; set; }

        public override bool Is2DOnly
        {
            get { return this.Layer2d; }
        }

        public override bool[] InputIs2D
        {
            get { return new[] { this.Layer2d, this.Layer2d }; }
        }

        public override string[] InputNames
        {
            get { return new[] { "Input A", "Input B" }; }
        }

        public override void ProcessCell(
            IRuntimeContext context, 
            int[] inputA, 
            int[] inputB, 
            int[] output, 
            long x,
            long y, 
            long z, 
            int i, 
            int j, 
            int k, 
            int width, 
            int height, 
            int depth, 
            int ox, 
            int oy, 
            int oz)
        {
            output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] =
                inputA[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] +
                inputB[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)];
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
