// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections;

namespace Tychaia.Data
{
    public class EdgePoint
    {
        public int BottomLeftCorner { get; set; }

        public int BottomRightCorner { get; set; }

        public int LowerBottomLeftCorner { get; set; }

        public int LowerBottomRightCorner { get; set; }

        public int LowerTopLeftCorner { get; set; }

        public int LowerTopRightCorner { get; set; }

        public int TopLeftCorner { get; set; }

        public int TopRightCorner { get; set; }

        public bool RenderAbove { get; set; }

        public bool RenderBelow { get; set; }

        public bool RenderEast { get; set; }

        public bool RenderWest
        {
            get;
            set;
        }

        public bool RenderNorth
        {
            get;
            set;
        }

        public bool RenderSouth
        {
            get;
            set;
        }

        public static EdgePoint Decompress(short b)
        {
            var edgePoint = new EdgePoint();
            var bitArray = new BitArray(BitConverter.GetBytes(b));

            edgePoint.TopLeftCorner = bitArray.Get(0) ? 1 : bitArray.Get(4) ? 2 : 0;
            edgePoint.TopRightCorner = bitArray.Get(1) ? 1 : bitArray.Get(5) ? 2 : 0;
            edgePoint.BottomLeftCorner = bitArray.Get(2) ? 1 : bitArray.Get(6) ? 2 : 0;
            edgePoint.BottomRightCorner = bitArray.Get(3) ? 1 : bitArray.Get(7) ? 2 : 0;

            edgePoint.LowerTopLeftCorner = bitArray.Get(4) ? 1 : 0;
            edgePoint.LowerTopRightCorner = bitArray.Get(5) ? 1 : 0;
            edgePoint.LowerBottomLeftCorner = bitArray.Get(6) ? 1 : 0;
            edgePoint.LowerBottomRightCorner = bitArray.Get(7) ? 1 : 0;

            edgePoint.RenderAbove = bitArray.Get(8);
            edgePoint.RenderBelow = bitArray.Get(9);
            edgePoint.RenderNorth = bitArray.Get(10);
            edgePoint.RenderSouth = bitArray.Get(11);
            edgePoint.RenderEast = bitArray.Get(12);
            edgePoint.RenderWest = bitArray.Get(13);

            return edgePoint;
        }

        public short Compress()
        {
            // We can infer that the upper corners have a value of 2
            // because they are only set to 2 when the lower values are 1.
            var bits = new[]
            {
                this.TopLeftCorner == 1, this.TopRightCorner == 1, this.BottomLeftCorner == 1, this.BottomRightCorner == 1, 
                this.LowerTopLeftCorner == 1, this.LowerTopRightCorner == 1, this.LowerBottomLeftCorner == 1, 
                this.LowerBottomRightCorner == 1,
                this.RenderAbove,
                this.RenderBelow,
                this.RenderNorth,
                this.RenderSouth,
                this.RenderEast,
                this.RenderWest
            };

            var bitArray = new BitArray(bits);
            var array = new byte[2];
            bitArray.CopyTo(array, 0);
            return BitConverter.ToInt16(array, 0);
        }
    }
}