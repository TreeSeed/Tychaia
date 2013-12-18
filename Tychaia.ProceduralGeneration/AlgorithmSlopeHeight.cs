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
    [FlowDesignerName("Slope Terrain")]
    public class AlgorithmSlopeHeight : Algorithm<int, int>
    {
        public AlgorithmSlopeHeight()
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

            if (east == value && west == value && north == value && (value >= 0 ? (south <= value - 2) : (south >= value + 2)))
            {
                value -= value >= 0 ? 1 : -1;
            }

            if (east == value && west == value && south == value && (value >= 0 ? (north <= value - 2) : (north >= value + 2)))
            {
                value -= value >= 0 ? 1 : -1;
            }

            if ((value >= 0 ? (west <= value - 2) : (west >= value + 2)) && east == value && north == value && south == value)
            {
                value -= value >= 0 ? 1 : -1;
            }

            if ((value >= 0 ? (east <= value - 2) : (east >= value + 2)) && west == value && north == value && south == value)
            {
                value -= value >= 0 ? 1 : -1;
            }
            
            output[(i + ox) + ((j + oy) * width)] = value == 0 ? 1 : value;
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
