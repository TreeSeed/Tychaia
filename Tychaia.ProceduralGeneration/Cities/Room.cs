using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration.CityBiomes.Buildings
{
    public abstract class Room
    {
        // Name, allowing you to search
        public string Name;       // The name of the building

        // Room Size and Location
        public int VarianceLength;
        public int VarianceWidth;
        public bool Entry = false;    // Forces the room to be the entrance to the building if possible
        public bool Filler = false;   // Fills all blank spaces with this room (basically only corridors)

        // Stuff for contents etc.
        // Object list

        public LayerColor BrushColor;
    }
}
