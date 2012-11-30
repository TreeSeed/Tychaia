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
        public int Length;              // Between 4 and 100 cells.
        public int Width;
        public int VarianceLength;
        public int VarianceWidth;
        public string CityBiomes;       // What city biomes this is located in.

        // Building Rooms
        public int[] RoomLength;
        public int[] RoomWidth;
        public Room[] RoomType;

        public Color BrushColor;
    }
}
