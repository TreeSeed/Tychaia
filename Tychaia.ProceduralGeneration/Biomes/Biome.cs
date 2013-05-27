using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Reflection;

namespace Tychaia.ProceduralGeneration.Biomes
{
    public abstract class Biome
    {
        // Requirements for biome placement
        public double Rainfall;
        public double Temperature;
        public int Terrain = 0;
        public double RainfallSelectionVariance = 1;
        public double TemperatureSelectionVariance = 1;
        public double TerrainSelectionVariance = 1;

        // Placement Modifiers and seasons
        public double RainfallModDaySummer;
        public double RainfallModNightSummer;
        public double RainfallModDayWinter;
        public double RainfallModNightWinter;
        public double RainfallVariance;
        public double TemperatureModDaySummer;
        public double TemperatureModNightSummer;
        public double TemperatureModDayWinter;
        public double TemperatureModNightWinter;
        public double TemperatureVariance;
        public double DayLengthSummer;
        public double DayLengthWinter;

        // Building assistance
        //public Material BuildingMaterial;       // The building material that the buildings are made out of in this biome

        // Color that this biome draws
        public Color BrushColor;

        // Chance that a tree will spawn on any given cell.
        public double TreeChance;
    }

    public static class BiomeEngine
    {
        public static List<Biome> Biomes = null;

        //Turns out not as easy as copy pasting
        static BiomeEngine()
        {
            BiomeEngine.Biomes = new List<Biome>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type t in a.GetTypes())
                    if (typeof(Biome).IsAssignableFrom(t) && !t.IsAbstract)
                        BiomeEngine.Biomes.Add(NewBiome(t));
        }

        private static Biome NewBiome(Type t)
        {
            return t.GetConstructor(Type.EmptyTypes).Invoke(null) as Biome;
        }

        public static Biome GetBiomeForCell(double rainfall, double temperature, double terrain)
        {
            /* What I was going to do:
             * Have it check for the most suitable biome. Biomes will just be given an average for each rain/temp/height and then it will select which biome fits best.
             */
            double[] score = new double[Biomes.Count];

            for (int i = 0; i < Biomes.Count; i++)
            {
                Biome biome = Biomes[i];

                if (biome.Terrain != 0)
                {
                    score[i] += Math.Abs(biome.Rainfall - rainfall) * biome.RainfallSelectionVariance;
                    score[i] += Math.Abs(biome.Temperature - temperature) * biome.TemperatureSelectionVariance;
                    score[i] += Math.Abs(biome.Terrain - terrain) * biome.TerrainSelectionVariance;
                }
                else
                {
                    score[i] = -1;
                }
            }

            int hold = 0;

            for (int i = 1; i < score.Count(); i++)
                if (score[hold] < score[i])
                    hold = i;

            if (score.Count() == 0)
                return null;
            else
                return Biomes[hold];
        }
    }
}
