using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Tychaia.ProceduralGeneration.Towns;

namespace Tychaia.ProceduralGeneration
{
    public static class TownEngine
    {
        public static List<Town> Towns = null;

        static TownEngine()
        {
            TownEngine.Towns = new List<Town>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type t in a.GetTypes())
                    if (typeof(Town).IsAssignableFrom(t) && !t.IsAbstract)
                        TownEngine.Towns.Add(TownEngine.NewTown(t));
        }

        private static Town NewTown(Type t)
        {
            return t.GetConstructor(Type.EmptyTypes).Invoke(null) as Town;
        }

        public static List<int> GetTownsForCell(double soilfertility, double oredensity, double rareoredensity, double distancefromwater)
        {
            List<int> ViableTowns = new List<int>();
            for (int i = 0; i < TownEngine.Towns.Count; i++)
            {
                Town t = TownEngine.Towns[i];   // Need to try implement a method that will place the best town possible
                if (soilfertility >= t.MinSoilFertility &&
                    oredensity >= t.MinOreDensity && 
                    rareoredensity >= t.MinRareOreDensity)
                    ViableTowns.Add(i);
            }

            return ViableTowns;
        }

        public static Dictionary<int, LayerColor> GetTownBrushes()
        {
            Dictionary<int, LayerColor> result = new Dictionary<int, LayerColor>();
            for (int i = 0; i < TownEngine.Towns.Count; i++)
                result.Add(i + 1, TownEngine.Towns[i].BrushColor);
            result.Add(0, LayerColor.Black);
            return result;
        }
    }
}
