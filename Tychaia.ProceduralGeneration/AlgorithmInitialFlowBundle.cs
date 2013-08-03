//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Drawing;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.CSharp;
using Tychaia.ProceduralGeneration.FlowBundles;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.FlowBundle)]
    [FlowDesignerName("Initialize FlowBundle")]
    public class AlgorithmInitialFlowBundle : Algorithm<FlowBundle>
    {
        [DataMember]
        [DefaultValue(4)]
        [Description("The maximum amount of data stored within the struct.")]
        public int BundleSize
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(true)]
        [Description("Show this layer as 2D in the editor.")]
        public bool Layer2D
        {
            get;
            set;
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public AlgorithmInitialFlowBundle()
        {
            this.Layer2D = true;
            this.BundleSize = 4;
        }

        public override void ProcessCell(IRuntimeContext context, FlowBundle[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = new FlowBundle(BundleSize);
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return Color.FromArgb(value.GetHashCode());
        }
    }
}

