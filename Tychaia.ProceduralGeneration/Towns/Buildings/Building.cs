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

        //Building Requirements
        public int MaxDistanceFromWater;         // Value of how far this is from water, used for fishing and docks.
        public double MinSoilFertility;          // Out of 1.00 (Used for placing food generation and sometimes water generation)
        public double MinOreDensity;             // Out of 1.00 (Used for placing mines)
        public double MinRareOreDensity;         // Out of 1.00 (Used for placing prestiege style buildings, such as castles and festival grounds)
        public double MinMilitaryStrength;       // Out of 1.00 (Used for placing military area such as barrcks and training grounds)
        public int BuildingType;                 // 1: Food Generation, 2: Drink Generation, 3: Ore Generation, 4: Rare Ore Generation
                                                 // This is so that the generator knows what style of builing is being generated, so that it can limit the number of generators in each city
                                                 // This value can be used when placing normal living areas to determing what buildings are to be placed (or possibly leave to just search name?)
        public int BuildingTier;                 // 1: Caused by cities (farms, mining camps, castles, ect), 
                                                 // 2: Caused by cities later (tavers, churches, ect), 
                                                 // 3: Basic houses shapes list (just a list of all the different accomodation sizes that will be randomly assigned)
                                                 // 4: Caused by other buildings (farm pieces, castle walls, ect)

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
