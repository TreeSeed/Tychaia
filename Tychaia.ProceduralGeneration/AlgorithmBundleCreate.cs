// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using Tychaia.ProceduralGeneration.Biomes;
using Tychaia.ProceduralGeneration.FlowBundles;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.FlowBundle)]
    [FlowDesignerCategory(FlowCategory.Initials)]
    [FlowDesignerName("Bundle from Int32")]
    public class AlgorithmBundleCreateInt32 : Algorithm<Int32, FlowBundle>
    {
        public AlgorithmBundleCreateInt32()
        {
            this.Layer2D = true;
            this.Identifier = "Unassigned";
            this.BundleSize = 4;
        }

        [DataMember]
        [DefaultValue(4)]
        [Description("The maximum amount of data stored within the bundle.")]
        public int BundleSize { get; set; }

        [DataMember]
        [DefaultValue("Unassigned")]
        [Description("The identifier for this instance in the bundle.")]
        public string Identifier { get; set; }

        [DataMember]
        [DefaultValue(true)]
        [Description("Show this layer as 2D in the editor.")]
        public bool Layer2D { get; set; }

        public override string[] InputNames
        {
            get { return new[] { "Int32" }; }
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public override void ProcessCell(IRuntimeContext context, Int32[] input, FlowBundle[] output, long x, long y,
            long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            var bundle = new FlowBundle();
            var result = bundle.Set(this.Identifier, input[(i + ox) + (j + oy) * width + (k + oz) * width * height]);
            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = result;
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return Color.FromArgb(value.Hash());
        }
    }

    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.FlowBundle)]
    [FlowDesignerCategory(FlowCategory.Initials)]
    [FlowDesignerName("Bundle from Biome")]
    public class AlgorithmBundleCreateBiome : Algorithm<Biome, FlowBundle>
    {
        public AlgorithmBundleCreateBiome()
        {
            this.Layer2D = true;
            this.Identifier = "Unassigned";
            this.BundleSize = 4;
        }

        [DataMember]
        [DefaultValue(4)]
        [Description("The maximum amount of data stored within the bundle.")]
        public int BundleSize { get; set; }

        [DataMember]
        [DefaultValue("Unassigned")]
        [Description("The identifier for this instance in the bundle.")]
        public string Identifier { get; set; }

        [DataMember]
        [DefaultValue(true)]
        [Description("Show this layer as 2D in the editor.")]
        public bool Layer2D { get; set; }

        public override string[] InputNames
        {
            get { return new[] { "Biome" }; }
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public override void ProcessCell(IRuntimeContext context, Biome[] input, FlowBundle[] output, long x, long y,
            long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            var bundle = new FlowBundle();
            var result = bundle.Set(this.Identifier, input[(i + ox) + (j + oy) * width + (k + oz) * width * height]);
            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = result;
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return Color.FromArgb(value.Hash());
        }
    }
}