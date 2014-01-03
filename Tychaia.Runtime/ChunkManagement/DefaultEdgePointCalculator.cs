// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.Data;

namespace Tychaia.Runtime
{
    public class DefaultEdgePointCalculator : IEdgePointCalculator
    {
        public EdgePoint CalculateEdgePoint(int edges)
        {
            var missingAbove = (edges & 0x00000001) != 0;
            var missingBelow = (edges & 0x00000002) != 0;
            var missingEast = (edges & 0x00000004) != 0;
            var missingWest = (edges & 0x00000008) != 0;
            var missingNorth = (edges & 0x00000010) != 0;
            var missingSouth = (edges & 0x00000020) != 0;
            var missingNorthEast = (edges & 0x00000040) != 0;
            var missingNorthWest = (edges & 0x00000080) != 0;
            var missingSouthEast = (edges & 0x00000100) != 0;
            var missingSouthWest = (edges & 0x00000200) != 0;
            var missingBelowEast = (edges & 0x00000400) != 0;
            var missingBelowWest = (edges & 0x00000800) != 0;
            var missingBelowNorth = (edges & 0x00001000) != 0;
            var missingBelowSouth = (edges & 0x00002000) != 0;
            var missingBelowNorthEast = (edges & 0x00004000) != 0;
            var missingBelowNorthWest = (edges & 0x00008000) != 0;
            var missingBelowSouthEast = (edges & 0x00010000) != 0;
            var missingBelowSouthWest = (edges & 0x00020000) != 0;
            var missingAboveAbove = (edges & 0x00040000) != 0;
            var missingAboveEast = (edges & 0x00080000) != 0;
            var missingAboveWest = (edges & 0x00100000) != 0;
            var missingAboveNorth = (edges & 0x00200000) != 0;
            var missingAboveSouth = (edges & 0x00400000) != 0;
            var missingAboveNorthEast = (edges & 0x00800000) != 0;
            var missingAboveNorthWest = (edges & 0x01000000) != 0;
            var missingAboveSouthEast = (edges & 0x02000000) != 0;
            var missingAboveSouthWest = (edges & 0x04000000) != 0;
            var missingBelowBelowNorthEast = (edges & 0x08000000) != 0;
            var missingBelowBelowNorthWest = (edges & 0x10000000) != 0;
            var missingBelowBelowSouthEast = (edges & 0x20000000) != 0;
            var missingBelowBelowSouthWest = (edges & 0x40000000) != 0;

            var edgePoint = new EdgePoint();

            if (missingAbove)
            {
                if (!(missingEast && missingWest))
                {
                    if (missingEast && !missingBelowEast)
                    {
                        edgePoint.TopRightCorner = 1;
                        edgePoint.BottomRightCorner = 1;
                    }

                    if (missingWest && !missingBelowWest)
                    {
                        edgePoint.TopLeftCorner = 1;
                        edgePoint.BottomLeftCorner = 1;
                    }
                }

                if (!(missingNorth && missingSouth))
                {
                    if (missingNorth && !missingBelowNorth)
                    {
                        edgePoint.TopLeftCorner = 1;
                        edgePoint.TopRightCorner = 1;
                    }

                    if (missingSouth && !missingBelowSouth)
                    {
                        edgePoint.BottomLeftCorner = 1;
                        edgePoint.BottomRightCorner = 1;
                    }
                }

                if (missingSouthEast && !missingNorthEast && !missingSouthWest && !missingBelowSouthEast)
                {
                    edgePoint.BottomRightCorner = 1;
                }

                if (missingNorthEast && !missingSouthEast && !missingNorthWest && !missingBelowNorthEast)
                {
                    edgePoint.TopRightCorner = 1;
                }

                if (missingSouthWest && !missingNorthWest && !missingSouthEast && !missingBelowSouthWest)
                {
                    edgePoint.BottomLeftCorner = 1;
                }

                if (missingNorthWest && !missingSouthWest && !missingNorthEast && !missingBelowNorthWest)
                {
                    edgePoint.TopLeftCorner = 1;
                }
            }

            if (missingAbove && missingBelowSouthEast && !missingBelowBelowSouthEast && !missingBelowNorth
                && !missingBelowWest && !missingBelowNorthWest && !missingBelowNorthEast && !missingBelowSouth
                && !missingBelowEast && !missingBelowSouthWest && missingSouth && missingEast && missingSouthEast)
            {
                edgePoint.BottomRightCorner = 2;
                edgePoint.LowerBottomRightCorner = 1;
            }

            if (missingAboveAbove && !missingAbove && !missingBelowSouthEast && missingSouthEast && !missingNorth
                && !missingWest && !missingNorthWest && !missingNorthEast && !missingSouth && !missingEast
                && !missingSouthWest && missingAboveSouth && missingAboveEast && missingAboveSouthEast)
            {
                edgePoint.BottomRightCorner = 1;
            }

            if (missingAbove && missingBelowSouthWest && !missingBelowBelowSouthWest && !missingBelowNorth
                && !missingBelowWest && !missingBelowNorthWest && !missingBelowNorthEast && !missingBelowSouth
                && !missingBelowEast && !missingBelowSouthEast && missingSouth && missingWest && missingSouthWest)
            {
                edgePoint.BottomLeftCorner = 2;
                edgePoint.LowerBottomLeftCorner = 1;
            }

            if (missingAboveAbove && !missingAbove && !missingBelowSouthWest && missingSouthWest && !missingNorth
                && !missingWest && !missingNorthWest && !missingNorthEast && !missingSouth && !missingEast
                && !missingSouthEast && missingAboveSouth && missingAboveWest && missingAboveSouthWest)
            {
                edgePoint.BottomLeftCorner = 1;
            }

            if (missingAbove && missingBelowNorthEast && !missingBelowBelowNorthEast && !missingBelowNorth
                && !missingBelowWest && !missingBelowNorthWest && !missingBelowSouth && !missingBelowEast
                && !missingBelowSouthWest && !missingBelowSouthEast && missingNorth && missingEast && missingNorthEast)
            {
                edgePoint.TopRightCorner = 2;
                edgePoint.LowerTopRightCorner = 1;
            }

            if (missingAboveAbove && !missingAbove && !missingBelowNorthEast && missingNorthEast && !missingNorth
                && !missingWest && !missingNorthWest && !missingSouth && !missingEast && !missingSouthWest
                && !missingSouthEast && missingAboveNorth && missingAboveEast && missingAboveNorthEast)
            {
                edgePoint.TopRightCorner = 1;
            }

            if (missingAbove && missingBelowNorthWest && !missingBelowBelowNorthWest && !missingBelowNorth
                && !missingBelowWest && !missingBelowNorthEast && !missingBelowSouth && !missingBelowEast
                && !missingBelowSouthEast && !missingBelowSouthWest && missingNorth && missingWest && missingNorthWest)
            {
                edgePoint.TopLeftCorner = 2;
                edgePoint.LowerTopLeftCorner = 1;
            }

            if (missingAboveAbove && !missingAbove && !missingBelowNorthWest && missingNorthWest && !missingNorth
                && !missingWest && !missingNorthEast && !missingSouth && !missingEast && !missingSouthEast
                && !missingSouthWest && missingAboveNorth && missingAboveWest && missingAboveNorthWest)
            {
                edgePoint.TopLeftCorner = 1;
            }

            edgePoint.RenderAbove = missingAbove;
            edgePoint.RenderBelow = missingBelow;
            edgePoint.RenderWest = missingWest || missingNorthWest || missingSouthWest
                                   || (missingSouth && !missingAbove) || (missingNorth && !missingAbove);
            edgePoint.RenderEast = missingEast || missingNorthEast || missingSouthEast
                                   || (missingSouth && !missingAbove) || (missingNorth && !missingAbove);
            edgePoint.RenderNorth = missingNorth || missingNorthEast || missingNorthWest
                                    || (missingEast && !missingAbove) || (missingWest && !missingAbove);
            edgePoint.RenderSouth = missingSouth || missingSouthEast || missingSouthWest
                                    || (missingEast && !missingAbove) || (missingWest && !missingAbove);

            return edgePoint;
        }
    }
}