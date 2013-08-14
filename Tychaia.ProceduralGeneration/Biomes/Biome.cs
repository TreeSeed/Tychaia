// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Tychaia.ProceduralGeneration.Biomes
{
    public abstract class Biome
    {
        // Requirements for biome placement
        public Color BrushColor;
        public double DayLengthSummer;
        public double DayLengthWinter;
        public double Rainfall;

        // Placement Modifiers and seasons
        public double RainfallModDaySummer;
        public double RainfallModDayWinter;
        public double RainfallModNightSummer;
        public double RainfallModNightWinter;
        public double RainfallSelectionVariance = 1;
        public double RainfallVariance;
        public double Temperature;
        public double TemperatureModDaySummer;
        public double TemperatureModDayWinter;
        public double TemperatureModNightSummer;
        public double TemperatureModNightWinter;
        public double TemperatureSelectionVariance = 1;
        public double TemperatureVariance;
        public int Terrain = 0;
        public double TerrainSelectionVariance = 1;

        // Chance that a tree will spawn on any given cell.
        public double TreeChance;
    }

    public static class BiomeEngine
    {
        public static List<Biome> Biomes = null;

        //Turns out not as easy as copy pasting
        static BiomeEngine()
        {
            Biomes = new List<Biome>();
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var t in a.GetTypes())
                    if (typeof(Biome).IsAssignableFrom(t) && !t.IsAbstract)
                        Biomes.Add(NewBiome(t));
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
            var score = new double[Biomes.Count];

            for (var i = 0; i < Biomes.Count; i++)
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

            var hold = 0;

            for (var i = 1; i < score.Count(); i++)
                if (score[hold] < score[i])
                    hold = i;

            if (score.Count() == 0)
                return null;
            return Biomes[hold];
        }

        public static Biome GetSimpleBiomeForCell()
        {
            var r = new Random();

            var b = r.Next(Biomes.Count);

            Biome biome = Biomes[b];

            return biome;
        }
    }
}
