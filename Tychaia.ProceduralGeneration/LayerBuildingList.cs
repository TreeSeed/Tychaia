﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using Tychaia.ProceduralGeneration.Towns;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Selects the building list based on input data.
    /// </summary>
    [DataContract]
    public class LayerBuildingList : Layer3D
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

        public LayerBuildingList(Layer soilfertility, Layer oredensity, Layer rareoredensity, Layer militarystrength, Layer secondarybiome, Layer townscatter)
            : base(new Layer[] { soilfertility, oredensity, rareoredensity, militarystrength, secondarybiome, townscatter })
        {
            this.MinSoilFertility = 0;
            this.MaxSoilFertility = 100;
            this.MinMilitaryStrength = 0;
            this.MaxMilitaryStrength = 100;
            this.MinOreDensity = 0;
            this.MaxOreDensity = 20;
            this.MinRareOreDensity = 0;
            this.MaxRareOreDensity = 20;
        }

        public override int[] GenerateData(int x, int y, int z, int width, int height, int depth)
        {
            if (this.Parents.Length < 6 || this.Parents[0] == null || this.Parents[1] == null || this.Parents[2] == null || this.Parents[3] == null || this.Parents[4] == null || this.Parents[5] == null)
                return new int[width * height * depth];

            int[] soilfertility = this.Parents[0].GenerateData(x, y, width, height);
            int[] oredensity = this.Parents[1].GenerateData(x, y, width, height);
            int[] rareoredensity = this.Parents[2].GenerateData(x, y, width, height);
            int[] militarystrength = this.Parents[3].GenerateData(x, y, width, height);
            int[] secondarybiome = this.Parents[4].GenerateData(x, y, width, height);
            int[] townscatter = this.Parents[5].GenerateData(x, y, width, height);
            int[] data = new int[width * height * depth];

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    for (int k = 0; k < depth; k++)
                        data[i + j * width + k * width * height] = -1;

            // Write out the secondary biomes.
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if (townscatter[i + j * width] == 1)
                    {
                        // Define Per-Cell placers
                        List<int> ViableFoodBuildings = new List<int>();
                        List<int> ViableOreBuildings = new List<int>();
                        List<int> ViableMilitaryBuildings = new List<int>();
                        List<int> ViablePrestigeBuildings = new List<int>();
                        List<int> ViableWaterBuildings = new List<int>();

                        // Normalize values.
                        double nsoilfertility = (soilfertility[i + j * width] - this.MinSoilFertility) / (double)(this.MaxSoilFertility - this.MinSoilFertility);
                        double noredensity = (oredensity[i + j * width] - this.MinOreDensity) / (double)(this.MaxOreDensity - this.MinOreDensity);
                        double nrareoredensity = (rareoredensity[i + j * width] - this.MinRareOreDensity) / (double)(this.MaxRareOreDensity - this.MinRareOreDensity);
                        double nmilitarystrength = (militarystrength[i + j * width] - this.MinMilitaryStrength) / (double)(this.MaxMilitaryStrength - this.MinMilitaryStrength);
                        int nsecondarybiome = secondarybiome[i + j * width];

                        // Get the number of buildings that should be placed by this town placer.
                        Random r = this.GetCellRNG(x + i, y + j);
                        int townsize = r.Next(0, 100);

                        // Store result. Gets all the viable buildings for the town generation.
                        // Need to be able to store this within each town cell
                        ViableFoodBuildings = BuildingEngine.GetBuildingsForCell(nsoilfertility, noredensity, nrareoredensity, nmilitarystrength, nsecondarybiome, 1);
                        ViableOreBuildings = BuildingEngine.GetBuildingsForCell(nsoilfertility, noredensity, nrareoredensity, nmilitarystrength, nsecondarybiome, 2);
                        ViableMilitaryBuildings = BuildingEngine.GetBuildingsForCell(nsoilfertility, noredensity, nrareoredensity, nmilitarystrength, nsecondarybiome, 3);
                        ViablePrestigeBuildings = BuildingEngine.GetBuildingsForCell(nsoilfertility, noredensity, nrareoredensity, nmilitarystrength, nsecondarybiome, 4);
                        ViableWaterBuildings = BuildingEngine.GetBuildingsForCell(nsoilfertility, noredensity, nrareoredensity, nmilitarystrength, nsecondarybiome, 5);

                        // Select some random buildings from the list of viable ones (should create really dynamic towns that way)
                        int selection = 0;
                        int u = 1;
                        if (ViableFoodBuildings.Count != 0)
                        {
                            double foodproduction = (townsize + nsoilfertility) / 2;
                            int foodvaluecount = 0;
                            selection = r.Next(0, ViableFoodBuildings.Count);
                            while (foodvaluecount < foodproduction)
                            {
                                data[i + j * width + u * width * height] = ViableFoodBuildings[selection] + 2;
                                foodvaluecount = foodvaluecount + BuildingEngine.Buildings[ViableFoodBuildings[selection]].BuildingValue;
                                selection = r.Next(0, ViableFoodBuildings.Count);
                                u++;
                            }
                        }
                        if (ViableOreBuildings.Count != 0)
                        {
                            double oreproduction = (townsize + (noredensity + nrareoredensity) / 2) / 2;
                            int orevaluecount = 0;
                            selection = r.Next(0, ViableOreBuildings.Count);
                            while (orevaluecount < oreproduction)
                            {
                                data[i + j * width + u * width * height] = ViableOreBuildings[selection] + 2;
                                orevaluecount = orevaluecount + BuildingEngine.Buildings[ViableOreBuildings[selection]].BuildingValue;
                                selection = r.Next(0, ViableOreBuildings.Count);
                                u++;
                            }
                        }
                        if (ViableMilitaryBuildings.Count != 0)
                        {
                            double militaryvalue = (townsize + nmilitarystrength) / 2;
                            int militaryvaluecount = 0;
                            selection = r.Next(0, ViableMilitaryBuildings.Count);
                            while (militaryvaluecount < militaryvalue)
                            {
                                data[i + j * width + u * width * height] = ViableMilitaryBuildings[selection] + 2;
                                militaryvaluecount = militaryvaluecount + BuildingEngine.Buildings[ViableMilitaryBuildings[selection]].BuildingValue;
                                selection = r.Next(0, ViableMilitaryBuildings.Count);
                                u++;
                            }
                        }
                        if (ViablePrestigeBuildings.Count != 0)
                        {
                            double townprestiege = (townsize + (nrareoredensity * 3) + nsoilfertility + nmilitarystrength) / 6;
                            int prestiegevaluecount = 0;
                            selection = r.Next(0, ViablePrestigeBuildings.Count);
                            while (prestiegevaluecount < townprestiege)
                            {
                                data[i + j * width + u * width * height] = ViablePrestigeBuildings[selection] + 2;
                                prestiegevaluecount = prestiegevaluecount + BuildingEngine.Buildings[ViablePrestigeBuildings[selection]].BuildingValue;
                                selection = r.Next(0, ViablePrestigeBuildings.Count);
                                u++;
                            }
                        }
                        if (ViableWaterBuildings.Count != 0)
                        {
                            double watervalue = (townsize + nrareoredensity / 2 + nsoilfertility / 2) / 2;
                            int watervaluecount = 0;
                            selection = r.Next(0, ViableWaterBuildings.Count);
                            while (watervaluecount < watervalue)
                            {
                                data[i + j * width + u * width * height] = ViableWaterBuildings[selection] + 2;
                                watervaluecount = watervaluecount + BuildingEngine.Buildings[ViableWaterBuildings[selection]].BuildingValue;
                                selection = r.Next(0, ViableWaterBuildings.Count);
                                u++;
                            }
                        }

                        // This signifies to the next layer that they generation is going to have to move placements that are on this point
                        data[i + j * width] = 1;
                    }
                    else
                    {
                        data[i + j * width] = 0;
                    }
                }

            return data;
        }

        public override Dictionary<int, System.Drawing.Brush> GetLayerColors()
        {
            if (this.Parents.Length < 6 || this.Parents[0] == null || this.Parents[1] == null || this.Parents[2] == null || this.Parents[3] == null || this.Parents[4] == null || this.Parents[5] == null)
                return null;
            else
                return BuildingEngine.GetBuildingBrushes();
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Soil Fertility", "Ore Density", "Rare Ore Density", "Military Strength", "Secondary Biome", "Towns Map" };
        }

        public override string ToString()
        {
            return "Building Selection";
        }

        public override int StandardDepth
        {
            get { return 100; }
        }

        public override bool[] GetParents3DRequired()
        {
            return new bool[] { false, false, false, false, false, false };
        }
    }
}
