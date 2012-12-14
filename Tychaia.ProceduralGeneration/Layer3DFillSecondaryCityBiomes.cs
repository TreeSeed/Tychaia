using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Generates city biomes based on input data.
    /// </summary>
    [DataContract]
    public class Layer3DFillSecondaryCityBiomes : Layer3D
    {
        public Layer3DFillSecondaryCityBiomes(Layer citybiomes)
            : base( citybiomes )
        {
        }

        protected override int[] GenerateDataImpl(long x, long y, long z, long width, long height, long depth)
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height * depth];

            int[] citybiome = this.Parents[0].GenerateData(x, y, width, height);
            int[] data = new int[width * height * depth];

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    for (int k = 0; k < depth; k++)
                    {
                        data[i + j * width + k * width * height] = -1;
                    }

            // Write out the city biomes.
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    try
                    {
                        data[i + j * width] = 0;
                        if (citybiome[i + j * width] != 0)
                        {                            
                            // Store result.
                            bool endloop = false;
                            int citybiomecount = 0;
                            while (endloop == false)
                            {
                                if (citybiome[i + j * width + citybiomecount * width * height] == 1)
                                {
                                    citybiomecount++;
                                }
                                else if (citybiome[i + j * width + citybiomecount * width * height] == 0)
                                {
                                    endloop = true;
                                }
                            }

                            int citybiomenumber = 0;
                            endloop = false;
                            while (endloop == false)
                            {
                                int temp = CitiesEngine.GetSecondaryCityBiomeForCell(citybiomenumber, citybiomecount);
                                if (temp != 0)
                                {
                                    data[i + j * width + citybiomenumber + 1 * width * height] = temp;
                                    citybiomenumber++;
                                    data[i + j * width] = 1;
                                }
                                else
                                {
                                    data[i + j * width + citybiomenumber + 1 * width * height] = temp;
                                    endloop = true;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // In case of overflow, underflow or divide by zero.
                        data[i + j * width] = 0;
                    }
                }

            return data;
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            return CitiesEngine.GetCityBiomeBrushes();
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "City Biome"};
        }

        public override string ToString()
        {
            return "Secondary City Biomes";
        }

        public override bool[] GetParents3DRequired()
        {
            return new bool[] { true };
        }
    }
}
