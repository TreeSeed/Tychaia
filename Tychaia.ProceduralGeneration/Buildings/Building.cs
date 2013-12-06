// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    public abstract class Building
    {
        // Size of this building
        public Color BrushColor;
        public int Height;
        public int Length;

        public Profession[] Professions; // Jobs this building creates
        public int Width;

        // NOTE: Each building doesn't actually provide anything to the player, those are provided by each professsion
        // This means that if you have a building that creates 2 blacksmiths it will naturally create more things
        // You can then create specializations etc to make each npc different in a single building
        // Also helps by not having to then later attach each npc to a set group of items in a building

        // public WorldObject[] ObjectsProvided; // A list of the objects that go into this building.
        // TODO: Check if it will be better/easier to do predrawn stuff, or section-stitching or random generation
        // Still have as much random generation as we can (setting areas for where things go, etc)

        // Color that this Building draws
    }

    public static class BuildingEngine
    {
        public static List<Building> Buildings = null;

        static BuildingEngine()
        {
            Buildings = new List<Building>();
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var t in a.GetTypes())
                    if (typeof(Building).IsAssignableFrom(t) && !t.IsAbstract)
                        Buildings.Add(NewBuilding(t));
        }

        private static Building NewBuilding(Type t)
        {
            return t.GetConstructor(Type.EmptyTypes).Invoke(null) as Building;
        }

        public static Building GetBuildingForCell(double rainfall, double temperature, double terrain)
        {
            throw new NotImplementedException("GetBuildingForCell not implemented");
            return null;
        }
    }
}
