using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Provides a smothing value for each cell.
    /// </summary>
    [DataContract]
    public class LayerEdgeDetection : Layer3D
    {
        public LayerEdgeDetection(Layer terrain)
            : base(terrain)
        {
        }

        public override int[] GenerateData(int x, int y, int z, int width, int height, int depth)
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height * depth];

            int[] parent = this.Parents[0].GenerateData(x, y, z, width, height, depth);
            int[] data = new int[width * height * depth];

            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                    for (int k = 0; k < depth; ++k)
                        data[i + j * width + k * width * height] = 0;

            // Value = Height / Position
            // 1     = Bot / Bot / Left
            // 2     = Bot / Bot / Mid
            // 4     = Bot / Bot / Right
            // 8     = Bot / Mid / Right
            // 16    = Bot / Top / Right
            // 32    = Bot / Top / Mid
            // 64    = Bot / Top / Left
            // 128   = Bot / Mid / Left
            // 256   = Mid / Bot / Left
            // 512   = Mid / Bot / Mid
            // 1024  = Mid / Bot / Right
            // 2048  = Mid / Mid / Right
            // 4096  = Mid / Top / Right
            // 8192  = Mid / Top / Mid
            // 16384 = Mid / Top / Left
            // 32768 = Mid / Mid / Left
            // 65536 = Top / Mid / Mid


            // Write out the smoothing value.
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                    for (int k = 0; k < depth; ++k)
                    {
                        int smoothingvalue = 0;
                    }

            return data;
        }

        public override Dictionary<int, System.Drawing.Brush> GetLayerColors()
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return null;
            else
            {
                Dictionary<int, System.Drawing.Brush> result = new Dictionary<int, System.Drawing.Brush>();
                result.Add(0, new System.Drawing.SolidBrush(Color.Transparent));
            }
                return BiomeEngine.GetSecondaryBiomeBrushes();
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Terrain" };
        }

        public override string ToString()
        {
            return "Edge Detection";
        }

        public override int StandardDepth
        {
            get { return 16; }
        }

        public override bool[] GetParents3DRequired()
        {
            return new bool[] { true };
        }
    }
}
