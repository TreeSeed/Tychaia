using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Tychaia.ProceduralGeneration.Biomes;

namespace Tychaia.ProceduralGeneration
{
    public static class BiomeEngine
    {
        public static List<SecondaryBiome> SecondaryBiomes = null;
        public static List<TertiaryBiome> TertiaryBiomes = null;

        public const int BIOME_OCEAN = 0;
        public const int BIOME_PLAINS = 1;
        public const int BIOME_DESERT = 2;
        public const int BIOME_FOREST = 3;
        public const int BIOME_SNOW = 4;

        static BiomeEngine()
        {
            BiomeEngine.SecondaryBiomes = new List<SecondaryBiome>();
            BiomeEngine.TertiaryBiomes = new List<TertiaryBiome>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type t in a.GetTypes())
                    if (typeof(SecondaryBiome).IsAssignableFrom(t) && !t.IsAbstract)
                        BiomeEngine.SecondaryBiomes.Add(BiomeEngine.NewSB(t));
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type t in a.GetTypes())
                    if (typeof(TertiaryBiome).IsAssignableFrom(t) && !t.IsAbstract)
                        BiomeEngine.TertiaryBiomes.Add(BiomeEngine.NewTB(t));
        }

        private static SecondaryBiome NewSB(Type t)
        {
            return t.GetConstructor(Type.EmptyTypes).Invoke(null) as SecondaryBiome;
        }

        private static TertiaryBiome NewTB(Type t)
        {
            return t.GetConstructor(Type.EmptyTypes).Invoke(null) as TertiaryBiome;
        }

        public static int GetSecondaryBiomeForCell(int biome, double rainfall, double temperature, double terrain)
        {
            for (int i = 0; i < BiomeEngine.SecondaryBiomes.Count; i++)
            {
                SecondaryBiome sb = BiomeEngine.SecondaryBiomes[i];
                if (sb.SuitableBiomes.Contains(biome) &&
                    rainfall >= sb.MinRainfall && rainfall < sb.MaxRainfall &&
                    temperature >= sb.MinTemperature && temperature < sb.MaxTemperature &&
                    terrain >= sb.MinTerrain && terrain < sb.MaxTerrain)
                    return i;
            }

            for (int i = 0; i < BiomeEngine.SecondaryBiomes.Count; i++)
            {
                SecondaryBiome sb = BiomeEngine.SecondaryBiomes[i];
                if (sb.DefaultFor == biome)
                    return i;
            }

            return -1;
        }

        public static Dictionary<int, System.Drawing.Brush> GetSecondaryBiomeBrushes()
        {
            Dictionary<int, System.Drawing.Brush> result = new Dictionary<int, System.Drawing.Brush>();
            for (int i = 0; i < BiomeEngine.SecondaryBiomes.Count; i++)
                result.Add(i, new System.Drawing.SolidBrush(BiomeEngine.SecondaryBiomes[i].BrushColor));
            return result;
        }
    }
}
