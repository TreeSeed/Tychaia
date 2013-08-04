﻿//
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
    [FlowDesignerName("Bundle Extract Int32")]
    public class AlgorithmBundleExtractInt32 : Algorithm<FlowBundle, Int32, Int32>
    {
        [DataMember]
        [DefaultValue("Unassigned")]
        [Description("The identifier for the instance you wish to retrieve from the bundle.")]
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
                return new[] { "FlowBundle", "Color" };
            }
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public AlgorithmBundleExtractInt32()
        {
            this.Layer2D = true;
            this.Identifier = "Unassigned";
        }

        public override void ProcessCell(IRuntimeContext context, FlowBundle[] inputA, Int32[] inputB, Int32[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
             output[(i + ox) + (j + oy)*width + (k + oz)*width*height] = inputA[(i + ox) + (j + oy) * width + (k + oz) * width * height].ExtractValue(Identifier);
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value, 1);
        }
    }

    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.FlowBundle)]
    [FlowDesignerName("Bundle Extract Biome")]
    public class AlgorithmBundleExtractBiome : Algorithm<FlowBundle, Biome, Biome>
    {
        [DataMember]
        [DefaultValue("Unassigned")]
        [Description("The identifier for the instance you wish to retrieve from the bundle.")]
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
                return new[] { "FlowBundle", "Color" };
            }
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public AlgorithmBundleExtractBiome()
        {
            this.Layer2D = true;
            this.Identifier = "Unassigned";
        }

        public override void ProcessCell(IRuntimeContext context, FlowBundle[] inputA, Biome[] inputB, Biome[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
             output[(i + ox) + (j + oy)*width + (k + oz)*width*height] = inputA[(i + ox) + (j + oy) * width + (k + oz) * width * height].ExtractValue(Identifier);
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value, 1);
        }
    }

}