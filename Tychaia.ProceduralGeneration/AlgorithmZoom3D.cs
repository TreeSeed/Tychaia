//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

using System.ComponentModel;
using System.Runtime.Serialization;
using System;

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
        public override int[] RequiredXBorder { get { return new int[] {2}; } }
        public override int[] RequiredYBorder { get { return new int[] {2}; } }
        public override int[] RequiredZBorder { get { return new int[] {2}; } }
        public override bool[] InputWidthAtHalfSize { get { return new bool[] {true}; } }
        public override bool[] InputHeightAtHalfSize { get { return new bool[] {true}; } }
        public override bool[] InputDepthAtHalfSize { get { return new bool[] {true}; } }

        public AlgorithmZoom3D()
        {
        }

        public override string[] InputNames
        {
            get { return new string[] { "Input" }; }
        }

        public override bool Is2DOnly
        {
            get { return false; }
        }

        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            int ocx = ((x - Math.Abs(i)) % 2 == 0 ? 0 : Math.Abs(i % 2)) - (i % 2 == -1 ? 1 : 0);
            int ocy = ((y - Math.Abs(j)) % 2 == 0 ? 0 : Math.Abs(j % 2)) - (j % 2 == -1 ? 1 : 0);
            int ocz = ((z - Math.Abs(k)) % 2 == 0 ? 0 : Math.Abs(k % 2)) - (k % 2 == -1 ? 1 : 0);

            int current = input[
                                (i / 2) + ox + ocx +
                ((j / 2) + oy + ocy) * width +
                ((k / 2) + oz + ocz) * width * height];

            output[i + ox + (j + oy) * width + (k + oz) * width * height] = current;
        }

        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
