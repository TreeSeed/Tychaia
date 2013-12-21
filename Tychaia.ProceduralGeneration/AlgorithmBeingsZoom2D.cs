// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using Tychaia.Data;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// "Zooms" in on the 1/4 center region of the parent layer, predicting the most likely
    /// values for the data that needs to be filled in (since resolution is being doubled).
    /// Only works in 2 dimensions.
    /// </summary>
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Beings)]
    [FlowDesignerName("Beings Zoom 2D")]
    public class AlgorithmBeingsZoom2D : Algorithm<Cell, Cell>
    {
        public AlgorithmBeingsZoom2D()
        {
            this.Mode = ZoomType.Spread;
        }

        [DataMember]
        [DefaultValue(ZoomType.Spread)]
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

        public override void ProcessCell(
            IRuntimeContext context,
            Cell[] input,
            Cell[] output,
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
                if (x % 2 == 0 && y % 2 == 0)
                    output[(i + ox) + ((j + oy) * width)] = current;
                else
                    output[(i + ox) + ((j + oy) * width)] = new Cell();
            }
            else if (this.Mode == ZoomType.Spread)
            {
                if (current.ClusterComplete && x % 2 == 0 && y % 2 == 0)
                    output[(i + ox) + ((j + oy) * width)] = current;
                else if (current.ClusterComplete)
                    output[(i + ox) + ((j + oy) * width)] = new Cell();
                else
                {
                    output[(i + ox) + ((j + oy) * width)] = new Cell();
                    output[(i + ox) + ((j + oy) * width)].ClusterLevel = current.ClusterLevel;
                    output[(i + ox) + ((j + oy) * width)].ClusterDefinitionAssetName = current.ClusterDefinitionAssetName;
                    var order = 0;
                    if (x % 2 == 0 && y % 2 == 0)
                        order = 0;
                    else if (x % 2 == 0 && (y % 2 == 1 || y % 2 == -1))
                        order = 1;
                    else if ((x % 2 == 1 || x % 2 == -1) && y % 2 == 0)
                        order = 2;
                    else
                        order = 3;

                    while (current.Count0 > 0)
                    {
                        if (order > 0)
                            order--;
                        else
                        {
                            output[(i + ox) + ((j + oy) * width)].Count0 += 1;
                            order = 3;
                        }
                    }

                    while (current.Count1 > 0)
                    {
                        if (order > 0)
                            order--;
                        else
                        {
                            output[(i + ox) + ((j + oy) * width)].Count1 += 1;
                            order = 3;
                        }
                    }

                    while (current.Count2 > 0)
                    {
                        if (order > 0)
                            order--;
                        else
                        {
                            output[(i + ox) + ((j + oy) * width)].Count2 += 1;
                            order = 3;
                        }
                    }

                    while (current.Count3 > 0)
                    {
                        if (order > 0)
                            order--;
                        else
                        {
                            output[(i + ox) + ((j + oy) * width)].Count3 += 1;
                            order = 3;
                        }
                    }

                    while (current.Count4 > 0)
                    {
                        if (order > 0)
                            order--;
                        else
                        {
                            output[(i + ox) + ((j + oy) * width)].Count4 += 1;
                            order = 3;
                        }
                    }

                    while (current.Count5 > 0)
                    {
                        if (order > 0)
                            order--;
                        else
                        {
                            output[(i + ox) + ((j + oy) * width)].Count5 += 1;
                            order = 3;
                        }
                    }

                    while (current.Count6 > 0)
                    {
                        if (order > 0)
                            order--;
                        else
                        {
                            output[(i + ox) + ((j + oy) * width)].Count6 += 1;
                            order = 3;
                        }
                    }

                    while (current.Count7 > 0)
                    {
                        if (order > 0)
                            order--;
                        else
                        {
                            output[(i + ox) + ((j + oy) * width)].Count7 += 1;
                            order = 3;
                        }
                    }

                    while (current.Count8 > 0)
                    {
                        if (order > 0)
                            order--;
                        else
                        {
                            output[(i + ox) + ((j + oy) * width)].Count8 += 1;
                            order = 3;
                        }
                    }

                    while (current.Count9 > 0)
                    {
                        if (order > 0)
                            order--;
                        else
                        {
                            output[(i + ox) + ((j + oy) * width)].Count9 += 1;
                            order = 3;
                        }
                    }

                    if (output[(i + ox) + ((j + oy) * width)].Count0 +
                        output[(i + ox) + ((j + oy) * width)].Count1 +
                        output[(i + ox) + ((j + oy) * width)].Count2 +
                        output[(i + ox) + ((j + oy) * width)].Count3 +
                        output[(i + ox) + ((j + oy) * width)].Count4 +
                        output[(i + ox) + ((j + oy) * width)].Count5 +
                        output[(i + ox) + ((j + oy) * width)].Count6 +
                        output[(i + ox) + ((j + oy) * width)].Count7 +
                        output[(i + ox) + ((j + oy) * width)].Count8 +
                        output[(i + ox) + ((j + oy) * width)].Count9 == 1)
                        output[(i + ox) + ((j + oy) * width)].ClusterComplete = true;
                    else 
                        output[(i + ox) + ((j + oy) * width)].ClusterComplete = false;
                }
            }
            else
            {
                output[(i + ox) + ((j + oy) * width)] = new Cell();
            }
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (this.Mode == ZoomType.Spread || this.Mode == ZoomType.Square)
                return this.DelegateColorForValueToParent(parent, value);
            else
                return Color.Red;
        }
    }
}
