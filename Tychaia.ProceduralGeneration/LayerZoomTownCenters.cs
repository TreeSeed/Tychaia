using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Implements towns by expanding them in size.
    /// </summary>
    [DataContract()]
    [FlowDesignerCategory(FlowCategory.Towns)]
    [FlowDesignerName("Zoom Town Centers")]
    public class LayerZoomTownCenters : Layer2D
    {
        [DataMember]
        [DefaultValue(false)]
        [Description("Whether to encase the center of the town with values.")]
        public bool SurroundCenter
        {
            get;
            set;
        }

        public LayerZoomTownCenters(Layer parent)
            : base(parent)
        {
            this.SurroundCenter = false;
        }

        protected override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height];

            long ox = 2; // Offsets
            long oy = 2;
            long rw = width / 2 + ox * 2;
            long rh = height / 2 + oy * 2;
            long rx = (x < 0 ? (x - 1) / 2 : x / 2) - ox; // Location in the parent
            long ry = (y < 0 ? (y - 1) / 2 : y / 2) - oy;
            int[] parent = this.Parents[0].GenerateData(rx, ry, rw, rh);
            int[] data = new int[width * height];

            for (int i = 0; i < width; i++) // i = x in zoomed context
                for (int j = 0; j < height; j++) // j = y in zoomed context
                {
                    /*
                     * x = 0 i = 0 v = x[0]
                     * x = 0 i = 1 v = x[0]
                     * x = 0 i = 2 v = x[1]
                     * x = 0 i = 3 v = x[1]
                     * 
                     * x = 1 i = 0 v = x[0]
                     * x = 1 i = 1 v = x[1]
                     * x = 1 i = 2 v = x[1]
                     * x = 1 i = 3 v = x[2]
                     */

                    int current = this.FindZoomedPoint(parent, i, j, ox, oy, x, y, rw);

                    if (i % 2 - Math.Abs(x) % 2 == 0 && j % 2 - Math.Abs(y) % 2 == 0)
                        data[i + j * width] = current;
                }

            return data;
        }

        private int FindZoomedPoint(int[] parent, long i, long j, long ox, long oy, long x, long y, long rw)
        {
            int ocx = (x % 2 != 0 && i % 2 != 0 ? (i < 0 ? -1 : 1) : 0);
            int ocy = (y % 2 != 0 && j % 2 != 0 ? (j < 0 ? -1 : 1) : 0);

            return parent[(i / 2 + ox + ocx) + (j / 2 + oy + ocy) * rw];
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return null;
            else
                return this.Parents[0].GetLayerColors();
        }

        public override string ToString()
        {
            return "Implement Towns";
        }
    }
}
