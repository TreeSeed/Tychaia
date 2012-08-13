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
        [Description("Allows us to use this for multiple iterations (so put this at different zoom levels).")]
        public int ZoomLevel
        {
            get;
            set;
        }

        public LayerBuildingPlacer(Layer biomes, Layer townscatter)
            : base(new Layer[] {biomes, townscatter })
        {
            this.ZoomLevel = 1;
        }

        public override int[] GenerateData(int x, int y, int z, int width, int height, int depth)
        {
            if (this.Parents.Length < 2 || this.Parents[0] == null || this.Parents[1] == null)
                return new int[width * height];

            int[] biomes = this.Parents[0].GenerateData(x, y, width, height);
            int[] townscatter = this.Parents[1].GenerateData(x, y, z, width, height, depth);
            int[] data = new int[width * height];

            // Populate with air
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    for (int k = 0; k < depth; k++)
                        data[i + j * width + k * width * height] = -1;

            // Write out the buildings list.
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if (townscatter[i + j * width] ==1)
                    {
                        List<int> BuildingsList = new List<int>();

                        switch (ZoomLevel)
                        {
                            case 1:
                                // Select buildings from the buildings list that are within a certain range of DistanceFromTown
                                // Do placement into that area
                                // Need to have this so that it is always going to place something within 15 offset
                                // Repeat for multiple zoom levels
                                // Remember to check if placement is viable.
                                // After block has been placed be sure to remove it from the building list
                                break;
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
