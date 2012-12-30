using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// "Zooms" in on the 1/4 center region of the parent layer, predicting the most likely
    /// values for the data that needs to be filled in (since resolution is being doubled).
    /// </summary>
    [DataContract]
    [FlowDesignerCategory(FlowCategory.General)]
    [FlowDesignerName("Zoom Iterations")]
    public class AlgorithmZoom : Algorithm<int, int>
    {
        [DataMember]
        [DefaultValue(ZoomType.Smooth)]
        [Description("The zooming algorithm to use.")]
        public ZoomType Mode
        {
            get;
            set;
        }
        
        public override int RequiredXBorder { get { return 2; } }
        public override int RequiredYBorder { get { return 2; } }
        public override bool InputWidthAtHalfSize { get { return true; } }
        public override bool InputHeightAtHalfSize { get { return true; } }

        public AlgorithmZoom()
        {
            this.Mode = ZoomType.Smooth;
        }

        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth)
        {
            int ox = 2; // Offsets
            int oy = 2;
            long rw = width / 2 + ox * 2;

            bool xmod = x % 2 != 0;
            bool ymod = y % 2 != 0;
            int ocx = xmod ? (int)(i % 2) : 0;
            int ocy = ymod ? (int)(j % 2) : 0;
            int ocx_w = xmod ? (int)((i - 1) % 2) : 0;
            int ocx_e = xmod ? (int)((i + 1) % 2) : 0;
            int ocy_n = ymod ? (int)((j - 1) % 2) : 0;
            int ocy_s = ymod ? (int)((j + 1) % 2) : 0;
            int v_ocx = (xmod && i % 2 != 0 ? (i < 0 ? -1 : 1) : 0);
            int v_ocy = (ymod && j % 2 != 0 ? (j < 0 ? -1 : 1) : 0);
            int v_ocy_n = (ymod && (j - 1) % 2 != 0 ? ((j - 1) < 0 ? -1 : 1) : 0);
            int v_ocy_s = (ymod && (j + 1) % 2 != 0 ? ((j + 1) < 0 ? -1 : 1) : 0);
            int v_ocx_e = (xmod && (i + 1) % 2 != 0 ? ((i + 1) < 0 ? -1 : 1) : 0);
            int v_ocx_w = (xmod && (i - 1) % 2 != 0 ? ((i - 1) < 0 ? -1 : 1) : 0);
            if (ocx != v_ocx)
                throw new InvalidOperationException("ocx != v_ocx");
            if (ocy != v_ocy)
                throw new InvalidOperationException("ocy != v_ocy");
            if (ocx_w != v_ocx_w)
                throw new InvalidOperationException("ocx_w != v_ocx_w");
            if (ocx_e != v_ocx_e)
                throw new InvalidOperationException("ocx_e != v_ocx_e");
            if (ocy_n != v_ocy_n)
                throw new InvalidOperationException("ocy_n != v_ocy_n");
            if (ocy_s != v_ocy_s)
                throw new InvalidOperationException("ocy_s != v_ocy_s -- " + ocy_s + " != " + v_ocy_s + " -- j: " + j + " j+1: " + (j + 1));


            int current = input[(i / 2 + ox + ocx) + (j / 2 + oy + ocy) * rw];
            int north = input[(i / 2 + ox + ocx) + ((j - 1) / 2 + oy + ocy_n) * rw];
            int south = input[(i / 2 + ox + ocx) + ((j + 1) / 2 + oy + ocy_s) * rw];
            int east = input[((i + 1) / 2 + ox + ocx_e) + (j / 2 + oy + ocy) * rw];
            int west = input[((i - 1) / 2 + ox + ocx_w) + (j / 2 + oy + ocy) * rw];
            
            if (this.Mode == ZoomType.Smooth || this.Mode == ZoomType.Fuzzy)
                output[i + j * width] = context.Smooth(x + i, y + j, north, south, west, east, current, i, j, ox, oy, rw, input);
            else                    
                output[i + j * width] = current;
        }

        /// <summary>
        /// An enumeration defining the type of zoom to perform.
        /// </summary>
        public enum ZoomType
        {
            Square,
            Smooth,
            Fuzzy,
        }
    }
}
