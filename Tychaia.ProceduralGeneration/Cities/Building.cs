using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tychaia.ProceduralGeneration.CityBiomes.Buildings
{
    public abstract class Building
    {
        // Name, allowing you to search
        public string Name;       // The name of the building

        // Building Size and Location
        public int Height;              // How tall the building is
        public int Size;                // How big the building is (1 = small, 2 = medium, 3 = large)
        public string CityBiomes;       // What city biomes this is located in.

        // Stuff for contents etc.


        public Color BrushColor;
    }
}
