using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Generates a layer containing heightmap information based on the
    /// input land area (0 and 1 value).
    /// </summary>
    [DataContract]
    public class LayerDeriveTerrain : Layer2D
    {
        [DataMember]
        [DefaultValue(true)]
        [Description("Whether diagonals to cells will be evaluated for their height.")]
        public bool CheckDiagonals
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(true)]
        [Description("The random chance that a cell will not be elevated when suitable for elevation.")]
        public double FailLimit
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(20)]
        [Description("The maximum height of the terrain in the result map.")]
        public int MaxTerrain
        {
            get;
            set;
        }

        public LayerDeriveTerrain(Layer parent, Layer biome)
            : base(new Layer[] { parent, biome })
        {
            this.CheckDiagonals = true;
            this.FailLimit = 1.0;
            this.MaxTerrain = 20;
        }

        protected override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            if (this.Parents.Length < 2 || this.Parents[0] == null || this.Parents[1] == null)
                return new int[width * height];

            int[] parent = this.Parents[0].GenerateData(x, y, width, height);
            int[] biome = this.Parents[1].GenerateData(x, y, width, height);
            int[] data = new int[width * height];

            // Copy 1-for-1 the water cells.
            for (long i = 0; i < width; i++)
                for (long j = 0; j < height; j++)
                    if (parent[i + j * width] == 0)
                        data[i + j * width] = 0;
                    else
                        data[i + j * width] = -1;


            // Evaluate terrain.
            int count = 0;
            int lookFor = 0;
            do
            {
                count = 0;
                for (long i = 0; i < width; i++)
                    for (long j = 0; j < height; j++)
                    {
                        if (data[i + j * width] != -1)
                            continue;

                        bool isSurrounded = this.IsCellSurrounded(data, i, j, width, height, lookFor);
                        if (isSurrounded)
                        {
                            if (this.GetRandomDouble(x + i, y + j, 0) > this.FailLimit && lookFor != 0)
                                data[i + j * width] = lookFor;
                            else
                                data[i + j * width] = (int)Math.Min(lookFor + 1, this.MaxTerrain);
                            count++;
                        }
                    }
                if (lookFor < this.MaxTerrain)
                    lookFor++;
            }
            while (count != 0);

            return data;
        }

        private bool IsCellSurrounded(int[] parent, long x, long y, long width, long height, int lookFor)
        {
            int top =           this.GetCellValue(parent, x    , y - 1, width, height);
            int left =          this.GetCellValue(parent, x - 1, y    , width, height);
            int right =         this.GetCellValue(parent, x + 1, y    , width, height);
            int bottom =        this.GetCellValue(parent, x    , y + 1, width, height);

            if (!this.CheckDiagonals)
                return (top == lookFor || left == lookFor ||
                        right == lookFor || bottom == lookFor);

            int topLeft =       this.GetCellValue(parent, x - 1, y - 1, width, height);
            int topRight =      this.GetCellValue(parent, x + 1, y - 1, width, height);
            int bottomLeft =    this.GetCellValue(parent, x - 1, y + 1, width, height);
            int bottomRight =   this.GetCellValue(parent, x + 1, y + 1, width, height);

            return (topLeft == lookFor || top == lookFor || topRight == lookFor ||
                    left == lookFor || right == lookFor ||
                    bottomLeft == lookFor || bottom == lookFor || bottomRight == lookFor);
        }

        private int GetCellValue(int[] parent, long x, long y, long width, long height)
        {
            if (x < 0 || x >= width ||
                y < 0 || y >= height)
                return -1;

            return parent[x + y * width];
        } 

        public override Dictionary<int, System.Drawing.Brush> GetLayerColors()
        {
            return LayerColors.GetTerrainBrushes(this.MaxTerrain);
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Land", "Biome" };
        }

        public override string ToString()
        {
            return "Derive Terrain";
        }
    }
}
