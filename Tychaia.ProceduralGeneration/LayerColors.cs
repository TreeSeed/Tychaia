using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    public static class LayerColors
    {
        public static Dictionary<int, Brush> ContinentBrushes = new Dictionary<int, Brush>
        {
            { 0  /* water   */, new SolidBrush(Color.FromArgb(0,0,255)) },
            { 1  /* grass   */, new SolidBrush(Color.FromArgb(0,255,0)) }
        };

        public static Dictionary<int, Brush> BiomeBrushes = new Dictionary<int, Brush>
        {
            { BiomeEngine.BIOME_OCEAN, new SolidBrush(Color.FromArgb(0,0,255)) },
            { BiomeEngine.BIOME_PLAINS, new SolidBrush(Color.FromArgb(0,255,0)) },
            { BiomeEngine.BIOME_DESERT, new SolidBrush(Color.FromArgb(255,255,0)) },
            { BiomeEngine.BIOME_FOREST, new SolidBrush(Color.FromArgb(0,127,0)) },
            { BiomeEngine.BIOME_SNOW, new SolidBrush(Color.FromArgb(255,255,255)) }
        };

        public static Dictionary<int, Brush> VoronoiBrushes = new Dictionary<int, Brush>
        {
            { 0  /* none     */, new SolidBrush(Color.FromArgb(63, 63, 63)) },
            { 1  /* original */, new SolidBrush(Color.FromArgb(255, 0, 0)) },
            { 2  /* vertex   */, new SolidBrush(Color.FromArgb(0, 255, 0)) },
            { 3  /* edge     */, new SolidBrush(Color.FromArgb(0, 0, 255)) },
        };

        public static Dictionary<int, Brush> TownBrushes = new Dictionary<int, Brush>
        {
            { 0  /* none     */, new SolidBrush(Color.FromArgb(0, 0, 0)) },
            { 1  /* active   */, new SolidBrush(Color.FromArgb(127, 0, 0)) },
            { 2  /* ruins    */, new SolidBrush(Color.FromArgb(127, 63, 63)) },
        };

        public static Dictionary<int, Brush> GetTerrainBrushes(int maxTerrain)
        {
            Dictionary<int, Brush> v = LayerColors.GetGradientBrushes(1, maxTerrain);
            v.Add(0, new SolidBrush(Color.FromArgb(0, 0, 255)));
            return v;
        }

        /// <summary>
        /// Returns a list of brushes used as a gradient over between the minValue
        /// and maxValue parameters.
        /// </summary>
        /// <param name="minValue">The minimum value in the integer field.</param>
        /// <param name="maxValue">The maximum value in the integer field.</param>
        /// <returns></returns>
        public static Dictionary<int, Brush> GetGradientBrushes(int minValue, int maxValue)
        {
            Dictionary<int, Brush> brushes = new Dictionary<int, Brush>();
            for (int i = 0; i < maxValue - minValue; i++)
            {
                int a = (int)(256 * (i / (double)(maxValue - minValue)));
                brushes.Add(i + minValue, new SolidBrush(Color.FromArgb(a, a, a)));
            }
            return brushes;
        }
    }
}
