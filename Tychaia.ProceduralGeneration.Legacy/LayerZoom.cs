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
    public class LayerZoom : Layer2D
    {
        [DataMember]
        [DefaultValue(ZoomType.Smooth)]
        [Description("The zooming algorithm to use.")]
        public ZoomType Mode
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(1)]
        [Description("The number of zoom iterations to perform.")]
        public int Iterations
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(1)]
        [Description("The number of zoom iterations to perform.")]
        public int Offset
        {
            get;
            set;
        }

        public LayerZoom(Layer parent)
            : base(parent)
        {
            this.Mode = ZoomType.Smooth;
            this.Iterations = 1;
        }

        private int[] GenerateDataIterate(int iter, long x, long y, long width, long height)
        {
            int ox = 2; // Offsets
            int oy = 2;
            long rw = width / 2 + ox * 2;
            long rh = height / 2 + oy * 2;
            long rx = (x < 0 ? (x - 1) / 2 : x / 2) - ox; // Location in the parent
            long ry = (y < 0 ? (y - 1) / 2 : y / 2) - oy;
            // For smoothing to work, we need to know the cells that are actually
            // beyond the edge of the center.
            int[] parent = null;
            if (iter == this.Iterations)
            {
                if (this.Parents.Length < 1 || this.Parents[0] == null)
                    parent = new int[width * height];
                else
                    parent = this.Parents[0].GenerateData(rx, ry, rw, rh);
            }
            else
                parent = this.GenerateDataIterate(iter + 1, rx, ry, rw, rh);
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
                    int north = this.FindZoomedPoint(parent, i, j - 1, ox, oy, x, y, rw);
                    int south = this.FindZoomedPoint(parent, i, j + 1, ox, oy, x, y, rw);
                    int east = this.FindZoomedPoint(parent, i + 1, j, ox, oy, x, y, rw);
                    int west = this.FindZoomedPoint(parent, i - 1, j, ox, oy, x, y, rw);

                    if (this.Mode == ZoomType.Smooth || this.Mode == ZoomType.Fuzzy)
                        data[i + j * width] = this.Smooth(x + i, y + j, north, south, west, east, current, i, j, ox, oy, rw, parent);
                    else
                        data[i + j * width] = current;
                }

            return data;
        }

        private int FindZoomedPoint(int[] parent, long i, long j, long ox, long oy, long x, long y, long rw)
        {
            //int ocx = (x % 2 != 0 && i % 2 != 0 ? (i < 0 ? -1 : 1) : 0);
            //int ocy = (y % 2 != 0 && j % 2 != 0 ? (j < 0 ? -1 : 1) : 0);
            //int ocx = (x % 2 != 0 && i % 2 != 0 ? (int)(i % 2) : 0);
            //int ocy = (y % 2 != 0 && j % 2 != 0 ? (int)(j % 2) : 0);
            int ocx = (x % 2 != 0) ? (int)(i % 2) : 0;
            int ocy = (y % 2 != 0) ? (int)(j % 2) : 0;

            return parent[(i / 2 + ox + ocx) + (j / 2 + oy + ocy) * rw];
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return null;
            else
                return this.Parents[0].GetLayerColors();
        }

        protected override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            if (this.Iterations > 0)
                return this.GenerateDataIterate(1, x, y, width, height);
            else if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height];
            else
                return this.Parents[0].GenerateData(x, y, width, height);
        }

        private int Smooth(long x, long y, int northValue, int southValue, int westValue, int eastValue, int currentValue, long i, long j, long ox, long oy, long rw, int[] parent)
        {
            // Parent-based Smoothing
            int selected = 0;

            if (x % 2 == 0)
            {
                if (y % 2 == 0)
                {
                    return currentValue;
                }
                else
                {
                    selected = this.GetRandomRange(x, y, 0, 2);
                    switch (selected)
                    {
                        case 0:
                            return currentValue;
                        case 1:
                            return southValue;
                    }
                }
            }
            else
            {
                if (y % 2 == 0)
                {
                    selected = this.GetRandomRange(x, y, 0, 2);
                    switch (selected)
                    {
                        case 0:
                            return currentValue;
                        case 1:
                            return eastValue;
                    }
                }
                else
                {
                    if (this.Mode == ZoomType.Smooth)
                    {
                        selected = this.GetRandomRange(x, y, 0, 3);
                        switch (selected)
                        {
                            case 0:
                                return currentValue;
                            case 1:
                                return southValue;
                            case 2:
                                return eastValue;
                        }
                    }
                    else
                    {
                        selected = this.GetRandomRange(x, y, 0, 4);
                        switch (selected)
                        {
                            case 0:
                                return currentValue;
                            case 1:
                                return southValue;
                            case 2:
                                return eastValue;
                            case 3:
                                return this.FindZoomedPoint(parent, i + 2, j + 2, ox, oy, x - i, y - j, rw);
                        }
                    }
                }
            }

            // Select one of the four options if we couldn't otherwise
            // determine a value.
            selected = this.GetRandomRange(x, y, 0, 4);

            switch (selected)
            {
                case 0:
                    return northValue;
                case 1:
                    return southValue;
                case 2:
                    return eastValue;
                case 3:
                    return westValue;
            }

            throw new InvalidOperationException();
        }

        public override string ToString()
        {
            return "Zoom Iterations";
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
