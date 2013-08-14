// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Drawing;

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
            Professions = new List<Profession>();
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var t in a.GetTypes())
                    if (typeof(Profession).IsAssignableFrom(t) && !t.IsAbstract)
                        Professions.Add(NewProfession(t));
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
