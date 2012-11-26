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

        static CitiesEngine()
        {
            CitiesEngine.CityBiomes = new List<CityBiome>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type t in a.GetTypes())
                    if (typeof(CityBiome).IsAssignableFrom(t) && !t.IsAbstract)
                        CitiesEngine.CityBiomes.Add(CitiesEngine.NewCity(t));
        }

        private static CityBiome NewCity(Type t)
        {
            return t.GetConstructor(Type.EmptyTypes).Invoke(null) as CityBiome;
        }

        public static int GetCityBiomeForCell(double soilfertility, double militarystrength, double oredensity, double rareoredensity, double height)
        {
            for (int i = 0; i < CitiesEngine.CityBiomes.Count; i++)
            {
                CityBiome cb = CitiesEngine.CityBiomes[i];
                if (soilfertility >= cb.MinSoilFertility && soilfertility < cb.MaxSoilFertility &&
                    militarystrength >= cb.MinMilitaryStrength && militarystrength < cb.MaxMilitaryStrength &&
                    oredensity >= cb.MinOreDensity && oredensity < cb.MaxOreDensity &&
                    rareoredensity >= cb.MinRareOreDensity && rareoredensity < cb.MaxRareOreDensity &&
                    height >= cb.MinHeight && height < cb.MaxHeight)
                    return (i + 1);
            }

            return 0;
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
