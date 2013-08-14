// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using Tychaia.ProceduralGeneration.Towns;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Towns)]
    [FlowDesignerName("Initial Towns")]
    public class AlgorithmInitialTowns : Algorithm<Town>
    {
        public AlgorithmInitialTowns()
        {
            this.Limit = 0.9;
            this.GuaranteeStartingPoint = true;
            this.Layer2D = true;
        }

        [DataMember]
        [DefaultValue(0.9)]
        [Description("The value between 0.0 and 1.0 above which the cell is selected.")]
        public double Limit { get; set; }

        [DataMember]
        [DefaultValue(true)]
        [Description("Whether to guarantee a town at the global (0, 0) position.")]
        public bool GuaranteeStartingPoint { get; set; }

        [DataMember]
        [DefaultValue(true)]
        [Description("Show this layer as 2D in the editor.")]
        public bool Layer2D { get; set; }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public override void ProcessCell(IRuntimeContext context, Town[] output, long x, long y, long z, int i, int j,
            int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            if (this.GuaranteeStartingPoint && x == 0 && y == 0)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = new Town();
            else if (!this.Layer2D &&
                     AlgorithmUtility.GetRandomDouble(context.Seed, x, y, z, context.Modifier) > this.Limit)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = new Town();
            else if (this.Layer2D &&
                     AlgorithmUtility.GetRandomDouble(context.Seed, x, y, 0, context.Modifier) > this.Limit)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = new Town();
            else
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = null;
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (value == null)
            {
                return Color.Gray;
            }

            return value.BrushColor;
        }
    }
}
