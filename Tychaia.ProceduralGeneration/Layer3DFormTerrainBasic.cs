using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Converts 2D information into a 3D landscape filled with stone and water.
    /// </summary>
    [DataContract()]
    public class Layer3DFormTerrainBasic : Layer3D
    {
        public Layer3DFormTerrainBasic(Layer parent)
            : base(parent)
        {
        }

        protected override int[] GenerateDataImpl(long x, long y, long z, long width, long height, long depth)
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height * depth];

            int[] parent = this.Parents[0].GenerateData(x, y, width, height);
            int[] data = new int[width * height * depth];

            // Fill data with air.
            for (long i = 0; i < width; i++)
                for (long j = 0; j < height; j++)
                    for (long k = 0; k < depth; k++)
                        data[i + j * width + k * width * height] = -1;

            // Loop over the terrain and fill in the areas.
            for (long i = 0; i < width; i++)
                for (long j = 0; j < height; j++)
                    for (long k = 0; k < depth; k++)
                    {
                        int terr = parent[i + j * height];
                        if (z + k <= 0)
                            data[i + j * width + k * width * height] = 0; // Water
                        if (z + k < terr)
                            data[i + j * width + k * width * height] = 1; // Stone
                    }

            return data;
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            return LayerColors.TerrainBrushes;
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Terrain" };
        }

        public override string ToString()
        {
            return "Form 3D Terrain Basic";
        }

        public override bool[] GetParents3DRequired()
        {
            return new bool[] { false, false };
        }
    }
}
