using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    public static class AlgorithmColors
    {
        public static Dictionary<int, Color> ContinentBrushes = new Dictionary<int, Color>
        {
            { 0  /* water   */, Color.FromArgb(0,0,255) },
            { 1  /* grass   */, Color.FromArgb(0,255,0) }
        };

        public static Dictionary<int, Color> VoronoiBrushes = new Dictionary<int, Color>
        {
            { 0  /* none     */, Color.FromArgb(63, 63, 63) },
            { 1  /* original */, Color.FromArgb(255, 0, 0) },
            { 2  /* vertex   */, Color.FromArgb(0, 255, 0) },
            { 3  /* edge     */, Color.FromArgb(0, 0, 255) },
        };

        public static Dictionary<int, Color> Voronoi3DBrushes = new Dictionary<int, Color>
        {
            { 0  /* none     */, Color.FromArgb(0, 63, 63, 63) },
            { 1  /* original */, Color.FromArgb(15, 255, 0, 0) },
            { 2  /* vertex   */, Color.FromArgb(15, 0, 255, 0) },
            { 3  /* edge     */, Color.FromArgb(15, 0, 0, 255) },
        };

        public static Dictionary<int, Color> TownBrushes = new Dictionary<int, Color>
        {
            { 0  /* none     */, Color.FromArgb(0, 0, 0) },
            { 1  /* active   */, Color.FromArgb(127, 0, 0) },
            { 2  /* ruins    */, Color.FromArgb(127, 63, 63) },
        };

        public static Dictionary<int, Color> TerrainBrushes = new Dictionary<int, Color>
        {
            { 0  /* water   */, Color.FromArgb(2, 0, 0, 255) },
            { 1  /* stone   */, Color.FromArgb(2, 127, 127, 127) }
        };

        public static Dictionary<int, Color> TreeBrushes = new Dictionary<int, Color>
        {
            { 0  /* no tree */, Color.FromArgb(0, 0, 0) },
            { 1  /* tree    */, Color.FromArgb(255, 0, 0) }
        };

        public static Dictionary<int, Color> GetTerrainBrushes(int maxTerrain)
        {
            return AlgorithmColors.GetGradientBrushesWater(-maxTerrain, maxTerrain);
        }

        /// <summary>
        /// Returns a list of brushes used as a gradient over between the minValue
        /// and maxValue parameters.
        /// </summary>
        /// <param name="minValue">The minimum value in the integer field.</param>
        /// <param name="maxValue">The maximum value in the integer field.</param>
        /// <returns></returns>
        public static Dictionary<int, Color> GetGradientBrushes(int minValue, int maxValue)
        {
            var brushes = new Dictionary<int, Color>();
            for (int i = 0; i < maxValue - minValue; i++)
            {
                var a = (int)(256 * (i / (double)(maxValue - minValue)));
                brushes.Add(i + minValue, Color.FromArgb(a, a, a));
            }
            return brushes;
        }

        /// <summary>
        /// Returns a list of brushes used as a gradient over between the minValue
        /// and maxValue parameters showing negative values as water.
        /// </summary>
        /// <param name="minValue">The minimum value in the integer field.</param>
        /// <param name="maxValue">The maximum value in the integer field.</param>
        /// <returns></returns>
        public static Dictionary<int, Color> GetGradientBrushesWater(int minValue, int maxValue)
        {
            var brushes = new Dictionary<int, Color>();
            for (int i = 0; i < maxValue - minValue; i++)
            {
                var a = (int)(256 * (i / (double)(maxValue - minValue)));
                brushes.Add(i + minValue, Color.FromArgb(i + minValue < 0 ? 0 : a, i + minValue < 0 ? 0 : a, a));
            }
            return brushes;
        }
    }
}
