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
    [FlowDesignerCategory(FlowCategory.Towns)]
    [FlowDesignerName("Place Buildings")]
    public class Layer3DBuildingPlacer : Layer3D
    {
        public static Random r = new Random();

        [DataMember]
        [DefaultValue(1)]
        [Description("Allows us to use this for multiple iterations (so put this at different zoom levels). Zoom level 1 = 1:1 ratio.")]
        public int ZoomLevel
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(10)]
        [Description("Edge check range.")]
        public int EdgeSampling
        {
            get;
            set;
        }

        // Cityareavoronoi basically creates areas and then asks if the area has a steep slope, if so don't build there.
        // Other ways to attempt to prevent cities from building both above a below a massive cliff?
        public Layer3DBuildingPlacer(Layer citybiomes)
            : base(new Layer[] { citybiomes })
        {
            this.ZoomLevel = 1;
            this.EdgeSampling = 10;
        }

        protected override int[] GenerateDataImpl(long x, long y, long z, long width, long height, long depth)
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height * depth];

            long ox = this.EdgeSampling;
            long oy = this.EdgeSampling;
            long rx = x - this.EdgeSampling;
            long ry = y - this.EdgeSampling;
            long rw = width + this.EdgeSampling * 2;
            long rh = height + this.EdgeSampling * 2;

            // Just need to add in offsets for x + y, up to 15;
            int[] citybiomes = this.Parents[0].GenerateData(rx, ry, z, rw, rh, depth);
            int[] data = new int[width * height * depth];
            int[] tempdata = new int[width * height * depth];

            // Populate with air
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    data[i + j * width] = -1;

            // Write out the buildings list.
            for (long i = 0; i < rw; i++)
                for (long j = 0; j < rh; j++)
                {
                    int BuildingID = BuildingEngine.GetBuildingsForCell(citybiomes, ZoomLevel, r, x, y, width, height);

                    if (i + BuildingEngine.Buildings[BuildingID].Length < rw && j + BuildingEngine.Buildings[BuildingID].Width < rh)
                        for (int k = 0; k < BuildingEngine.Buildings[BuildingID].Length; k++)
                            for (int l = 0; l < BuildingEngine.Buildings[BuildingID].Width; l++)
                                data[i + k + (j + l) * width] = BuildingID;

                }

            return data;
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return null;
            else
                return BuildingEngine.GetBuildingBrushes();
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "City Biomes" };
        }

        public override string ToString()
        {
            return "Building Placement";
        }

        public override bool[] GetParents3DRequired()
        {
            return new bool[] { true };
        }
    }
}
