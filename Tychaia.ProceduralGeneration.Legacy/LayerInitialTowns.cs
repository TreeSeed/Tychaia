using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Generates a layer of initial procedural generation data where each cell
    /// indicates either landmass or ocean.
    /// </summary>
    [DataContract]
    [FlowDesignerCategory(FlowCategory.Towns)]
    [FlowDesignerName("Initial Towns")]
    public class LayerInitialTowns : Layer2D
    {
        [DataMember]
        [DefaultValue(0.9)]
        [Description("The value between 0.0 and 1.0 above which the cell is treated as a town.")]
        public double TownLimit
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(true)]
        [Description("Whether to guarantee a town at the global (0, 0) position.")]
        public bool GuaranteeStartingPoint
        {
            get;
            set;
        }

        [DataMember]
        [Description("The seed modifier value to apply to this town map.")]
        public long Modifier
        {
            get;
            set;
        }

        public LayerInitialTowns()
            : base()
        {
            // Set defaults.
            this.TownLimit = 0.9;
            this.GuaranteeStartingPoint = true;
            this.Modifier = new Random().Next();
        }

        protected override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            int[] data = new int[width * height];

            for (int a = 0; a < width; a++)
                for (int b = 0; b < height; b++)
                {
                    if (this.GetRandomDouble(x + a, y + b, 0, (int)this.Modifier) > this.TownLimit)
                        data[a + b * width] = 1;
                    else
                        data[a + b * width] = 0;
                }

            // Guarantee the player a starting point at 0, 0.
            if (this.GuaranteeStartingPoint)
            if (0 >= x && 0 >= y && 0 < x + width && 0 < y + height)
                data[(0 - x) + (0 - y) * width] = 1;

            return data;
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            return LayerColors.TownBrushes;
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { };
        }

        public override string ToString()
        {
            return "Initial Towns";
        }
    }
}
