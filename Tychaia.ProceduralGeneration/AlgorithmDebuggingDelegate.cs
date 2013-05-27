//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.Undefined)]
    [FlowDesignerCategory(FlowCategory.Debugging)]
    [FlowDesignerName("Delegate")]
    public class AlgorithmDebuggingDelegate : Algorithm<int, int>
    {
        public override int[] RequiredXBorder { get { return new int[] { 2 }; } }
        public override int[] RequiredYBorder { get { return new int[] { 2 }; } }
        public override int[] RequiredZBorder { get { return new int[] { 2 }; } }

        public bool ShowAs2D
        {
            get;
            set;
        }

        public override bool Is2DOnly
        {
            get { return this.ShowAs2D; }
        }

        public Action<IRuntimeContext, int[], int[], long, long, long, int, int, int, int, int, int, int, int, int> Delegate
        {
            get;
            set;
        }

        public override string[] InputNames
        {
            get
            {
                return new string[] { "Parent" };
            }
        }

        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            if (this.Delegate == null)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = 0;
            else
                this.Delegate(context, input, output, x, y, z, i, j, k, width, height, depth, ox, oy, oz);
        }

        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (value == 0)
                return System.Drawing.Color.FromArgb(1, 0, 0, 0);
            else
                return System.Drawing.Color.FromArgb(255, 0, 0);
        }
    }
}

