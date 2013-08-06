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
    [FlowDesignerCategory(FlowCategory.Add)]
    [FlowDesignerName("Bundle Set Int32")]
    public class AlgorithmBundleSetInt32 : Algorithm<FlowBundle, Int32, FlowBundle>
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

        public AlgorithmBundleSetInt32()
        {
            this.Layer2D = true;
            this.Identifier = "Unassigned";
        }

        public override void ProcessCell(IRuntimeContext context, FlowBundle[] inputA, Int32[] inputB, FlowBundle[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] =
				inputA[(i + ox) + (j + oy) * width + (k + oz) * width* height].Set(
					this.Identifier, 
					inputB[(i + ox) + (j + oy) * width + (k + oz) * width* height]);
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return Color.FromArgb(value.Hash());
        }
    }

    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.FlowBundle)]
    [FlowDesignerCategory(FlowCategory.Add)]
    [FlowDesignerName("Bundle Set Biome")]
    public class AlgorithmBundleSetBiome : Algorithm<FlowBundle, Biome, FlowBundle>
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

        public AlgorithmBundleSetBiome()
        {
            this.Layer2D = true;
            this.Identifier = "Unassigned";
        }

        public override void ProcessCell(IRuntimeContext context, FlowBundle[] inputA, Biome[] inputB, FlowBundle[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] =
				inputA[(i + ox) + (j + oy) * width + (k + oz) * width* height].Set(
					this.Identifier, 
					inputB[(i + ox) + (j + oy) * width + (k + oz) * width* height]);
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return Color.FromArgb(value.Hash());
        }
    }

}