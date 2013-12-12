// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
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
            this.Layer2D = true;
            this.ColorSet = ColorScheme.Beings;
            this.LowerValue = 0;
            this.HigherValue = 50;
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
        [DefaultValue(0)]
        [Description("The Lower value selected.")]
        public int LowerValue { get; set; }

        [DataMember]
        [DefaultValue(1)]
        [Description("The Higher value selected.")]
        public int HigherValue { get; set; }

        [DataMember]
        [DefaultValue(ColorScheme.Beings)]
        [Description("The color scheme to use.")]
        public ColorScheme ColorSet { get; set; }

        [DataMember]
        [DefaultValue(true)]
        [Description("Show this layer as 2D in the editor.")]
        public bool Layer2D { get; set; }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        // Will be able to use this algorithm for:
        // Land - This is the equivelent of InitialLand
        // Towns - This is the equivelent of InitialTowns
        // Landmarks - We can spread landmarks over the world, which we can then use values to determine the size/value of the landmarks.
        // Monsters - By utilising multiple value scaling we can either distribute individual monsters or monster groups or even monster villages.
        // Tresure chests - Spreading tresure chests in dungeons (can be used as an estimated location then moved slightly too).
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
            else if (!this.Layer2D && AlgorithmUtility.GetRandomDouble(context.Seed, x, y, z, context.Modifier) > this.Limit)
                numBeings = AlgorithmUtility.GetRandomRange(context.Seed, x, y, z, this.LowerValue, this.HigherValue, context.Modifier);
            else if (this.Layer2D && AlgorithmUtility.GetRandomDouble(context.Seed, x, y, 0, context.Modifier) > this.Limit)
                numBeings = AlgorithmUtility.GetRandomRange(context.Seed, x, y, 0, this.LowerValue, this.HigherValue, context.Modifier);
            else
                numBeings = 0;

            var outputCell = new Cell();

            // output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] 
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (this.ColorSet == ColorScheme.Perlin)
            {
                if (value == this.LowerValue)
                    return Color.White;
                return Color.Black;
            }

            if (this.ColorSet == ColorScheme.Beings)
            {
                if (value == this.LowerValue)
                    return Color.LightGray;
                
                return Color.Red;
            }

            return Color.Gray;
        }

        public enum ColorScheme
        {
            Beings,
            Perlin,
        }
    }
}
