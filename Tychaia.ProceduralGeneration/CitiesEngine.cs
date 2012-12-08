using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Tychaia.ProceduralGeneration.CityBiomes;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    public static class CitiesEngine
    {
        public static List<CityBiome> CityBiomes = null;
        public static List<SecondaryCityBiome> SecondaryCityBiomes = null;

        static CitiesEngine()
        {
            CitiesEngine.CityBiomes = new List<CityBiome>();
            CitiesEngine.SecondaryCityBiomes = new List<SecondaryCityBiome>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type t in a.GetTypes())
                    if (typeof(CityBiome).IsAssignableFrom(t) && !t.IsAbstract)
                        CitiesEngine.CityBiomes.Add(CitiesEngine.NewCity(t));
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type t in a.GetTypes())
                    if (typeof(SecondaryCityBiome).IsAssignableFrom(t) && !t.IsAbstract)
                        CitiesEngine.CityBiomes.Add(CitiesEngine.NewSecondary(t));
        }

        private static CityBiome NewCity(Type t)
        {
            return t.GetConstructor(Type.EmptyTypes).Invoke(null) as CityBiome;
        }

        private static SecondaryCityBiome NewSecondary(Type t)
        {
            return t.GetConstructor(Type.EmptyTypes).Invoke(null) as SecondaryCityBiome;
        }

        public static int GetCityBiomeForCell(double soilfertility, double animaldensity, double oredensity, double rareoredensity, int citybiome)
        {
            if (CitiesEngine.CityBiomes.Count > citybiome)
            {
                CityBiome cb = CitiesEngine.CityBiomes[citybiome];
                if (soilfertility >= cb.MinSoilFertility && soilfertility < cb.MaxSoilFertility &&
                    animaldensity >= cb.MinAnimalDensity && animaldensity < cb.MaxAnimalDensity &&
                    oredensity >= cb.MinOreDensity && oredensity < cb.MaxOreDensity &&
                    rareoredensity >= cb.MinRareOreDensity && rareoredensity < cb.MaxRareOreDensity)
                {
                    return (1);
                }
                else return 0;
            }
            else
            {
                return -1;
            }
        }

        public static int AddCityBiomeForCell(double soilfertility, double animaldensity, double oredensity, double rareoredensity, int citybiome, int citybiomecount)
        {
            if (CitiesEngine.CityBiomes.Count > citybiome)
            {
                CityBiome cb = CitiesEngine.CityBiomes[citybiome];
                if (soilfertility >= (cb.MinSoilFertility - (1 - cb.MinSoilFertility)) && soilfertility < cb.MaxSoilFertility &&
                    animaldensity >= (cb.MinAnimalDensity - (1 - cb.MinAnimalDensity)) && animaldensity < cb.MaxAnimalDensity &&
                    oredensity >= (cb.MinOreDensity - (1 - cb.MinOreDensity)) && oredensity < cb.MaxOreDensity &&
                    rareoredensity >= (cb.MinRareOreDensity - (1 - cb.MinRareOreDensity)) && rareoredensity < cb.MaxRareOreDensity)
                {
                    return (1);
                }
                else return 0;
            }
            else
            {
                return -1;
            }
        }

        public static int GetSecondaryCityBiomeForCell(int citybiome, int citybiomescount)
        {
            if (CitiesEngine.SecondaryCityBiomes.Count > citybiome)
            {
                SecondaryCityBiome cb = CitiesEngine.SecondaryCityBiomes[citybiome];
                if (citybiomescount >= cb.RequiredOtherBiomes)
                    return (1);
            }
            else
            {
                return 0;
            }

            return -1;
        }

        public static Dictionary<int, System.Drawing.Brush> GetCityBiomeBrushes()
        {
            Dictionary<int, System.Drawing.Brush> result = new Dictionary<int, System.Drawing.Brush>();
            result.Add(0, new System.Drawing.SolidBrush(Color.Black));
            for (int i = 0; i < CitiesEngine.CityBiomes.Count; i++)
                result.Add(i + 1, new System.Drawing.SolidBrush(CitiesEngine.CityBiomes[i].BrushColor));
            return result;
        }
    }
}
