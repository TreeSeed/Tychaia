using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Provides a smothing value for each cell.
    /// </summary>
    [DataContract]
    public class Layer3DEdgeDetection : Layer3D
    {
        public Layer3DEdgeDetection(Layer terrain)
            : base(terrain)
        {
        }

        protected override int[] GenerateDataImpl(long x, long y, long z, long width, long height, long depth)
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height * depth];

            int ox = 1;
            int oy = 1;
            int oz = 1;
            long rx = x - ox;
            long ry = y - oy;
            long rz = z - oz;
            long rw = width + ox * 2;
            long rh = height + oy * 2;
            long rd = depth + oz * 2;
            int[] parent = this.Parents[0].GenerateData(x - ox, y - oy, z - oz, rw, rh, rd);
            int[] data = new int[width * height * depth];

            // Populate with no blocks
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                    for (int k = 0; k < depth; ++k)
                        data[i + j * width + k * width * height] = -1;

            // Value = Height / Position
            // V     = Z   / X   / Y
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
            // Last value doesnt' matter - if last value is true then it is set to 0.

            // Write out the smoothing value.
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                    for (int k = 0; k < depth; ++k)
                    {
                        int smoothingvalue = 0;

                        // Check if block above is full - if so then set to 0 (normal block).
                        if (parent[(i + ox) + (j + oy) * rh + ((k + oz) + 1) * rw * rh] != -1)
                        {
                            data[i + j * width + k * width * height] = 0;
                        }
                        else
                        {
                            i += ox;
                            j += oy;
                            k += oz;
                            smoothingvalue += addcheck(parent, i, j, k, rh, rw, 1, 1, 1, 1);
                            smoothingvalue += addcheck(parent, i, j, k, rh, rw, 1, 1, 0, 2);
                            smoothingvalue += addcheck(parent, i, j, k, rh, rw, 1, 1, -1, 4);
                            smoothingvalue += addcheck(parent, i, j, k, rh, rw, 1, 0, -1, 8);
                            smoothingvalue += addcheck(parent, i, j, k, rh, rw, 1, -1, -1, 16);
                            smoothingvalue += addcheck(parent, i, j, k, rh, rw, 1, -1, 0, 32);
                            smoothingvalue += addcheck(parent, i, j, k, rh, rw, 1, -1, 1, 64);
                            smoothingvalue += addcheck(parent, i, j, k, rh, rw, 1, 0, 1, 128);
                            smoothingvalue += addcheck(parent, i, j, k, rh, rw, 0, 1, 1, 256);
                            smoothingvalue += addcheck(parent, i, j, k, rh, rw, 0, 1, 0, 512);
                            smoothingvalue += addcheck(parent, i, j, k, rh, rw, 0, 1, -1, 1024);
                            smoothingvalue += addcheck(parent, i, j, k, rh, rw, 0, 0, -1, 2048);
                            smoothingvalue += addcheck(parent, i, j, k, rh, rw, 0, -1, -1, 4096);
                            smoothingvalue += addcheck(parent, i, j, k, rh, rw, 0, -1, 0, 8192);
                            smoothingvalue += addcheck(parent, i, j, k, rh, rw, 0, -1, 1, 16384);
                            smoothingvalue += addcheck(parent, i, j, k, rh, rw, 0, 0, 1, 32768);
                            i -= ox;
                            j -= oy;
                            k -= oz;
                            data[i + j * width + k * width * height] = smoothingvalue;
                        }
                    }

            return data;
        }

        public int addcheck(int[] parent, long i, long j, long k, long rh, long rw, long zo, long xo, long yo, int score)
        {
            if (parent[(i - xo) + (j - yo) * rh + (k - zo) * rh * rw] != -1)
            { return score; }
            else
            { return 0; }
        }

        public override bool IsLayerColorsFlags()
        {
            return true;
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return null;
            else
            {
                Dictionary<int, LayerColor> result = new Dictionary<int, LayerColor>();
                // V     = Z   / X   / Y
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
                result.Add(0, LayerColor.Transparent);
                result.Add(1, LayerColor.FromArgb(63, 63, 63));
                result.Add(2, LayerColor.FromArgb(63, 63, 127));
                result.Add(4, LayerColor.FromArgb(63, 63, 191));
                result.Add(8, LayerColor.FromArgb(63, 127, 191));
                result.Add(16, LayerColor.FromArgb(63, 191, 191));
                result.Add(32, LayerColor.FromArgb(63, 191, 127));
                result.Add(64, LayerColor.FromArgb(63, 191, 63));
                result.Add(128, LayerColor.FromArgb(63, 127, 63));
                result.Add(256, LayerColor.FromArgb(127, 63, 63));
                result.Add(512, LayerColor.FromArgb(127, 63, 127));
                result.Add(1024, LayerColor.FromArgb(127, 63, 191));
                result.Add(2048, LayerColor.FromArgb(127, 127, 191));
                result.Add(4096, LayerColor.FromArgb(127, 191, 191));
                result.Add(8192, LayerColor.FromArgb(127, 191, 127));
                result.Add(16384, LayerColor.FromArgb(127, 191, 63));
                result.Add(32768, LayerColor.FromArgb(127, 127, 63));
                result.Add(65536, LayerColor.FromArgb(191, 127, 127));
                return result;
            }
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Terrain" };
        }

        public override string ToString()
        {
            return "Edge Detection";
        }

        public override bool[] GetParents3DRequired()
        {
            return new bool[] { true };
        }
    }
}
