// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using Tychaia.Data;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Output)]
    [FlowDesignerName("GenerateBlockInfo")]
    public class AlgorithmGenerateBlockInfo : Algorithm<int, Cell>
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

        public override bool[] InputIs2D
        {
            get { return new[] { this.Layer2D }; }
        }

        public override void ProcessCell(
            IRuntimeContext context,
            int[] input,
            Cell[] output,
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
            var value = input[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)];
            string result = null;
            if (value <= 0 && value >= -5)
                result = "block.Sand";
            else if (value <= -5)
                result = "block.Dirt";
            else if (value == int.MaxValue && z <= 0)
                result = "block.Water";
            else if (value == int.MaxValue)
                result = null;
            else if (input[(i + ox) + ((j + oy) * width) + ((k + oz + 1) * width * height)] == int.MaxValue && z == 1)
                result = "block.Sand";
            else if (input[(i + ox) + ((j + oy) * width) + ((k + oz + 1) * width * height)] == int.MaxValue)
                result = "block.Grass";
            else
                result = "block.Dirt";
            output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)].BlockAssetName = result;
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            switch ((string)value.BlockAssetName)
            {
                case "block.Sand":
                    return Color.Yellow;
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
