// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.Undefined)]
    [FlowDesignerCategory(FlowCategory.Debugging)]
    [FlowDesignerName("Initial Value Delegate")]
    public class AlgorithmDebuggingInitialValueDelegate : Algorithm<int>
    {
        public bool ShowAs2D { get; set; }

        public override bool Is2DOnly
        {
            get { return this.ShowAs2D; }
        }

        public Func<long, long, long, int, int, int, int> GetValueForPosition { get; set; }

        public override void ProcessCell(IRuntimeContext context, int[] output, long x, long y, long z, int i, int j,
            int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            if (this.GetValueForPosition == null)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = 0;
            else
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = this.GetValueForPosition(x, y, z, i, j,
                    k);
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (value == 0)
                return Color.FromArgb(1, 0, 0, 0);
            return Color.FromArgb(255, 0, 0);
        }
    }
}
