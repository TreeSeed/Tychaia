// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.FlowBundle)]
    [FlowDesignerCategory(FlowCategory.Extract)]
    [FlowDesignerName("Bundle Get Int32")]
    public class AlgorithmBundleGetInt32 : Algorithm<FlowBundle, Int32, Int32>
    {
        [DataMember]
        [DefaultValue("Unassigned Int32")]
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
        
        public override bool[] InputIs2D
        {
            get { return new[] { this.Layer2D }; }
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public AlgorithmBundleGetInt32()
        {
            this.Layer2D = true;
            this.Identifier = "Unassigned";
        }

        public override void ProcessCell(IRuntimeContext context, FlowBundle[] inputA, Int32[] inputB, Int32[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
             output[(i + ox) + (j + oy) * width + (k + oz) * width* height] =
                inputA[(i + ox) + (j + oy) * width + (k + oz) * width * height].Get(this.Identifier);
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value, 1);
        }
    }

    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.FlowBundle)]
    [FlowDesignerCategory(FlowCategory.Extract)]
    [FlowDesignerName("Bundle Get Biome")]
    public class AlgorithmBundleGetBiome : Algorithm<FlowBundle, Biome, Biome>
    {
        [DataMember]
        [DefaultValue("Unassigned Biome")]
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
        
        public override bool[] InputIs2D
        {
            get { return new[] { this.Layer2D }; }
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public AlgorithmBundleGetBiome()
        {
            this.Layer2D = true;
            this.Identifier = "Unassigned";
        }

        public override void ProcessCell(IRuntimeContext context, FlowBundle[] inputA, Biome[] inputB, Biome[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
             output[(i + ox) + (j + oy) * width + (k + oz) * width* height] =
                inputA[(i + ox) + (j + oy) * width + (k + oz) * width * height].Get(this.Identifier);
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value, 1);
        }
    }

    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.FlowBundle)]
    [FlowDesignerCategory(FlowCategory.Extract)]
    [FlowDesignerName("Bundle Get BlockInfo")]
    public class AlgorithmBundleGetBlockInfo : Algorithm<FlowBundle, BlockInfo, BlockInfo>
    {
        [DataMember]
        [DefaultValue("Unassigned BlockInfo")]
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
        
        public override bool[] InputIs2D
        {
            get { return new[] { this.Layer2D }; }
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public AlgorithmBundleGetBlockInfo()
        {
            this.Layer2D = true;
            this.Identifier = "Unassigned";
        }

        public override void ProcessCell(IRuntimeContext context, FlowBundle[] inputA, BlockInfo[] inputB, BlockInfo[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
             output[(i + ox) + (j + oy) * width + (k + oz) * width* height] =
                inputA[(i + ox) + (j + oy) * width + (k + oz) * width * height].Get(this.Identifier);
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value, 1);
        }
    }

}
