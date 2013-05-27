using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// A layer which erases town cells that exist over ocean or water.
    /// </summary>
    [DataContract]
    [FlowDesignerCategory(FlowCategory.Towns)]
    [FlowDesignerName("Erase Incorrect Towns")]
    public class LayerEraseTowns : Layer2D
    {
        public LayerEraseTowns(Layer towns, Layer land)
            : base(new Layer[] { towns, land })
        {
        }

        protected override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            if (this.Parents.Length < 2 || this.Parents[0] == null || this.Parents[1] == null)
                return new int[width * height];

            int[] towns = this.Parents[0].GenerateData(x, y, width, height);
            int[] land = this.Parents[1].GenerateData(x, y, width, height);
            int[] data = new int[width * height];

            // Erase any towns that exist over water.
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (land[i + j * width] == 0)
                        data[i + j * width] = 0;
                    else
                        data[i + j * width] = towns[i + j * width];

            // Also erase any cells that do not have at least 4 cells next to them.
            /*
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (!this.IsCellSurrounded(temp, i, j, width, height, towns[i + j * width]))
                        data[i + j * width] = 0;
                    else
                        data[i + j * width] = temp[i + j * width];
            */

            return data;
        }

        private bool IsCellSurrounded(int[] parent, int x, int y, int width, int height, int lookFor)
        {
            int top = this.GetCellValue(parent, x, y - 1, width, height);
            int left = this.GetCellValue(parent, x - 1, y, width, height);
            int right = this.GetCellValue(parent, x + 1, y, width, height);
            int bottom = this.GetCellValue(parent, x, y + 1, width, height);
            int topLeft = this.GetCellValue(parent, x - 1, y - 1, width, height);
            int topRight = this.GetCellValue(parent, x + 1, y - 1, width, height);
            int bottomLeft = this.GetCellValue(parent, x - 1, y + 1, width, height);
            int bottomRight = this.GetCellValue(parent, x + 1, y + 1, width, height);

            int count = 0;
            count += (topLeft == lookFor) ? 1 : 0;
            count += (top == lookFor) ? 1 : 0;
            count += (topRight == lookFor) ? 1 : 0;
            count += (left == lookFor) ? 1 : 0;
            count += (right == lookFor) ? 1 : 0;
            count += (bottomLeft == lookFor) ? 1 : 0;
            count += (bottom == lookFor) ? 1 : 0;
            count += (bottomRight == lookFor) ? 1 : 0;
            return (count > 3);
        }

        private int GetCellValue(int[] parent, int x, int y, int width, int height)
        {
            if (x < 0 || x >= width ||
                y < 0 || y >= height)
                return -1;

            return parent[x + y * width];
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return null;
            else
                return this.Parents[0].GetLayerColors();
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Towns", "Land" };
        }

        public override string ToString()
        {
            return "Erase Towns";
        }
    }
}
