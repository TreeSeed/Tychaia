using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Reflection;

namespace Tychaia.ProceduralGeneration.Professions
{
    public abstract class Profession
    {
        // Things this profession creates

        // Services this profession provides

        // Color that this Profession draws
        public Color BrushColor;
    }

    public static class ProfessionEngine
    {
        public static List<Profession> Professions = null;

        //Turns out not as easy as copy pasting
        static ProfessionEngine()
        {
            ProfessionEngine.Professions = new List<Profession>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type t in a.GetTypes())
                    if (typeof(Profession).IsAssignableFrom(t) && !t.IsAbstract)
                        ProfessionEngine.Professions.Add(NewProfession(t));
        }

        private static Profession NewProfession(Type t)
        {
            return t.GetConstructor(Type.EmptyTypes).Invoke(null) as Profession;
        }

        public static Profession GetProfessionForCell(double rainfall, double temperature, double terrain)
        {
            throw new NotImplementedException("GetProfessionForCell not implemented");

            return null;

        }
    }
}
