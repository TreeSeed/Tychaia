// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// "Zooms" in on the 1/4 center region of the parent layer, predicting the most likely
    /// values for the data that needs to be filled in (since resolution is being doubled).
    /// Only works in 2 dimensions.
    /// </summary>
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Zooming)]
    [FlowDesignerName("Zoom 2D")]
    public class AlgorithmZoom2D : Algorithm<int, int>
    {
        public AlgorithmZoom2D()
        {
            this.Mode = ZoomType.Smooth;
        }

        [DataMember]
        [DefaultValue(ZoomType.Smooth)]
        [Description("The zooming algorithm to use.")]
        public ZoomType Mode { get; set; }

        public override int[] RequiredXBorder
        {
            get { return new[] { 2 }; }
        }

        public override int[] RequiredYBorder
        {
            get { return new[] { 2 }; }
        }

        public override int[] RequiredZBorder
        {
            get { return new[] { 0 }; }
        }

        public override bool[] InputWidthAtHalfSize
        {
            get { return new[] { true }; }
        }

        public override bool[] InputHeightAtHalfSize
        {
            get { return new[] { true }; }
        }

        public override bool[] InputDepthAtHalfSize
        {
            get { return new[] { false }; }
        }

        public override string[] InputNames
        {
            get { return new[] { "Input" }; }
        }

        public override bool[] InputIs2D
        {
            get { return new[] { true }; }
        }

        public override bool Is2DOnly
        {
            get { return true; }
        }

// add modifier so that it won't constantly shift the same thing?
// only a temporary solution?
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
            var ocx = (x - i) % 2 == 0 ? (i % 2 == -1 ? -1 : 0) : (i % 2 == 1 ? 1 : 0);
            var ocy = (y - j) % 2 == 0 ? (j % 2 == -1 ? -1 : 0) : (j % 2 == 1 ? 1 : 0);

            var current = input[(i / 2) + ox + ocx +
                (((j / 2) + oy + ocy) * width)];

            if (this.Mode == ZoomType.Square)
            {
                output[(i + ox) + ((j + oy) * width)] = current;
            }
            else if (this.Mode == ZoomType.Spread)
            {
                var remainder = current % 2;
                int selected = AlgorithmUtility.GetRandomRange(context.Seed, x, y, 0, 4);
                var selected2 = AlgorithmUtility.GetRandomRange(context.Seed, x, y, 0, 4);
                
                var ymod = y % 2 == 0;
                var xmod = x % 2 == 0;
                
                var ocx_e = (x - i) % 2 == 0 ? ((i + 1) % 2 == -1 ? -1 : 0) : ((i + 1) % 2 == 1 ? 1 : 0);
                var east =
                    input[(((i + 1) / 2) + ox + ocx_e) + (((j / 2) + oy + ocy) * width)];

                var ocy_s = (y - j) % 2 == 0 ? ((j + 1) % 2 == -1 ? -1 : 0) : ((j + 1) % 2 == 1 ? 1 : 0);

                switch (selected2)
                {
                    case 0:
                        output[(i + ox) + ((j + oy) * width)] = (selected < remainder ? remainder : 0) + (current / 2);
                        break;
                    case 1:
                        var south = input[((i / 2) + ox + ocx) + ((((j + 1) / 2) + oy + ocy_s) * width)];

                        if (xmod)
                        {
                            remainder = (south + current) % 2;
                            output[(i + ox) + ((j + oy) * width)] = (selected < remainder ? remainder : 0) + ((south + current) / 2);
                        }
                        else if (ymod)
                        {
                            remainder = (east + current) % 2;
                            output[(i + ox) + ((j + oy) * width)] = (selected < remainder ? remainder : 0) + ((east + current) / 2);
                        }
                        else
                        {
                            remainder = (south + current) % 2;
                            output[(i + ox) + ((j + oy) * width)] = (selected < remainder ? remainder : 0) + ((south + current) / 2);
                        }
                        
                        break;
                    case 2:
                        remainder = (east + current) % 2;
                        output[(i + ox) + ((j + oy) * width)] = (selected < remainder ? remainder : 0) + ((east + current) / 2);
                        break;
                    case 3:
                        var southEast = input[(((i + 1) / 2) + ox + ocx_e) + ((((j + 1) / 2) + oy + ocy_s) * width)];

                        remainder = (southEast + current) % 2;
                        output[(i + ox) + ((j + oy) * width)] = (selected < remainder ? remainder : 0) + ((southEast + current) / 2);
                        break;
                }
            }
            else if (this.Mode == ZoomType.SmoothSpread)
            {
                var remainder = current % 2;
                var selected = AlgorithmUtility.GetRandomRange(context.Seed, x, y, 0, 4);
                
                var ymod = y % 2 == 0;
                var xmod = x % 2 == 0;
                
                var ocx_e = (x - i) % 2 == 0 ? ((i + 1) % 2 == -1 ? -1 : 0) : ((i + 1) % 2 == 1 ? 1 : 0);
                var east = input[(((i + 1) / 2) + ox + ocx_e) + (((j / 2) + oy + ocy) * width)];
                var ocy_s = (y - j) % 2 == 0 ? ((j + 1) % 2 == -1 ? -1 : 0) : ((j + 1) % 2 == 1 ? 1 : 0);
                var south = input[((i / 2) + ox + ocx) + ((((j + 1) / 2) + oy + ocy_s) * width)];

                if (!xmod && !ymod)
                {
                    var southEast = input[(((i + 1) / 2) + ox + ocx_e) + ((((j + 1) / 2) + oy + ocy_s) * width)];
                    remainder = (southEast + current) % 2;
                    output[(i + ox) + ((j + oy) * width)] = (selected < remainder ? remainder : 0) + ((southEast + current) / 2);
                } 
                else if (xmod)
                {
                    remainder = (south + current) % 2;
                    output[(i + ox) + ((j + oy) * width)] = (selected < remainder ? remainder : 0) + ((south + current) / 2);
                } 
                else if (ymod)
                {
                    remainder = (east + current) % 2;
                    output[(i + ox) + ((j + oy) * width)] = (selected < remainder ? remainder : 0) + ((east + current) / 2);
                } 
                else
                {
                    remainder = current % 2;
                    output[(i + ox) + ((j + oy) * width)] = selected < remainder ? remainder : 0 + (current / 2);
                }
            }
            else
            {
                int selected;

                var ymod = y % 2 == 0;
                var xmod = x % 2 == 0;

                if (!xmod && !ymod)
                    if (this.Mode == ZoomType.Fuzzy)
                        selected = AlgorithmUtility.GetRandomRange(context.Seed, x, y, 0, 4);
                    else
                        selected = AlgorithmUtility.GetRandomRange(context.Seed, x, y, 0, 3);
                else if (xmod && ymod)
                    selected = 4;
                else
                    selected = AlgorithmUtility.GetRandomRange(context.Seed, x, y, 0, 2);

                var ocx_e = (x - i) % 2 == 0 ? ((i + 1) % 2 == -1 ? -1 : 0) : ((i + 1) % 2 == 1 ? 1 : 0);
                var east =
                    input[(((i + 1) / 2) + ox + ocx_e) + (((j / 2) + oy + ocy) * width)];

                var ocy_s = (y - j) % 2 == 0 ? ((j + 1) % 2 == -1 ? -1 : 0) : ((j + 1) % 2 == 1 ? 1 : 0);

                switch (selected)
                {
                    case 0:
                        output[(i + ox) + ((j + oy) * width)] = current;
                        break;
                    case 1:
                        var south = input[((i / 2) + ox + ocx) + ((((j + 1) / 2) + oy + ocy_s) * width)];

                        if (xmod)
                            output[(i + ox) + ((j + oy) * width)] = south;
                        else if (ymod)
                            output[(i + ox) + ((j + oy) * width)] = east;
                        else
                            output[(i + ox) + ((j + oy) * width)] = south;
                        break;
                    case 2:
                        output[(i + ox) + ((j + oy) * width)] = east;
                        break;
                    case 3:
                        var southEast = input[(((i + 1) / 2) + ox + ocx_e) + ((((j + 1) / 2) + oy + ocy_s) * width)];

                        output[(i + ox) + ((j + oy) * width)] = southEast;
                        break;
                    case 4:
                        output[(i + ox) + ((j + oy) * width)] = current;
                        break;
                }
            }
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
