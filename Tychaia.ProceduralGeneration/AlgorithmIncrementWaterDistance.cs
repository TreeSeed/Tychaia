using System.ComponentModel;
using System.Runtime.Serialization;
using System;
using System.Drawing;

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
        [DataMember]
        [DefaultValue(0)]
        [Description("The maximum height of the terrain in the result map defined by a binary shift (3 -> 4, 4 -> 8, 5 -> 16).")]
        public int MaxTerrainBinary
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(ColorScheme.Land)]
        [Description("The color scheme to use.")]
        public ColorScheme ColorSet
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(false)]
        [Description("Whether this is the first increment water distance in the series.")]
        public bool Initial
        {
            get;
            set;
        }

        // Keep offsets odd (otherwise it screws it up)
        // TODO: Fix offsets odd not giving the correct ocx/ocy values
        public override int[] RequiredXBorder { get { return new int[] {2}; } }
        public override int[] RequiredYBorder { get { return new int[] {2}; } }
        public override int[] RequiredZBorder { get { return new int[] {0}; } }
        public override bool[] InputWidthAtHalfSize { get { return new bool[] {false}; } }
        public override bool[] InputHeightAtHalfSize { get { return new bool[] {false}; } }
        public override bool[] InputDepthAtHalfSize { get { return new bool[] {false}; } }
        
        public AlgorithmIncrementWaterDistance()
        {
            this.MaxTerrainBinary = 0;
            this.ColorSet = ColorScheme.Land;
        }

        public override string[] InputNames
        {
            get { return new string[] { "Input" }; }
        }
        
        public override bool Is2DOnly
        {
            get { return true; }
        }
        
        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            int v00 = input[((i - 1) + ox) + ((j - 1) + oy) * width + (k + oz) * width * height];
            int v01 = input[((i - 1) + ox) + ((j + 0) + oy) * width + (k + oz) * width * height];
            int v02 = input[((i - 1) + ox) + ((j + 1) + oy) * width + (k + oz) * width * height];
            int v10 = input[((i + 0) + ox) + ((j - 1) + oy) * width + (k + oz) * width * height];
            int v11 = input[((i + 0) + ox) + ((j + 0) + oy) * width + (k + oz) * width * height];
            int v12 = input[((i + 0) + ox) + ((j + 1) + oy) * width + (k + oz) * width * height];
            int v20 = input[((i + 1) + ox) + ((j - 1) + oy) * width + (k + oz) * width * height];
            int v21 = input[((i + 1) + ox) + ((j + 0) + oy) * width + (k + oz) * width * height];
            int v22 = input[((i + 1) + ox) + ((j + 1) + oy) * width + (k + oz) * width * height];
            
            if (this.Initial)
            {
                int result;
                if (v11 > 0)
                    result = v11;
                else if (
                    (v00 > 0 ? 1 : 0 +
                    v01 > 0 ? 1 : 0 +
                    v02 > 0 ? 1 : 0 + 
                    v10 > 0 ? 1 : 0 + 
                    v12 > 0 ? 1 : 0 + 
                    v20 > 0 ? 1 : 0 + 
                    v21 > 0 ? 1 : 0 + 
                    v22 > 0 ? 1 : 0) > 0)
                    result = 0;
                else
                    result = v11;

                output[i + ox + (j + oy) * width + (k + oz) * width * height] = result;
            }
            else
            {
                int mod = 0;
                if ((v00 < v11 || v20 < v11 || v02 < v11 || v22 < v11) && v11 > 0)
                    mod = 1;
                else if (v11 < 0 && (v01 > v11 || v10 > v11 || v21 > v11 || v12 > v11))
                    mod = -1;

                output[i + ox + (j + oy) * width + (k + oz) * width * height] = v11 * 2 - mod;
            }
        }

        public enum ColorScheme
        {
            Land,
            Perlin,
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
    }
}
