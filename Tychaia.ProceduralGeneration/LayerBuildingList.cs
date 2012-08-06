using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Selects the building locations based on input data.
    /// </summary>
    [DataContract]
    public class LayerBuildingList : Layer2D
    {
        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the soil fertility map.")]
        public int MinSoilFertility
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum integer value in the soil fertility map.")]
        public int MaxSoilFertility
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the military strength map.")]
        public int MinMilitaryStrength
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum integer value in the military strength map.")]
        public int MaxMilitaryStrength
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the ore density map.")]
        public int MinOreDensity
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(20)]
        [Description("The maximum integer value in the ore density map.")]
        public int MaxOreDensity
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the rare ore density map.")]
        public int MinRareOreDensity
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(20)]
        [Description("The maximum integer value in the rare ore density map.")]
        public int MaxRareOreDensity
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(7)]
        [Description("The maximum integer value for the city distance from water.")]
        public int MaxDistanceFromWater
        {
            get;
            set;
        }

        public LayerBuildingList(Layer soilfertility, Layer oredensity, Layer rareoredensity, Layer militarystrength, Layer distancefromwater, Layer townscatter)
            : base(new Layer[] { soilfertility, oredensity, rareoredensity, militarystrength, distancefromwater, townscatter })
        {
            this.MinSoilFertility = 0;
            this.MaxSoilFertility = 100;
            this.MinMilitaryStrength = 0;
            this.MaxMilitaryStrength = 100;
            this.MinOreDensity = 0;
            this.MaxOreDensity = 20;
            this.MinRareOreDensity = 0;
            this.MaxRareOreDensity = 20;
            this.MaxDistanceFromWater = 7;
        }

        public override int[] GenerateData(int x, int y, int width, int height)
        {
            if (this.Parents.Length < 6 || this.Parents[0] == null || this.Parents[1] == null || this.Parents[2] == null || this.Parents[3] == null || this.Parents[4] == null || this.Parents[5] == null)
                return new int[width * height];

            int[] soilfertility = this.Parents[0].GenerateData(x, y, width, height);
            int[] oredensity = this.Parents[1].GenerateData(x, y, width, height);
            int[] rareoredensity = this.Parents[2].GenerateData(x, y, width, height);
            int[] militarystrength = this.Parents[3].GenerateData(x, y, width, height);
            int[] distancefromwater = this.Parents[4].GenerateData(x, y, width, height);
            int[] townscatter = this.Parents[5].GenerateData(x, y, width, height);
            int[] data = new int[width * height];
            List<int> ViableFoodBuildings = new List<int>();
            List<int> ViableOreBuildings = new List<int>();
            List<int> ViableMilitaryBuildings = new List<int>();
            List<int> ViablePrestigeBuildings = new List<int>();
            List<int> ViableWaterBuildings = new List<int>();
            List<int> BuildingsList = new List<int>();

            // Write out the secondary biomes.
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if (townscatter[i + j * width] == 1) // Need to change this so it checks if it is building off of a town or off of a building placer
                    {
                        // Normalize values.
                        double nsoilfertility = (soilfertility[i + j * width] - this.MinSoilFertility) / (double)(this.MaxSoilFertility - this.MinSoilFertility);
                        double noredensity = (oredensity[i + j * width] - this.MinOreDensity) / (double)(this.MaxOreDensity - this.MinOreDensity);
                        double nrareoredensity = (rareoredensity[i + j * width] - this.MinRareOreDensity) / (double)(this.MaxRareOreDensity - this.MinRareOreDensity);
                        double nmilitarystrength = (militarystrength[i + j * width] - this.MinMilitaryStrength) / (double)(this.MaxMilitaryStrength - this.MinMilitaryStrength);
                        double ndistancefromwater = (distancefromwater[i + j * width]);

                        // Get the number of buildings that should be placed by this town placer.
                        Random r = this.GetCellRNG(x + i, y + j);
                        int townsize = r.Next(0, 100);

                        double townprestiege = (townsize + (nrareoredensity * 3) + nsoilfertility + nmilitarystrength) / 6;
                        double foodproduction = (townsize + nsoilfertility) /2;
                        double oreproduction = (townsize + (noredensity + nrareoredensity) / 2) / 2;
                        double militaryvalue = (townsize + nmilitarystrength) /2;
                        double watervalue = (townsize + nrareoredensity / 2 + nsoilfertility / 2) / 2;

                        // Store result. Gets all the viable buildings for the town generation.
                        // Need to be able to store this within each town cell
                        ViableFoodBuildings = BuildingEngine.GetBuildingsForCell(nsoilfertility, noredensity, nrareoredensity, nmilitarystrength, ndistancefromwater, 1);
                        ViableOreBuildings = BuildingEngine.GetBuildingsForCell(nsoilfertility, noredensity, nrareoredensity, nmilitarystrength, ndistancefromwater, 2);
                        ViableMilitaryBuildings = BuildingEngine.GetBuildingsForCell(nsoilfertility, noredensity, nrareoredensity, nmilitarystrength, ndistancefromwater, 3);
                        ViablePrestigeBuildings = BuildingEngine.GetBuildingsForCell(nsoilfertility, noredensity, nrareoredensity, nmilitarystrength, ndistancefromwater, 4);
                        ViableWaterBuildings = BuildingEngine.GetBuildingsForCell(nsoilfertility, noredensity, nrareoredensity, nmilitarystrength, ndistancefromwater, 5);

                        // Place all building placers, will have to place close range buildings really close.
                        // Make sure that each of the viable food, ore and military buildings add up to the variables above.
                        int foodvaluecount = 0;
                        int orevaluecount = 0;
                        int militaryvaluecount = 0;
                        int prestiegevaluecount = 0;
                        int watervaluecount = 0;

                        // Calculate total score for each production type
                        for (int k = 0; k < ViableFoodBuildings.Count; k++)
                        {
                            foodvaluecount = BuildingEngine.Buildings[ViableFoodBuildings[k]].BuildingValue + foodvaluecount;                            
                        }
                        for (int l = 0; l < ViableOreBuildings.Count; l++)
                        {
                            orevaluecount = BuildingEngine.Buildings[ViableOreBuildings[l]].BuildingValue + orevaluecount;
                        }
                        for (int m = 0; m < ViableMilitaryBuildings.Count; m++)
                        {
                            militaryvaluecount = BuildingEngine.Buildings[ViableMilitaryBuildings[m]].BuildingValue + militaryvaluecount;
                        }
                        for (int n = 0; n < ViablePrestigeBuildings.Count; n++)
                        {
                            prestiegevaluecount = BuildingEngine.Buildings[ViablePrestigeBuildings[n]].BuildingValue + prestiegevaluecount;
                        }
                        for (int u = 0; u < ViableWaterBuildings.Count; u++)
                        {
                            watervaluecount = BuildingEngine.Buildings[ViableWaterBuildings[u]].BuildingValue + watervaluecount;
                        }

                        // Select some random buildings from the list of viable ones (should create really dynamic towns that way)
                        int selection = 0;
                        int o = 0;
                        int p = 0;
                        int q = 0;
                        int s = 0;
                        int t = 0;

                        while (foodvaluecount < foodproduction)
                        {
                            selection = r.Next(0, ViableFoodBuildings.Count - o);
                            BuildingsList[o] = ViableFoodBuildings[selection];
                            foodvaluecount = foodvaluecount + BuildingEngine.Buildings[ViableFoodBuildings[selection]].BuildingValue;
                            o++;
                        }
                        while (orevaluecount < oreproduction)
                        {
                            selection = r.Next(0, ViableOreBuildings.Count - p);
                            BuildingsList[o + p] = ViableOreBuildings[selection];
                            orevaluecount = orevaluecount + BuildingEngine.Buildings[ViableOreBuildings[selection]].BuildingValue;
                            p++;
                        }
                        while (militaryvaluecount < militaryvalue)
                        {
                            selection = r.Next(0, ViableMilitaryBuildings.Count - q);
                            BuildingsList[o + p + q] = ViableMilitaryBuildings[selection];
                            militaryvaluecount = militaryvaluecount + BuildingEngine.Buildings[ViableMilitaryBuildings[selection]].BuildingValue;
                            q++;
                        }
                        while (prestiegevaluecount < townprestiege)
                        {
                            selection = r.Next(0, ViablePrestigeBuildings.Count - s);
                            BuildingsList[o + p + q + s] = ViablePrestigeBuildings[selection];
                            prestiegevaluecount = prestiegevaluecount + BuildingEngine.Buildings[ViablePrestigeBuildings[selection]].BuildingValue;
                            s++;
                        }
                        while (watervaluecount < watervalue)
                        {
                            selection = r.Next(0, ViableWaterBuildings.Count - t);
                            BuildingsList[o + p + q + s + t] = ViableWaterBuildings[selection];
                            watervaluecount = watervaluecount + BuildingEngine.Buildings[ViableWaterBuildings[selection]].BuildingValue;
                            s++;
                        }

                        // Will need to store the BuildingsList in the town center location
                        int BuildingsListTemp = 0;
                        if (BuildingEngine.Buildings.Count() > 500 || BuildingsList.Count > 127)
                        {
                            throw new IndexOutOfRangeException("You have more than 500 builings in your database! WOW! You should cut that down to 500 so the program works.");
                        }
                        else
                        {
                            if (BuildingsList.Count() > 63)
                            {
                                for (int z = 0; z < BuildingsList.Count(); z++)
                                {
                                    if (z > 63)
                                    {
                                        BuildingsListTemp = BuildingsListTemp - (-BuildingsList[z] - 500);
                                    }
                                    else
                                    {
                                        BuildingsListTemp = BuildingsListTemp + (BuildingsList[z] + 500);
                                    }
                                }
                            }
                            else
                            {
                                for (int z = 0; z < BuildingsList.Count(); z++)
                                {
                                    BuildingsListTemp = BuildingsListTemp + (BuildingsList[z] + 500);
                                }
                            }
                        }

                        // Write the number of buildings within the list to the points around the center point (that way no matter where it is located you will be able to check it)
                        data[i + j * width] = BuildingsListTemp;
                        if ((i + 1 + (j + 1) * width) != 0)
                        {
                            data[i + 1 + (j + 1) * width] = BuildingsList.Count();
                        }
                        if ((i - 1 + (j + 1) * width) != 0)
                        {
                            data[i - 1 + (j + 1) * width] = BuildingsList.Count();
                        }
                        if ((i - 1 + (j - 1) * width) != 0)
                        {
                            data[i - 1 + (j - 1) * width] = BuildingsList.Count();
                        }
                        if ((i + 1 + (j - 1) * width) != 0)
                        {
                            data[i + 1 + (j - 1) * width] = BuildingsList.Count();
                        }
                    }
                }

            return data;
        }

        public override Dictionary<int, System.Drawing.Brush> GetLayerColors()
        {
            return BuildingEngine.GetBuildingBrushes();
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Soil Fertility", "Ore Density", "Rare Ore Density", "Distance from water", "Towns Map" };
        }

        public override string ToString()
        {
            return "Building Selection";
        }
    }
}
