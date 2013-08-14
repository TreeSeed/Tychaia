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
    [FlowDesignerName("Initial Gradient")]
    public class AlgorithmGradientInitial : Algorithm<int>
    {
        public override bool Is2DOnly
        {
            get { return false; }
        }

        public override void ProcessCell(IRuntimeContext context, int[] output, long x, long y, long z, int i, int j,
            int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = (int) (x + y * 256);
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (value <= 0)
                return Color.FromArgb(0, 0, 255);
            return Color.FromArgb(Math.Max(Math.Min(value / 256, 255), 0), Math.Max(Math.Min(value % 256, 255), 0), 0);
        }
    }

    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.Undefined)]
    [FlowDesignerCategory(FlowCategory.Debugging)]
    [FlowDesignerName("Initial Grid")]
    public class AlgorithmGridInitial : Algorithm<int>
    {
        public override bool Is2DOnly
        {
            get { return false; }
        }

        public override void ProcessCell(IRuntimeContext context, int[] output, long x, long y, long z, int i, int j,
            int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            var a = 0;
            if (x % 4 == 0)
                a += 1;
            if (y % 4 == 0)
                a += 2;
            if (z % 4 == 0)
                a += 4;

            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = a;
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            switch ((int) value)
            {
                default:
                    return Color.FromArgb(150, 150, 150);
                case 0:
                    return Color.FromArgb(0, 0, 0);
                case 1:
                    return Color.FromArgb(255, 0, 0);
                case 2:
                    return Color.FromArgb(0, 255, 0);
                case 3:
                    return Color.FromArgb(255, 255, 0);
                case 4:
                    return Color.FromArgb(0, 0, 255);
                case 5:
                    return Color.FromArgb(255, 0, 255);
                case 6:
                    return Color.FromArgb(0, 255, 255);
                case 7:
                    return Color.FromArgb(255, 255, 255);
            }
        }
    }
}
