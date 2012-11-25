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
    public class LayerBuildingPlacer : Layer3D
    {
        [DataMember]
        [DefaultValue(1)]
        [Description("Allows us to use this for multiple iterations (so put this at different zoom levels). Zoom level 1 = 1:1 ratio.")]
        public int ZoomLevel
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(5)]
        [Description("Edge check range.")]
        public int EdgeSampling
        {
            get;
            set;
        }
        public LayerBuildingPlacer(Layer biomes, Layer townscatter)
            : base(new Layer[] {biomes, townscatter })
        {
            this.ZoomLevel = 1;
            this.EdgeSampling = 5;
        }

        protected override int[] GenerateDataImpl(long x, long y, long z, long width, long height, long depth)
        {
            if (this.Parents.Length < 2 || this.Parents[0] == null || this.Parents[1] == null)
                return new int[width * height * depth];

            long ox = this.EdgeSampling;
            long oy = this.EdgeSampling;
            long rx = x - this.EdgeSampling;
            long ry = y - this.EdgeSampling;
            long rw = width + this.EdgeSampling * 2;
            long rh = height + this.EdgeSampling * 2;

            // Just need to add in offsets for x + y, up to 15
            int[] biomes = this.Parents[0].GenerateData(rx, ry, rw, rh);
            int[] townscatter = this.Parents[1].GenerateData(rx, ry, z, rw, rh, depth);
            int[] data = new int[width * height * depth];
            int[] tempdata = new int[width * height * depth];

            // Populate with air
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    for (int k = 0; k < depth; k++)
                        data[i + j * width + k * width * height] = -1;

            // Write out the buildings list.
            for (long i = 0; i < rw; i++)
                for (long j = 0; j < rh; j++)
                {
                    if (townscatter[i + j * rw] == 1 || townscatter[i + j * rw] == -2 || townscatter[i + j * rw] == -3)
                    {
                        List<int> BuildingsList = new List<int>();
                        int p = 1;

                        while (townscatter[i + j * rw + p * rw * rh] != -1)
                        {
                            BuildingsList.Add(townscatter[i + j * rw + p * rw * rh] - 2);
                            p++;
                        }

                        if (townscatter[i + j * rw] == 1)
                        {
                            data = PlaceBuildings(width, height, depth, biomes, i, j, rw, ox, oy, BuildingsList, x + i, y + j, data);
                        }
                        else if (townscatter[i + j * rw] == -2)
                        {
                            List<int> NewBuildingsList = new List<int>();
                            List<int> TempBuildingsList = new List<int>();
                            // Add possible evaluate placers
                            for (int k = 0; k < BuildingsList.Count; k++)
                            {
                                if (BuildingEngine.Buildings[BuildingsList[k]].BuildingPlacer == true)
                                {
                                    NewBuildingsList = BuildingEngine.GetPlacerBuildingsForCell(biomes[i + j * rw], BuildingEngine.Buildings[BuildingsList[k]].BuildingPlacerID);
                                    int m = 0;
                                    if (NewBuildingsList.Count != 0)
                                    {
                                        int valuecount = 0;
                                        int selection = this.GetRandomRange(x + i, y + j, NewBuildingsList.Count);
                                        while (valuecount < BuildingEngine.Buildings[NewBuildingsList[m]].BuildingPlacerValue)
                                        {
                                            TempBuildingsList.Add(NewBuildingsList[selection] + 2);
                                            valuecount = valuecount + BuildingEngine.Buildings[NewBuildingsList[selection]].BuildingValue;
                                            selection = this.GetRandomRange(x + i, y + j, NewBuildingsList.Count);
                                            m++;
                                        }
                                        data = PlaceBuildings(width, height, depth, biomes, i, j, rw, ox, oy, TempBuildingsList, x + i, y + j, data);
                                    }
                                }
                                else
                                {
                                    if (i >= ox && i <= (rw - ox) && j >= oy && j <= (rh - oy))
                                    {
                                        int o = 1;
                                        while (data[i - ox + (j - oy) * width + o * width * height] != -1)
                                        {
                                            o++;
                                        }
                                        data[i - ox + (j - oy) * width + o * width * height] = BuildingsList[k] + 2;
                                    }
                                }
                            }
                        }
                        else if (townscatter[i + j * rw] == -3)
                        {
                            // Just adds the single building in that cell to its list. Passes to the next layer.
                            if (i >= ox && i <= (rw - ox) && j >= oy && j <= (rh - oy))
                            {
                                int o = 1;
                                while (data[i - ox + (j - oy) * width + o * width * height] != -1)
                                {
                                    o++;
                                }
                                data[i - ox + (j - oy) * width + o * width * height] = BuildingsList[0] + 2;
                            }
                        }
                    }
                }

            // Means that if there is more than 1 building placed then it will not place any more.
            for (int i = 0; i < rw; i++)
                for (int j = 0; j < rh; j++)
                {
                    if (i >= ox && i <= (rw - ox) && j >= oy && j <= (rh - oy))
                    {
                        if (data[i - ox + (j - oy) * width + 2 * width * height] != -1 && data[i - ox + (j - oy) * width] != -2)
                        {
                            data[i - ox + (j - oy) * width] = 1;
                        }
                        else if (data[i - ox + (j - oy) * width] != 2 && data[i - ox + (j - oy) * width + 1 * width * height] != -1)
                        {
                            data[i - ox + (j - oy) * width] = -3;
                        }
                        else
                        {
                            data[i - ox + (j - oy) * width] = 0;
                        }
                    }
                }
            return data;
        }

        public int[] PlaceBuildings(long width, long height, long depth, int[] biomes, long i, long j, long rw, long ox, long oy, List<int> BuildingsList, long kx, long ky, int[] data)
        {
            long selection = 0;
            long placementside = 0;
            long placementvalue = 0;
            int trytimeout = 0;
            bool onland = true;

            for (int k = 0; k < BuildingsList.Count; k++)
            {
                if (BuildingEngine.Buildings[BuildingsList[k]].MaxTownLocation / ZoomLevel > 0 && BuildingEngine.Buildings[BuildingsList[k]].MaxTownLocation / ZoomLevel < this.EdgeSampling)
                {
                    // Checks in a square around it, have to make this check so it is a circle, else it may get innacurate. 
                    int maxzoom = BuildingEngine.Buildings[BuildingsList[k]].MaxTownLocation / ZoomLevel;
                    int minzoom = BuildingEngine.Buildings[BuildingsList[k]].MinTownLocation / ZoomLevel;
                    // Won't place on water                
                    do
                    {
                        selection = this.GetRandomRange(kx, ky, minzoom, maxzoom, 0);
                        placementside = this.GetRandomRange(kx, ky, 1, 4);
                        placementvalue = this.GetRandomRange(kx, ky, 1, (selection * 2));
                        switch (placementside)
                        {
                            case 1:
                                if (BiomeEngine.SecondaryBiomes[biomes[i + (1 * selection) + (j + placementvalue - selection + 1) * rw]].MaxTerrain == 0)
                                {
                                    onland = true;
                                }
                                break;
                            case 2:
                                if (BiomeEngine.SecondaryBiomes[biomes[i - (1 * selection) + (j + placementvalue - selection + 1) * rw]].MaxTerrain == 0)
                                {
                                    onland = true;
                                }
                                break;
                            case 3:
                                if (BiomeEngine.SecondaryBiomes[biomes[i + placementvalue - selection + 1 + (j + (1 * selection) * rw)]].MaxTerrain == 0)
                                {
                                    onland = true;
                                }
                                break;
                            case 4:
                                if (BiomeEngine.SecondaryBiomes[biomes[i + placementvalue - selection + 1 + (j - (1 * selection) * rw)]].MaxTerrain == 0)
                                {
                                    onland = true;
                                }
                                break;
                        }
                        trytimeout++;
                    } while (onland == false && trytimeout < (selection * 2) + 3);
                    if (trytimeout < (selection * 2) + 3)
                    {
                        int o = 1;
                        switch (placementside)
                        {
                            case 1:
                                if ((i + (1 * selection)) >= ox && (j + placementvalue - selection + 1) >= oy && (i + (1 * selection)) <= (rw - ox) && (j + placementvalue - selection + 1) <= (rw - oy))
                                {
                                    while (data[i + (1 * selection) - ox + (j + placementvalue - selection + 1 - oy) * width + o * width * height] != -1)
                                    {
                                        o++;
                                    }
                                    data[i + (1 * selection) - ox + (j + placementvalue - selection + 1 - oy) * width + o * width * height] = BuildingsList[k] + 2;

                                    if (BuildingEngine.Buildings[BuildingsList[k]].BuildingPlacer == true)
                                    {
                                        data[i + (1 * selection) - ox + (j + placementvalue - selection + 1 - oy) * width] = -2;
                                    }
                                }
                                break;
                            case 2:
                                if ((i - (1 * selection)) >= ox && (j + placementvalue - selection + 1) >= oy && (i - (1 * selection)) <= (rw - ox) && (j + placementvalue - selection + 1) <= (rw - oy))
                                {
                                    while (data[i - (1 * selection) - ox + (j + placementvalue - selection + 1 - oy) * width + o * width * height] != -1)
                                    {
                                        o++;
                                    }
                                    data[i - (1 * selection) - ox + (j + placementvalue - selection + 1 - oy) * width + o * width * height] = BuildingsList[k] + 2;

                                    if (BuildingEngine.Buildings[BuildingsList[k]].BuildingPlacer == true)
                                    {
                                        data[i - (1 * selection) - ox + (j + placementvalue - selection + 1 - oy) * width] = -2;
                                    }
                                }
                                break;
                            case 3:
                                if ((j + (1 * selection)) >= oy && (i + placementvalue - selection + 1) >= ox && (j + (1 * selection)) <= (rw - oy) && (i + placementvalue - selection + 1) <= (rw - ox))
                                {
                                    while (data[i + placementvalue - selection + 1 - ox + (j + 1 * selection - oy) * width + o * width * height] != -1)
                                    {
                                        o++;
                                    }
                                    data[i + placementvalue - selection + 1 - ox + (j + 1 * selection - oy) * width + o * width * height] = BuildingsList[k] + 2;

                                    if (BuildingEngine.Buildings[BuildingsList[k]].BuildingPlacer == true)
                                    {
                                        data[i + placementvalue - selection + 1 - ox + (j + 1 * selection - oy) * width] = -2;
                                    }
                                }
                                break;
                            case 4:
                                if ((j - (1 * selection)) >= oy && (i + placementvalue - selection + 1) >= ox && (i + placementvalue - selection + 1) <= (rw - ox) && (j - (1 * selection)) <= (rw - oy))
                                {
                                    while (data[i + placementvalue - selection + 1 - ox + (j - 1 * selection - oy) * width + o * width * height] != -1)
                                    {
                                        o++;
                                    }
                                    data[i + placementvalue - selection + 1 - ox + (j - 1 * selection - oy) * width + o * width * height] = BuildingsList[k] + 2;

                                    if (BuildingEngine.Buildings[BuildingsList[k]].BuildingPlacer == true)
                                    {
                                        data[i + placementvalue - selection + 1 - ox + (j - 1 * selection - oy) * width] = -2;
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (i > ox && i < (rw - ox) && j > oy && j < (rw - oy))
                        {
                            int o = 1;
                            while (data[i - ox + (j - oy) * width + o * width * height] != -1)
                            {
                                o++;
                            }
                            data[i - ox + (j - oy) * width + o * width * height] = BuildingsList[k] + 2;
                        }
                    }
                }
            }
            return data;
        }

        public override Dictionary<int, System.Drawing.Brush> GetLayerColors()
        {
            if (this.Parents.Length < 2 || this.Parents[0] == null || this.Parents[1] == null)
                return null;
            else
                return BuildingEngine.GetBuildingBrushes();
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "Secondary Biomes", "Towns Map" };
        }

        public override string ToString()
        {
            return "Building Placement";
        }

        public override int StandardDepth
        {
            get { return 16; }
        }

        public override bool[] GetParents3DRequired()
        {
            return new bool[] { false, true };
        }
    }
}
