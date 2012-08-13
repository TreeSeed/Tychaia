using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Tychaia.ProceduralGeneration.Buildings;
using Tychaia.ProceduralGeneration.Biomes;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    public static class BuildingEngine
    {
        public static List<Building> Buildings = null;

        static BuildingEngine()
        {
            BuildingEngine.Buildings = new List<Building>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type t in a.GetTypes())
                    if (typeof(Building).IsAssignableFrom(t) && !t.IsAbstract)
                        BuildingEngine.Buildings.Add(BuildingEngine.NewBuilding(t));
        }

        private static Building NewBuilding(Type t)
        {
            return t.GetConstructor(Type.EmptyTypes).Invoke(null) as Building;
        }

        public static List<int> GetBuildingsForCell(double soilfertility, double oredensity, double rareoredensity, double militarystrength, int secondarybiome, int generationtype)
        {
            List<int> ViableBuildings = new List<int>();
            SecondaryBiome biome = BiomeEngine.SecondaryBiomes[secondarybiome]; 

            for (int i = 0; i < BuildingEngine.Buildings.Count; i++)
            {
                Building b = BuildingEngine.Buildings[i];
                if (soilfertility >= b.MinSoilFertility &&
                    oredensity >= b.MinOreDensity &&
                    rareoredensity >= b.MinRareOreDensity &&
                    militarystrength >= b.MinMilitaryStrength &&
                    generationtype == b.GenerationType &&
                    biome.HeatValue > b.MinHeatValue &&
                    biome.HeatValue < b.MaxHeatValue &&
                    biome.WaterValue > b.MinWaterValue &&
                    biome.WaterValue < b.MaxWaterValue)
                {

                    ViableBuildings.Add(i);
                }
            }

            return ViableBuildings;
        }

        public static Dictionary<int, System.Drawing.Brush> GetBuildingBrushes()
        {
            Dictionary<int, System.Drawing.Brush> result = new Dictionary<int, System.Drawing.Brush>();
            result.Add(0, new System.Drawing.SolidBrush(Color.Black));
            result.Add(1, new System.Drawing.SolidBrush(Color.Red));
            result.Add(-1, new System.Drawing.SolidBrush(Color.Transparent));
            for (int i = 0; i < BuildingEngine.Buildings.Count; i++)
                result.Add(i + 2, new System.Drawing.SolidBrush(BuildingEngine.Buildings[i].BrushColor));
            return result;
        }
    }
}
