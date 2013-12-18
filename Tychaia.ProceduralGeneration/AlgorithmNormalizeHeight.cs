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
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Manipulation)]
    [FlowDesignerName("Normalize Terrain")]
    public class AlgorithmNormalizeHeight : Algorithm<int, int>
    {
        public AlgorithmNormalizeHeight()
        {
        }

        public override int[] RequiredXBorder
        {
            get { return new[] { 1 }; }
        }

        public override int[] RequiredYBorder
        {
            get { return new[] { 1 }; }
        }

        public override string[] InputNames
        {
            get { return new[] { "Height Map" }; }
        }

        public override bool Is2DOnly
        {
            get { return true; }
        }

        public override bool[] InputIs2D
        {
            get { return new[] { true }; }
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

            var value = input[(i + ox) + ((j + oy) * width)];

            if ((east == value || west == value || north == value) && south <= value - 2)
            {
                value -= 1;
            }

            if ((east == value || west == value || south == value) && north <= value - 2)
            {
                value -= 1;
            }

            if (west <= value - 2 && (east == value || north == value || south == value))
            {
                value -= 1;
            }

            if (east <= value - 2 && (west == value || north == value || south == value))
            {
                value -= 1;
            }
            

            output[(i + ox) + ((j + oy) * width)] = value;
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
