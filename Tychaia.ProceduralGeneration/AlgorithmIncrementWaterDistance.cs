// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
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
    [FlowDesignerCategory(FlowCategory.Terrain)]
    [FlowDesignerName("Increment Water Distance")]
    public class AlgorithmIncrementWaterDistance : Algorithm<int, int>
    {
        public AlgorithmIncrementWaterDistance()
        {
            this.MaxTerrainBinary = 0;
            this.ColorSet = ColorScheme.Land;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description(
            "The maximum height of the terrain in the result map defined by a binary shift (3 -> 4, 4 -> 8, 5 -> 16).")]
        public int MaxTerrainBinary { get; set; }

        [DataMember]
        [DefaultValue(ColorScheme.Land)]
        [Description("The color scheme to use.")]
        public ColorScheme ColorSet { get; set; }

        [DataMember]
        [DefaultValue(false)]
        [Description("Whether this is the first increment water distance in the series.")]
        public bool Initial { get; set; }

        // Keep offsets odd (otherwise it screws it up)
        // TODO: Fix offsets odd not giving the correct ocx/ocy values
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
            get { return new[] { false }; }
        }

        public override bool[] InputHeightAtHalfSize
        {
            get { return new[] { false }; }
        }

        public override bool[] InputDepthAtHalfSize
        {
            get { return new[] { false }; }
        }

        public override string[] InputNames
        {
            get { return new[] { "Input" }; }
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
            var v00 = input[((i - 1) + ox) + (((j - 1) + oy) * width)];
            var v01 = input[((i - 1) + ox) + (((j + 0) + oy) * width)];
            var v02 = input[((i - 1) + ox) + (((j + 1) + oy) * width)];
            var v10 = input[((i + 0) + ox) + (((j - 1) + oy) * width)];
            var v11 = input[((i + 0) + ox) + (((j + 0) + oy) * width)];
            var v12 = input[((i + 0) + ox) + (((j + 1) + oy) * width)];
            var v20 = input[((i + 1) + ox) + (((j - 1) + oy) * width)];
            var v21 = input[((i + 1) + ox) + (((j + 0) + oy) * width)];
            var v22 = input[((i + 1) + ox) + (((j + 1) + oy) * width)];

            if (this.Initial)
            {
                output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] = v11;
            }
            else
            {
                var mod = 0;
                if ((v00 < v11 || v20 < v11 || v02 < v11 || v22 < v11) && v11 > 0)
                    mod = 1;
                else if (v11 < 0 && (v20 > v11 || v02 > v11 || v22 > v11 || v00 > v11))
                    mod = -1;

                output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] = (v11 * 2) - mod;
            }
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (this.Initial)
            {
                if (value == -1)
                    return Color.Orange;
                if (value == 0)
                    return Color.Yellow;
                if (value == 1)
                    return Color.Green;
                return Color.Red;
            }

            if (this.MaxTerrainBinary < 0)
                this.MaxTerrainBinary = 0;
            var maxValue = 1 << this.MaxTerrainBinary;
            var minValue = -(1 << this.MaxTerrainBinary);
            int a;
            if (value < 0)
                a = 215 - (int)(value / (double)minValue * 180);
            else
                a = 64 + (int)(value / (double)maxValue * 127);
            if (a < 0 || a > 255)
                return Color.Red;

            return Color.FromArgb(0, value < 0 ? 0 : a, value < 0 ? a : 0);
        }

        public enum ColorScheme
        {
            Land,
            Perlin,
        }
    }
}
