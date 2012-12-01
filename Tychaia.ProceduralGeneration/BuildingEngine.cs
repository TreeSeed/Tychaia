using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Tychaia.ProceduralGeneration.CityBiomes.Buildings;
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

        public static int GetBuildingsForCell(int[] citybiomes, int zoomlevel, Random r, long x, long y, long width, long length)
        {
            List<int> ViableBuildings = new List<int>();
            
            for (int i = 0; i < BuildingEngine.Buildings.Count; i++)
            {
                Building b = BuildingEngine.Buildings[i];
                if (b.Length < (zoomlevel * 10) || b.Height < (zoomlevel * 10))
                {
                    double rand = r.NextDouble();
                    if (rand > b.PlaceLimit)
                        for (int k = 0; citybiomes[x + y * width + k * length * width] != 0; k++)
                            for (int j = 0; j < b.CityBiomes.Length; j++)
                                if (b.CityBiomes[j] == CitiesEngine.CityBiomes[k])
                                    ViableBuildings.Add(i);
                }
            }

            for (int i = 0; i < ViableBuildings.Count; i++)
            {
                double rand = r.NextDouble();
                if ((1 / ViableBuildings.Count) > rand)
                    return i;
            }

            return -1;
        }

        public static Dictionary<int, System.Drawing.Brush> GetBuildingBrushes()
        {
            Dictionary<int, System.Drawing.Brush> result = new Dictionary<int, System.Drawing.Brush>();
            result.Add(-1, new System.Drawing.SolidBrush(Color.Transparent));
            for (int i = 0; i < BuildingEngine.Buildings.Count; i++)
                result.Add(i, new System.Drawing.SolidBrush(BuildingEngine.Buildings[i].BrushColor));
            return result;
        }
    }
}
