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
    [FlowDesignerName("Edge Detection")]
    public class AlgorithmEdgeDetection : Algorithm<int, int>
    {
        public AlgorithmEdgeDetection()
        {
        }

        public override bool Is2DOnly
        {
            get { return false; }
        }

        public override bool[] InputIs2D
        {
            get { return new[] { false }; }
        }

        public override string[] InputNames
        {
            get { return new[] { "Terrain" }; }
        }

        public override int[] RequiredXBorder
        {
            get { return new[] { 1 }; }
        }

        public override int[] RequiredYBorder
        {
            get { return new[] { 1 }; }
        }

        public override int[] RequiredZBorder
        {
            get { return new[] { 1 }; }
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
            var above = input[(i + ox) + ((j + oy) * width) + ((k + 1 + oz) * width * height)];
            var below = input[(i + ox) + ((j + oy) * width) + ((k - 1 + oz) * width * height)];
            var east = input[(i + 1 + ox) + ((j + oy) * width) + ((k + oz) * width * height)];
            var west = input[(i - 1 + ox) + ((j + oy) * width) + ((k + oz) * width * height)];
            var north = input[(i + ox) + ((j - 1 + oy) * width) + ((k + oz) * width * height)];
            var south = input[(i + ox) + ((j + 1 + oy) * width) + ((k + oz) * width * height)];
            var northEast = input[(i + 1 + ox) + ((j - 1 + oy) * width) + ((k + oz) * width * height)];
            var northWest = input[(i - 1 + ox) + ((j - 1 + oy) * width) + ((k + oz) * width * height)];
            var southEast = input[(i + 1 + ox) + ((j + 1 + oy) * width) + ((k + oz) * width * height)];
            var southWest = input[(i - 1 + ox) + ((j + 1 + oy) * width) + ((k + oz) * width * height)];

            var value = 0;

            if (above == int.MaxValue)
            {
                value |= 1;
            }

            if (below == int.MaxValue)
            {
                value |= 2;
            }

            if (east == int.MaxValue)
            {
                value |= 4;
            }

            if (west == int.MaxValue)
            {
                value |= 8;
            }

            if (north == int.MaxValue)
            {
                value |= 16;
            }

            if (south == int.MaxValue)
            {
                value |= 32;
            }

            if (northEast == int.MaxValue)
            {
                value |= 64;
            }

            if (northWest == int.MaxValue)
            {
                value |= 128;
            }

            if (southEast == int.MaxValue)
            {
                value |= 256;
            }

            if (southWest == int.MaxValue)
            {
                value |= 512;
            }

            output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] = value;
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            var red = ((value & 1) != 0 ? 32 : 0) + ((value & 2) != 0 ? 128 : 0);
            var green = ((value & 4) != 0 ? 32 : 0) + ((value & 8) != 0 ? 128 : 0);
            var blue = ((value & 16) != 0 ? 32 : 0) + ((value & 32) != 0 ? 128 : 0);

            return Color.FromArgb(red, green, blue);
        }
    }
}
