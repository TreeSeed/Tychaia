using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tychaia.ProceduralGeneration.Buildings
{
    public abstract class Building
    {
        // Name, allowing you to search
        public string Name;       // The name of the building

        // Building Size and Location
        public int Height;           // How tall the building is
        public int Length;           // How long the building is
        public int Width;            // How wide the building is 
        public int MaxTownLocation;  // The higher this value the further away from the town it will be
        public int MinTownLocation;  // The minimum distance from town center (used for farms ect that wouldn't be in center)
        public bool BuildingPlacer;  // Signifies if this is a building that is only used to place other buildings
        public int BuildingPlacerID; // All buildings are placed as placers then evaluated at the end, however this signifies that this is purely a placer.
        public int BuildingPlacerValue;

        //Building Requirements
        public int MaxDistanceFromWater;             // Value of how far this is from water, used for fishing and docks.
        public int MinPopulation;                    // Value for the minimum population required for making this building
        public double MinSoilFertility = 0;          // Out of 1.00 (Used for placing food generation and sometimes water generation)
        public double MinOreDensity = 0;             // Out of 1.00 (Used for placing mines)
        public double MinRareOreDensity = 0;         // Out of 1.00 (Used for placing prestiege style buildings, such as castles and festival grounds)
        public double MinMilitaryStrength = 0;       // Out of 1.00 (Used for placing military area such as barrcks and training grounds)
        public int GenerationType;                   // 1: Food, 2: Ore, 3: Millitary, 4: Prestige, 5: Water, etc
                                                     // 1: Any building that generates food, 2: Mining buildings, 3: castles, other things that are taken from being a rich city, 4: water generation (wells, etc)
                                                     // 5: Barracks, outposts ect
                                                     // Use this genertaion type to filter what things you want (you can use it for walls, houses, castile pieces, farm pieces ect)
        public int BuildingValue = 1;                // The value that this gives towards its building type.
                                                     // Set to 1 so it doesn't cause infinite loops.

        // Secondary Biome Requirements
        public double MinWaterValue = 0.0;            // Required water value from the secondary biome for this building to be placed.
        public double MaxWaterValue = 1.0;            // Out of 1.00
        public double MinHeatValue = 0.0;             // Required heat value from the secondary biome for this building to be placed. 
        public double MaxHeatValue = 1.0;             // Out of 1.00

        // Building Production
        public int Population;      // Lists the number of people that work at the building.
                                    // This is used to place houses around the area of this building


        public Color BrushColor;
    }
}
