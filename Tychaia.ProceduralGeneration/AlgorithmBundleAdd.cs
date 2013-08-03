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
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.FlowBundle)]
    [FlowDesignerName("Bundle Add Int32")]
    public class AlgorithmBundleAddInt32 : Algorithm<FlowBundles, Int32, FlowBundles>
    {
        [DataMember]
        [DefaultValue("Unassigned")]
        [Description("The identifier for this instance in the bundle.")]
        public string Identifier
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

        public override string[] InputNames
        {
            get
            {
                return new[] { "FlowBundle", "Int32" };
            }
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public AlgorithmBundleAddInt32()
        {
            this.Layer2D = true;
            this.Identifier = "Unassigned";
        }

        public override void ProcessCell(IRuntimeContext context, FlowBundles[] inputA, Int32[] inputB, FlowBundles[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            inputA[(i + ox) + (j + oy) * width + (k + oz) * width * height].AddValue(inputB[(i + ox) + (j + oy) * width + (k + oz) * width * height]);

            output[(i + ox) + (j + oy)*width + (k + oz)*width*height] = inputA[(i + ox) + (j + oy)*width + (k + oz)*width*height];
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }

    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.FlowBundle)]
    [FlowDesignerName("Bundle Add Biome")]
    public class AlgorithmBundleAddBiome : Algorithm<FlowBundles, Biome, FlowBundles>
    {
        [DataMember]
        [DefaultValue("Unassigned")]
        [Description("The identifier for this instance in the bundle.")]
        public string Identifier
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

        public override string[] InputNames
        {
            get
            {
                return new[] { "FlowBundle", "Biome" };
            }
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public AlgorithmBundleAddBiome()
        {
            this.Layer2D = true;
            this.Identifier = "Unassigned";
        }

        public override void ProcessCell(IRuntimeContext context, FlowBundles[] inputA, Biome[] inputB, FlowBundles[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            inputA[(i + ox) + (j + oy) * width + (k + oz) * width * height].AddValue(inputB[(i + ox) + (j + oy) * width + (k + oz) * width * height]);

            output[(i + ox) + (j + oy)*width + (k + oz)*width*height] = inputA[(i + ox) + (j + oy)*width + (k + oz)*width*height];
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }

}