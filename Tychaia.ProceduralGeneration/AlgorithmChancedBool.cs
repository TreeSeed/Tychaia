//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Chanced)]
    [FlowDesignerName("On / Off Values")]
    public class AlgorithmChancedBool : Algorithm<int, int>
    {
        [DataMember]
        [DefaultValue(true)]
        [Description("Whether to guarantee the maximum value at the global (0, 0) position.")]
        public bool GuaranteeStartingPoint
        {
            get;
            set;
        }

        [DataMember]
        [Description("The seed modifier value to apply to this layer.")]
        public long Modifier
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The Lower value selected.")]
        public int LowerValue
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(1)]
        [Description("The Higher value selected.")]
        public int HigherValue
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
        [DefaultValue(true)]
        [Description("Show this layer as 2D in the editor.")]
        public bool Layer2D
        {
            get;
            set;
        }

        public override string[] InputNames
        {
            get { return new string[] { "Input" }; }
        }

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }

        public AlgorithmChancedBool()
        {
            this.GuaranteeStartingPoint = true;
            this.Layer2D = true;
            this.ColorSet = ColorScheme.Land;
            this.LowerValue = 0;
            this.HigherValue = 1;
            this.Modifier = 435345;
        }

        // Will be able to use this algorithm for:
        // Land - This is the equivelent of InitialLand
        // Towns - This is the equivelent of InitialTowns
        // Landmarks - We can spread landmarks over the world, which we can then use values to determine the size/value of the landmarks.
        // Monsters - By utilising multiple value scaling we can either distribute individual monsters or monster groups or even monster villages.
        // Tresure chests - Spreading tresure chests in dungeons (can be used as an estimated location then moved slightly too).
        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            double chance = ((double)input[(i + ox) + (j + oy) * width + (k + oz) * width * height] / 100) * ((double)input[(i + ox) + (j + oy) * width + (k + oz) * width * height] / 100);

            if (this.GuaranteeStartingPoint && x == 0 && y == 0)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = this.HigherValue;
            else if (!Layer2D && AlgorithmUtility.GetRandomDouble(context.Seed, x, y, z, this.Modifier) > chance)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = this.HigherValue;
            else if (Layer2D && AlgorithmUtility.GetRandomDouble(context.Seed, x, y, 0, this.Modifier) > chance)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = this.HigherValue;
            else
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = this.LowerValue;
        }

        public enum ColorScheme
        {
            Land,
            Perlin,
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (this.ColorSet == ColorScheme.Perlin)
            {
                if (value == this.LowerValue)
                    return Color.Black;
                else
                    return Color.White;
            }
            else if (this.ColorSet == ColorScheme.Land)
            {
                if (value == this.LowerValue)
                    return Color.Blue;
                else
                    return Color.Green;
            }
            else
            {
                return Color.Gray;
            }
        }
    }
}

