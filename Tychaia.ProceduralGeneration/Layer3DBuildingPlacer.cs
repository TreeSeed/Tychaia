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
        public Layer3DBuildingPlacer(Layer terrain, Layer citybiomes, Layer cityareavoronoi)
            : base(new Layer[] { terrain, citybiomes, cityareavoronoi })
        {
            this.ZoomLevel = 1;
            this.EdgeSampling = 10;
        }

        protected override int[] GenerateDataImpl(long x, long y, long z, long width, long height, long depth)
        {
            if (this.Parents.Length < 2 || this.Parents[0] == null || this.Parents[1] == null)
                return new int[width * height * depth];

            long rx = x - this.EdgeSampling;
            long ry = y - this.EdgeSampling;
            long rw = width + this.EdgeSampling * 2;
            long rh = height + this.EdgeSampling * 2;

            // Just need to add in offsets for x + y, up to 15
            int[] citybiomes = this.Parents[1].GenerateData(rx, ry, z, rw, rh, depth);
            int[] data = new int[width * height * depth];

            // Populate with air
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    for (int k = 0; k < depth; k++)
                        data[i + j * width + k * width * height] = -1;

            // Write out the buildings list.
            for (long i = 0; i < rw; i++)
                for (long j = 0; j < rh; j++)
                {
                    data[i + j * width] = BuildingEngine.GetBuildingsForCell(citybiomes, ZoomLevel, r, x, y, width, height);
                }

            return data;
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            if (this.Parents.Length < 2 || this.Parents[0] == null || this.Parents[1] == null)
                return null;
            else
                return BuildingEngine.GetBuildingBrushes();
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "City Biomes", "Terrain" };
        }

        public override string ToString()
        {
            return "Building Placement";
        }

        public override bool[] GetParents3DRequired()
        {
            return new bool[] { false, true };
        }
    }
}
