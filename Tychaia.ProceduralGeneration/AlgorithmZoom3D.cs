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
    /// <summary>
    /// "Zooms" in on the 1/8 center region of the parent layer, predicting the most likely
    /// values for the data that needs to be filled in (since resolution is being doubled).
    /// Works in 3 dimensions.
    /// </summary>
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Zooming)]
    [FlowDesignerName("Zoom 3D")]
    public class AlgorithmZoom3D : Algorithm<int, int>
    {
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
            get { return new[] { 2 }; }
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
            get { return new[] { true }; }
        }

        public override string[] InputNames
        {
            get { return new[] { "Input" }; }
        }

        public override bool[] InputIs2D
        {
            get { return new[] { false }; }
        }

        public override bool Is2DOnly
        {
            get { return false; }
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
            var ocx = ((x - Math.Abs(i)) % 2 == 0 ? 0 : Math.Abs(i % 2)) - (i % 2 == -1 ? 1 : 0);
            var ocy = ((y - Math.Abs(j)) % 2 == 0 ? 0 : Math.Abs(j % 2)) - (j % 2 == -1 ? 1 : 0);
            var ocz = ((z - Math.Abs(k)) % 2 == 0 ? 0 : Math.Abs(k % 2)) - (k % 2 == -1 ? 1 : 0);

            var current = input[(i / 2) + ox + ocx +
                (((j / 2) + oy + ocy) * width) +
                (((k / 2) + oz + ocz) * width * height)];

            output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] = current;
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
