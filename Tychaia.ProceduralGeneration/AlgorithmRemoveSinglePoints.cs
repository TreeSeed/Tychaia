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
    [FlowDesignerName("Eliminate Single Points")]
    public class AlgorithmRemoveSinglePoints : Algorithm<int, int>
    {
        public AlgorithmRemoveSinglePoints()
        {
        }

        public override bool Is2DOnly
        {
            get { return true; }
        }

        public override bool[] InputIs2D
        {
            get { return new[] { true }; }
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
            var self = input[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)];
            var east = input[(i + 1 + ox) + ((j + oy) * width) + ((k + oz) * width * height)];
            var west = input[(i - 1 + ox) + ((j + oy) * width) + ((k + oz) * width * height)];
            var north = input[(i + ox) + ((j - 1 + oy) * width) + ((k + oz) * width * height)];
            var south = input[(i + ox) + ((j + 1 + oy) * width) + ((k + oz) * width * height)];
            var northEast = input[(i + 1 + ox) + ((j - 1 + oy) * width) + ((k + oz) * width * height)];
            var northWest = input[(i - 1 + ox) + ((j - 1 + oy) * width) + ((k + oz) * width * height)];
            var southEast = input[(i + 1 + ox) + ((j + 1 + oy) * width) + ((k + oz) * width * height)];
            var southWest = input[(i - 1 + ox) + ((j + 1 + oy) * width) + ((k + oz) * width * height)];

            /*
             * Eliminate the following bad arrangements:
             * 
             *    A       B       C       D
             * 
             *   ___     __X     __X     X_X
             *   _X_     _X_     _X_     _X_
             *   ___     ___     X__     X_X
             * 
             * 
             *    E       F       G
             * 
             *   _XX     _XX     _X_
             *   _X_     _X_     _X_
             *   ___     XX_     _X_
             * 
             */

            var eliminate = false;

            // A
            if (east <= self - 1 && west <= self - 1 && north <= self - 1 && south <= self - 1 && 
                northEast <= self - 1 && northWest <= self - 1 && southEast <= self - 1 && southWest <= self - 1)
            {
                eliminate = true;
            }

            // B, C & D
            if (east <= self - 1 && west <= self - 1 && north <= self - 1 && south <= self - 1)
            {
                // If we don't have any side cells, then we're invalid no matter how
                // many diagonals there are.
                eliminate = true;
            }

            // E
            if ((east >= self && west <= self - 1 && north <= self - 1 && south <= self - 1) ||
                (east <= self - 1 && west >= self && north <= self - 1 && south <= self - 1) ||
                (east <= self - 1 && west <= self - 1 && north >= self && south <= self - 1) ||
                (east <= self - 1 && west <= self - 1 && north <= self - 1 && south >= self))
            {
                eliminate = true;
            }

            // F & G
            {
                // If we have two side cells in opposing directions, but not a third cell
                // perpendicular, then it's not a valid arrangement.
                if (east >= self && west >= self)
                {
                    if (north <= self - 1 && south <= self - 1)
                    {
                        eliminate = true;
                    }
                }

                if (north >= self && south >= self)
                {
                    if (east <= self - 1 && west <= self - 1)
                    {
                        eliminate = true;
                    }
                }
            }

            if (eliminate)
            {
                output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] = self - 1;
            }
            else
            {
                output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] = self;
            }
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
