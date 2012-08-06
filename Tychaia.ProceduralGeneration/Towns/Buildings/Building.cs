using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tychaia.ProceduralGeneration.Buildings
{
    public abstract class Building
    {
        // |   |  __   _____  ___
        // |\  | /  \    |   |          NOTE: NEED TO MAKE THESE INTO SUB CLASSES
        // | \ | |  |    |   |--
        // |  \| \__/    |   |___

        // Name, allowing you to search
        public string Name;       // The name of the building

        // Building Size and Location
        public int Height;           // How tall the building is
        public int Length;           // How long the building is
        public int Width;            // How wide the building is 
        public int MaxTownLocation;  // The higher this value the further away from the town it will be
        public int MinTownLocation;  // The minimum distance from town center (used for farms ect that wouldn't be in center)
        public bool BuildingPlacer;  // Signifies if this is a building that is only used to place other buildings
                                     // All buildings are placed as placers then evaluated at the end, however this signifies that this is purely a placer.

        //Building Requirements
        public int MaxDistanceFromWater;         // Value of how far this is from water, used for fishing and docks.
        public int MinPopulation;                // Value for the minimum population required for making this building
        public double MinSoilFertility;          // Out of 1.00 (Used for placing food generation and sometimes water generation)
        public double MinOreDensity;             // Out of 1.00 (Used for placing mines)
        public double MinRareOreDensity;         // Out of 1.00 (Used for placing prestiege style buildings, such as castles and festival grounds)
        public double MinMilitaryStrength;       // Out of 1.00 (Used for placing military area such as barrcks and training grounds)
        public int BuildingValue;                // The value that this gives towards its building type.
        public int BuildingType;                 // 1: Food Generation, 2: Ore Generation, 3: Military, 4: Prestige, 5: Water Generation
                                                 // 1: Farms (animals, fruit, vegrables), 2: Mines, 3: Barracks, training grounds, ect, 4: Castles, large marketplaces, tournament grounds,
                                                 // This is so that the generator knows what style of builing is being generated, so that it can limit the number of generators in each city
                                                 // This value can be used when placing normal living areas to determing what buildings are to be placed (or possibly leave to just search name?)
        public int BuildingTier;                 // 1: Caused by cities (farms, mining camps, castles, ect), these should always be BuilingPlacers 
                                                 // 2: Caused by cities after outlying generation
                                                 // 3: Caused by cities later (tavers, churches, ect). These are buildings that will be placed at certain popultaion limits
                                                 // 4: Caused by other buildings (farm pieces, mining camp ares, castle lookouts, ect)
                                                 // 5: Basic houses shapes list (just a list of all the different accomodation sizes that will be randomly assigned)
                                                 // 6: Walls

        // Secondary Biome Requirements
        public double MinWaterValue;            // Required water value from the secondary biome for this building to be placed.
        public double MaxWaterValue;            // Out of 1.00
        public double MinHeatValue;             // Required heat value from the secondary biome for this building to be placed. 
        public double MaxHeatValue;             // Out of 1.00

        // Building Production
        public int Population;      // Lists the number of people that work at the building.
                                    // This is used to place houses around the area of this building


        public Color BrushColor;
    }
}
