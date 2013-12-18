// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.Undefined)]
    [FlowDesignerCategory(FlowCategory.Buggy)]
    [FlowDesignerName("Terrain Extender")]
    public class AlgorithmTerrainExtender : Algorithm<int, int>
    {
    // This layer will end up screwing the water depth.
        public AlgorithmTerrainExtender()
        {
            this.Limit = 0.8;
            this.Layer2D = true;
            this.ColorSet = ColorScheme.Land;
        }

        [DataMember]
        [DefaultValue(0.8)]
        [Description("The value between 0.0 and 1.0 above which the cell is selected.")]
        public double Limit { get; set; }

        [DataMember]
        [DefaultValue(ColorScheme.Land)]
        [Description("The color scheme to use.")]
        public ColorScheme ColorSet { get; set; }

        [DataMember]
        [DefaultValue(true)]
        [Description("Show this layer as 2D in the editor.")]
        public bool Layer2D { get; set; }

        public override int[] RequiredXBorder
        {
            get { return new[] { 1 }; }
        }

        public override int[] RequiredYBorder
        {
            get { return new[] { 1 }; }
        }
        
        public override bool Is2DOnly
        {
            get { return true; }
        }

        public override bool[] InputIs2D
        {
            get
            {
                return new[] { true };
            }
        }

        public override string[] InputNames
        {
            get
            {
                return new[] { "Terrain" };
            }
        }

        public override void ProcessCell(
            IRuntimeContext context,
            int[] input,
            int[] output,
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
            var east = input[(i + 1 + ox) + ((j + oy) * width)];
            var west = input[(i - 1 + ox) + ((j + oy) * width)];
            var north = input[(i + ox) + ((j - 1 + oy) * width)];
            var south = input[(i + ox) + ((j + 1 + oy) * width)];
            
            var self = input[(i + ox) + ((j + oy) * width)];
            var value = self;

            if (self == -1 && AlgorithmUtility.GetRandomDouble(context.Seed, x, y, 0, context.Modifier) > this.Limit)
                value = 1;
            else if (east == -1 && self < 0 && AlgorithmUtility.GetRandomDouble(context.Seed, x + 1, y, 0, context.Modifier) > this.Limit)
                value = -1;
            else if (west == -1 && self < 0 && AlgorithmUtility.GetRandomDouble(context.Seed, x - 1, y, 0, context.Modifier) > this.Limit)
                value = -1;
            else if (north == -1 && self < 0 && AlgorithmUtility.GetRandomDouble(context.Seed, x, y - 1, 0, context.Modifier) > this.Limit)
                value = -1;
            else if (south == -1 && self < 0 && AlgorithmUtility.GetRandomDouble(context.Seed, x, y + 1, 0, context.Modifier) > this.Limit)
                value = -1;

            output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] = value;
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
