using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Converts 2D information into a 3D landscape.
    /// </summary>
    [DataContract()]
    public class Layer3DFormTerrain : Layer3D
    {
        public Layer3DFormTerrain(Layer parent)
            : base(parent)
        {
        }

        public override int[] GenerateData(int x, int y, int z, int width, int height, int depth)
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height];

            int[] parent = this.Parents[0].GenerateData(x, y, width, height);
            int[] data = new int[width * height * depth];

            // Fill data with air.
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    for (int k = 0; k < depth; k++)
                        data[i + j * width + k * width * height] = -1;

            // Loop over the terrain and fill in the areas.
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if (parent[i + j * height] == 0)
                    {
                        // Ocean
                        data[i + j * width + 0 * width * height] = 0;
                    }
                    else
                    {
                        // Land
                        for (int k = 0; k < Math.Min(parent[i + j * height], depth); k++)
                            data[i + j * width + k * width * height] = parent[i + j * height];
                    }
                }

            return data;
        }

        public override Dictionary<int, System.Drawing.Brush> GetLayerColors()
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return null;
            else
                return this.Parents[0].GetLayerColors();
        }

        public override string ToString()
        {
            return "Form 3D Terrain";
        }

        public override int StandardDepth
        {
            get { return 16; }
        }

        public override bool[] GetParents3DRequired()
        {
            return new bool[] { false };
        }
    }
}
