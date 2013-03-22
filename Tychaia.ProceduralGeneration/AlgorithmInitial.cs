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
    [FlowDesignerCategory(FlowCategory.Initials)]
    [FlowDesignerName("Random Values")]
    public class AlgorithmInitial : Algorithm<int>
    {
        [DataMember]
        [DefaultValue(0)]
        [Description("The value between 0.0 and 1.0 above which the cell is selected.")]
        public double Limit
        {
            get;
            set;
        }
        
        [DataMember]
        [DefaultValue(true)]
        [Description("Whether to guarantee the maximum value at the global (0, 0) position.")]
        public bool GuaranteeStartingPoint
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(1)]
        [Description("The minimum value that a selected cell will be given.")]
        public int MinimumValue
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum value that a selected cell will be given.")]
        public int MaximumValue
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The maximum value that a selected cell will be given.")]
        public int Modifier
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

        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }
        
        public AlgorithmInitial()
        {
            this.Limit = 0;
            this.GuaranteeStartingPoint = true;
            this.MinimumValue = 1;
            this.MaximumValue = 100;
            this.Layer2D = true;
            this.Modifier = 0;
            this.ColorSet = ColorScheme.Land;
        }

        // Will be able to use this algorithm for:
        // Land - This is the equivelent of InitialLand
        // Towns - This is the equivelent of InitialTowns
        // Landmarks - We can spread landmarks over the world, which we can then use values to determine the size/value of the landmarks.
        // Monsters - By utilising multiple value scaling we can either distribute individual monsters or monster groups or even monster villages.
        // Tresure chests - Spreading tresure chests in dungeons (can be used as an estimated location then moved slightly too).
        public override void ProcessCell(IRuntimeContext context, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            if (this.GuaranteeStartingPoint && x == 0 && y == 0)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = this.MaximumValue;
            else if (!Layer2D && context.GetRandomDouble(x, y, z, context.Modifier) > this.Limit)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = context.GetRandomRange(x, y, z, this.MinimumValue, this.MaximumValue, context.Modifier + this.Modifier);
            else if (Layer2D && context.GetRandomDouble(x, y, 0, context.Modifier) > this.Limit)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = context.GetRandomRange(x, y, 0, this.MinimumValue, this.MaximumValue, context.Modifier + this.Modifier);
            else
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = 0;
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
                return Color.FromArgb((int)(255 * (value / 100f)), (int)(255 * (value / 100f)), (int)(255 * (value / 100f)));
            }
            else if (this.ColorSet == ColorScheme.Land)
            {
                if (value == 0)
                    return Color.Blue;
                else
                    return Color.FromArgb(0, (int)(255 * (value / 100f)), 0);
            }
            else
            {
                return Color.Gray;
            }
        }
    }
}

