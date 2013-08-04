//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using Mono.CSharp;
using Tychaia.ProceduralGeneration.Blocks;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Output)]
    [FlowDesignerName("Store Result Blocks")]
    public class AlgorithmResultBlocks : Algorithm<int, BlockInfo>
    {
        [DataMember]
        [DefaultValue(false)]
        [Description("Whether to display this layer as 2D in the editor.")]
        public bool Layer2D
        {
            get;
            set;
        }

        public override string[] InputNames
        {
            get { return new string[] { "Input" }; }
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public override void ProcessCell(IRuntimeContext context, int[] input, BlockInfo[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
            {
            if (input[(i + ox) + (j + oy)*width + (k + oz)*width*height] == Int32.MaxValue && z > 0)
            {
                output[(i + ox) + (j + oy)*width + (k + oz)*width*height] = new BlockInfo(null);
            }
            else if (input[(i + ox) + (j + oy)*width + (k + oz)*width*height] >= 0)
            {
                output[(i + ox) + (j + oy)*width + (k + oz)*width*height] = new BlockInfo("block.Grass");
            }
            else if (input[(i + ox) + (j + oy) * width + (k + oz) * width * height] < 0 && input[(i + ox) + (j + oy) * width + (k + oz) * width * height] == Int32.MaxValue)
            {
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = new BlockInfo("block.Water");
            }
            else
            {
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = new BlockInfo("block.Dirt");
            }
        }

        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}

