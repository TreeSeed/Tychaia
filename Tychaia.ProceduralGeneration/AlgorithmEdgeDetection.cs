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

            var belowEast = input[(i + 1 + ox) + ((j + oy) * width) + ((k - 1 + oz) * width * height)];
            var belowWest = input[(i - 1 + ox) + ((j + oy) * width) + ((k - 1 + oz) * width * height)];
            var belowNorth = input[(i + ox) + ((j - 1 + oy) * width) + ((k - 1 + oz) * width * height)];
            var belowSouth = input[(i + ox) + ((j + 1 + oy) * width) + ((k - 1 + oz) * width * height)];
            var belowNorthEast = input[(i + 1 + ox) + ((j - 1 + oy) * width) + ((k - 1 + oz) * width * height)];
            var belowNorthWest = input[(i - 1 + ox) + ((j - 1 + oy) * width) + ((k - 1 + oz) * width * height)];
            var belowSouthEast = input[(i + 1 + ox) + ((j + 1 + oy) * width) + ((k - 1 + oz) * width * height)];
            var belowSouthWest = input[(i - 1 + ox) + ((j + 1 + oy) * width) + ((k - 1 + oz) * width * height)];

            var value = 0;

            if (above == int.MaxValue)
            {
                value |= 0x00001;
            }

            if (below == int.MaxValue)
            {
                value |= 0x00002;
            }

            if (east == int.MaxValue)
            {
                value |= 0x00004;
            }

            if (west == int.MaxValue)
            {
                value |= 0x00008;
            }

            if (north == int.MaxValue)
            {
                value |= 0x00010;
            }

            if (south == int.MaxValue)
            {
                value |= 0x00020;
            }

            if (northEast == int.MaxValue)
            {
                value |= 0x00040;
            }

            if (northWest == int.MaxValue)
            {
                value |= 0x00080;
            }

            if (southEast == int.MaxValue)
            {
                value |= 0x00100;
            }

            if (southWest == int.MaxValue)
            {
                value |= 0x00200;
            }

            if (belowEast == int.MaxValue)
            {
                value |= 0x00400;
            }

            if (belowWest == int.MaxValue)
            {
                value |= 0x00800;
            }

            if (belowNorth == int.MaxValue)
            {
                value |= 0x01000;
            }

            if (belowSouth == int.MaxValue)
            {
                value |= 0x02000;
            }

            if (belowNorthEast == int.MaxValue)
            {
                value |= 0x04000;
            }

            if (belowNorthWest == int.MaxValue)
            {
                value |= 0x08000;
            }

            if (belowSouthEast == int.MaxValue)
            {
                value |= 0x10000;
            }

            if (belowSouthWest == int.MaxValue)
            {
                value |= 0x20000;
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
