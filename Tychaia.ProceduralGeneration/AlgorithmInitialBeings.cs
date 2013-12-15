// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using Tychaia.Asset;
using Tychaia.Data;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Beings)]
    [FlowDesignerName("Initial Beings")]
    public class AlgorithmInitialBeings : Algorithm<Cell>
    {
        public AlgorithmInitialBeings()
        {
            this.Limit = 0.9;
            this.GuaranteeStartingPoint = true;
            this.ColorSet = ColorScheme.Beings;
            this.PlacementType = BeingType.Enemies;
        }

        [DataMember]
        [DefaultValue(0.9)]
        [Description("The value between 0.0 and 1.0 above which the cell is selected.")]
        public double Limit { get; set; }

        [DataMember]
        [DefaultValue(true)]
        [Description("Whether to guarantee a 0 value at the global (0, 0) position.")]
        public bool GuaranteeStartingPoint { get; set; }

        [DataMember]
        [DefaultValue(ColorScheme.Beings)]
        [Description("The color scheme to use.")]
        public ColorScheme ColorSet { get; set; }

        [DataMember]
        [DefaultValue(BeingType.Enemies)]
        [Description("Determines if you spawn \"Enemies\" or \"NPCs\".")]
        public BeingType PlacementType
        {
            get;
            set;
        }

        public override bool Is2DOnly
        {
            get { return true; }
        }

        public override void ProcessCell(
            IRuntimeContext context,
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
            var numBeings = 0;
            if (this.GuaranteeStartingPoint && x == 0 && y == 0)
                numBeings = 0;
            else if (AlgorithmUtility.GetRandomDouble(context.Seed, x, y, 0, context.Modifier) > this.Limit)
                numBeings = 1;
            //// numBeings = AlgorithmUtility.GetRandomRange(context.Seed, x, y, 0, this.LowerValue, this.HigherValue, context.Modifier);
            else
                numBeings = 0;

            var outputCell = new Cell();

            // output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] 
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (this.ColorSet == ColorScheme.Perlin)
            {
                if (string.IsNullOrEmpty(value.ClusterDefinitionAssetName))
                    return Color.White;
                return Color.Black;
            }

            if (this.ColorSet == ColorScheme.Beings)
            {
                if (string.IsNullOrEmpty(value.ClusterDefinitionAssetName))
                    return Color.White;
                
                return Color.Red;
            }

            return Color.Gray;
        }
        
        public enum ColorScheme
        {
            Beings,
            Perlin,
        }

        public enum BeingType
        {
            Enemies,
            NPCs,
        }
    }
}
