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
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Terrain)]
    [FlowDesignerName("Terrain Extender")]
    public class AlgorithmTerrainExtender : Algorithm<int, int>
    {
        public AlgorithmTerrainExtender()
        {
            this.Limit = 0.8;
            this.Layer2D = true;
            this.ColorSet = ColorScheme.Land;
        }

        [DataMember]
        [DefaultValue(0.8)]
        [Description("The value between 0.0 and 1.0 above which the cell is selected.")]
        public double Limit { get; set; }

        [DataMember]
        [DefaultValue(ColorScheme.Land)]
        [Description("The color scheme to use.")]
        public ColorScheme ColorSet { get; set; }

        [DataMember]
        [DefaultValue(true)]
        [Description("Show this layer as 2D in the editor.")]
        public bool Layer2D { get; set; }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public override bool[] InputIs2D
        {
            get
            {
                return new[] { this.Layer2D };
            }
        }

        public override string[] InputNames
        {
            get
            {
                return new[] { "Terrain" };
            }
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
            if (!this.Layer2D &&
                     AlgorithmUtility.GetRandomDouble(context.Seed, x, y, z, context.Modifier) > this.Limit)
                output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] = (input[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] == -1 ? 1 : input[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] + 1);
            else if (this.Layer2D &&
                     AlgorithmUtility.GetRandomDouble(context.Seed, x, y, 0, context.Modifier) > this.Limit)
                output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] = (input[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] == -1 ? 1 : input[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] + 1);
            else
                output[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)] = input[(i + ox) + ((j + oy) * width) + ((k + oz) * width * height)];
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }

        public enum ColorScheme
        {
            Land,
            Perlin,
        }
    }
}
