using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Reflection;
using Tychaia.ProceduralGeneration.Professions;

namespace Tychaia.ProceduralGeneration.Buildings
{
    public abstract class Building
    {
        // Size of this building
        public int Length;
        public int Width;
        public int Height;

        public Profession[] Professions; // Jobs this building creates
        // NOTE: Each building doesn't actually provide anything to the player, those are provided by each professsion
        // This means that if you have a building that creates 2 blacksmiths it will naturally create more things
        // You can then create specializations etc to make each npc different in a single building
        // Also helps by not having to then later attach each npc to a set group of items in a building

//        public WorldObject[] ObjectsProvided; // A list of the objects that go into this building.
        // TODO: Check if it will be better/easier to do predrawn stuff, or section-stitching or random generation
        //                                               Still have as much random generation as we can (setting areas for where things go, etc)
        

        // Color that this Building draws
        public Color BrushColor;
    }

    public static class BuildingEngine
    {
        public static List<Building> Buildings = null;

        //Turns out not as easy as copy pasting
        static BuildingEngine()
        {
            BuildingEngine.Buildings = new List<Building>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type t in a.GetTypes())
                    if (typeof(Building).IsAssignableFrom(t) && !t.IsAbstract)
                        BuildingEngine.Buildings.Add(NewBuilding(t));
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
