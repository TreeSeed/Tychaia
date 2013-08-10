// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using Tychaia.ProceduralGeneration.Blocks;
using Tychaia.ProceduralGeneration.FlowBundles;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.FlowBundle)]
    [FlowDesignerCategory(FlowCategory.Output)]
    [FlowDesignerName("Output FlowBundle")]
    public class AlgorithmBundleOutput : Algorithm<FlowBundle, FlowBundle>
    {
        [DataMember]
        [DefaultValue(false)]
        [Description("Whether to display this layer as 2D in the editor.")]
        public bool Layer2D { get; set; }

        public override string[] InputNames
        {
            get { return new[] { "Input" }; }
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public override void ProcessCell(IRuntimeContext context, FlowBundle[] input, FlowBundle[] output, long x, long y,
            long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = input[(i + ox) + (j + oy) * width + (k + oz) * width * height];
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}