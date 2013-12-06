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
    [FlowDesignerName("Initial Delegate")]
    public class AlgorithmDebuggingInitialDelegate : Algorithm<int>
    {
        private static readonly Func<long, long, long, bool> m_Test1 = (x, y, z) => (x == 4 && y == 6 && z == 7);
        private static readonly Func<long, long, long, bool> m_Test2 = (x, y, z) => (y == 0 && z == 0);
        public bool ShowAs2D { get; set; }

        public override bool Is2DOnly
        {
            get { return this.ShowAs2D; }
        }

        public bool Test1
        {
            get { return this.ValueShouldBePlacedAt == m_Test1; }
            set { this.ValueShouldBePlacedAt = value ? m_Test1 : null; }
        }

        public bool Test2
        {
            get { return this.ValueShouldBePlacedAt == m_Test2; }
            set { this.ValueShouldBePlacedAt = value ? m_Test2 : null; }
        }

        public Func<long, long, long, bool> ValueShouldBePlacedAt { get; set; }

        public override void ProcessCell(
            IRuntimeContext context,
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
            if (this.ValueShouldBePlacedAt == null)
                output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] = 0;
            else
                output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] = this.ValueShouldBePlacedAt(x, y, z)
                    ? 1
                    : 0;
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (value == 0)
                return Color.FromArgb(1, 0, 0, 0);
            return Color.FromArgb(255, 0, 0);
        }
    }
}
