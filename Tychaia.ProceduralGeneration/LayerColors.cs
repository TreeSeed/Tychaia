using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration
{
    public static class LayerColors
    {
        public static Dictionary<int, LayerColor> ContinentBrushes = new Dictionary<int, LayerColor>
        {
            { 0  /* water   */, new LayerColor(0,0,255) },
            { 1  /* grass   */, new LayerColor(0,255,0) }
        };

        public static Dictionary<int, LayerColor> BiomeBrushes = new Dictionary<int, LayerColor>
        {
            { BiomeEngine.BIOME_OCEAN, new LayerColor(0,0,255) },
            { BiomeEngine.BIOME_PLAINS, new LayerColor(0,255,0) },
            { BiomeEngine.BIOME_DESERT, new LayerColor(255,255,0) },
            { BiomeEngine.BIOME_FOREST, new LayerColor(0,127,0) },
            { 600, new LayerColor(63,63,63) }, // HACK
            { BiomeEngine.BIOME_SNOW, new LayerColor(255,255,255) }
        };

        public static Dictionary<int, LayerColor> VoronoiBrushes = new Dictionary<int, LayerColor>
        {
            { 0  /* none     */, new LayerColor(63, 63, 63) },
            { 1  /* original */, new LayerColor(255, 0, 0) },
            { 2  /* vertex   */, new LayerColor(0, 255, 0) },
            { 3  /* edge     */, new LayerColor(0, 0, 255) },
        };

        public static Dictionary<int, LayerColor> Voronoi3DBrushes = new Dictionary<int, LayerColor>
        {
            { 0  /* none     */, new LayerColor(0, 63, 63, 63) },
            { 1  /* original */, new LayerColor(15, 255, 0, 0) },
            { 2  /* vertex   */, new LayerColor(15, 0, 255, 0) },
            { 3  /* edge     */, new LayerColor(15, 0, 0, 255) },
        };

        public static Dictionary<int, LayerColor> TownBrushes = new Dictionary<int, LayerColor>
        {
            { 0  /* none     */, new LayerColor(0, 0, 0) },
            { 1  /* active   */, new LayerColor(127, 0, 0) },
            { 2  /* ruins    */, new LayerColor(127, 63, 63) },
        };

        public static Dictionary<int, LayerColor> TerrainBrushes = new Dictionary<int, LayerColor>
        {
            { 0  /* water   */, new LayerColor(2, 0, 0, 255) },
            { 1  /* stone   */, new LayerColor(2, 127, 127, 127) }
        };

        public static Dictionary<int, LayerColor> GetTerrainBrushes(int maxTerrain)
        {
            return LayerColors.GetGradientBrushesWater(-maxTerrain, maxTerrain);
        }

        /// <summary>
        /// Returns a list of brushes used as a gradient over between the minValue
        /// and maxValue parameters.
        /// </summary>
        /// <param name="minValue">The minimum value in the integer field.</param>
        /// <param name="maxValue">The maximum value in the integer field.</param>
        /// <returns></returns>
        public static Dictionary<int, LayerColor> GetGradientBrushes(int minValue, int maxValue)
        {
            Dictionary<int, LayerColor> brushes = new Dictionary<int, LayerColor>();
            for (int i = 0; i < maxValue - minValue; i++)
            {
                int a = (int)(256 * (i / (double)(maxValue - minValue)));
                brushes.Add(i + minValue, new LayerColor(a, a, a));
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
        public static Dictionary<int, LayerColor> GetGradientBrushesWater(int minValue, int maxValue)
        {
            Dictionary<int, LayerColor> brushes = new Dictionary<int, LayerColor>();
            for (int i = 0; i < maxValue - minValue; i++)
            {
                int a = (int)(256 * (i / (double)(maxValue - minValue)));
                brushes.Add(i + minValue, new LayerColor(i + minValue < 0 ? 0 : a, i + minValue < 0 ? 0 : a, a));
            }
            return brushes;
        }
    }
}
