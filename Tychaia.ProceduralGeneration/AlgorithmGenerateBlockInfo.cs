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

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Output)]
    [FlowDesignerName("GenerateBlockInfo")]
    public class AlgorithmGenerateBlockInfo : Algorithm<int, BlockInfo>
    {
        [DataMember]
        [DefaultValue(false)]
        [Description("Whether to display this layer as 2D in the editor.")]
        public bool Layer2D { get; set; }

        public override string[] InputNames
        {
            get { return new[] { "Input" }; }
        }

        public override int[] RequiredZBorder
        {
            get { return new[] { 2 }; }
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public override void ProcessCell(IRuntimeContext context, int[] input, BlockInfo[] output, long x, long y,
            long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            BlockInfo result;
            var value = input[(i + ox) + (j + oy) * width + (k + oz) * width * height];
            if (value <= 0 && input[(i + ox) + (j + oy) * width + (k + oz + 1) * width * height] == Int32.MaxValue)
                result = new BlockInfo("block.Water");
            else if (value == Int32.MaxValue || value <= 0)
                result = new BlockInfo(null);
            else if (input[(i + ox) + (j + oy) * width + (k + oz + 1) * width * height] == Int32.MaxValue)
                result = new BlockInfo("block.Grass");
            else
                result = new BlockInfo("block.Dirt");
            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = result;
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            switch ((string)value.ToString())
            {
                case "block.Grass":
                    return Color.Green;
                case "block.Dirt":
                    return Color.Brown;
                case "block.Water":
                    return Color.Blue;
                default:
                    return Color.Transparent;
            }
        }
    }
}