using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Adds town size to the existing towns in the generator
    /// </summary>
    [DataContract()]
    public class LayerExtendTowns : Layer2D
    {
        [DataMember]
        [DefaultValue(1)]
        [Description("The number of zoom iterations to perform.")]
        public int Iterations
        {
            get;
            set;
        }

        public LayerExtendTowns(Layer parent, Layer land)
            : base(new Layer[] {parent, land})
        {
            this.Iterations = 1;
        }

        public int[] GenerateDataIterate(int iter, int x, int y, int width, int height)
        {
            int ox = 15;
            int oy = 15;
            int rw = width + ox * 2;
            int[] data = new int[width * height];
            double townsize = 0;
            int[] parent = null;

            if (iter == this.Iterations)
            {
                if (this.Parents.Length < 2 || this.Parents[0] == null || this.Parents[1] == null)
                    return new int[width * height];
                else
                    parent = this.Parents[0].GenerateData(x - ox, y - oy, rw, height + oy * 2);
            }
            else
                parent = this.GenerateDataIterate(iter + 1, x - ox, y -oy, rw, rw);

            for (int k = 0; k < width; k++)
                for (int l = 0; l < height; l++)
                {
                    data[k + l * width] = parent[(k + ox) + (l + oy) * rw]; 
                }

                for (int i = 0; i < rw; i++)
                    for (int j = 0; j < rw; j++)
                    {
                        if (parent[(i) + (j) * rw] != 0)
                        {
                            townsize = TownEngine.Towns[parent[(i) + (j) * rw]].TownSize;

                            double selectedchance = this.GetRandomLong(x + i, y + j, iter) % 100;
                            int selected = this.GetRandomRange(x + i, y + j, 4, iter);
                            double chance = selectedchance / 100;
                            if (chance >= townsize)
                            {
                                switch (selected)
                                {
                                    case 0:
                                        if ((i + 1) > ox && j > oy && j < (oy + width) && (i + 3) < (ox + width))
                                        {
                                            data[i - ox + 1 + (j - oy) * width] = parent[(i) + (j) * rw];
                                            data[i - ox + 2 + (j - oy) * width] = parent[(i) + (j) * rw];
                                            data[i - ox + 3 + (j - oy) * width] = parent[(i) + (j) * rw];
                                        }
                                        break;
                                    case 1:
                                        if ((i - 3) > ox && j > oy && j < (oy + width) && (i - 1) < (ox + width))
                                        {
                                            data[i - 1 - ox + (j - oy) * width] = parent[(i) + (j) * rw];
                                            data[i - 2 - ox + (j - oy) * width] = parent[(i) + (j) * rw];
                                            data[i - 3 - ox + (j - oy) * width] = parent[(i) + (j) * rw];
                                        }
                                        break;
                                    case 2:
                                        if (i > ox && (j + 1) > oy && (j + 3) < (oy + width) && i < (ox + width))
                                        {
                                            data[i - ox + (j + 1 - oy) * width] = parent[(i) + (j) * rw];
                                            data[i - ox + (j + 2 - oy) * width] = parent[(i) + (j) * rw];
                                            data[i - ox + (j + 3 - oy) * width] = parent[(i) + (j) * rw];
                                        }
                                        break;
                                    case 3:
                                        if (i > ox && (j - 3) > oy && (j - 1) < (oy + width) && i < (ox + width))
                                        {
                                            data[i - ox + (j - 1 - oy) * width] = parent[(i) + (j) * rw];
                                            data[i - ox + (j - 2 - oy) * width] = parent[(i) + (j) * rw];
                                            data[i - ox + (j - 3 - oy) * width] = parent[(i) + (j) * rw];
                                        }
                                        break;
                                }
                            }
                        }
                    }
            return data;
        }

        public override string[] GetParentsRequired()
        {
            return new string[]{"Towns", "Land"};
        }

        protected override int[] GenerateDataImpl(int x, int y, int width, int height)
        {
            if (this.Iterations > 0)
                return this.GenerateDataIterate(1, x, y, width, height);
            else if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height];
            else
                return this.Parents[0].GenerateData(x, y, width, height);
        }

        public override Dictionary<int, System.Drawing.Brush> GetLayerColors()
        {
            return TownEngine.GetTownBrushes();
        }

        public override string ToString()
        {
            return "Extend Towns";
        }
    }
}
