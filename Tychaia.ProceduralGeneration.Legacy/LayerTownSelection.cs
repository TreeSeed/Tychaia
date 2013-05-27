using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using Tychaia.ProceduralGeneration.Towns;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Selects the town type based on input data.
    /// </summary>
    [DataContract]
    [FlowDesignerCategory(FlowCategory.Towns)]
    [FlowDesignerName("Select Towns")]
    public class LayerTownSelection : Layer2D
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

        public LayerTownSelection(Layer soilfertility, Layer oredensity, Layer rareoredensity, Layer distancefromwater, Layer townscatter)
            : base(new Layer[] { soilfertility, oredensity, rareoredensity, distancefromwater, townscatter })
        {
            this.MinSoilFertility = 0;
            this.MaxSoilFertility = 100;
            this.MinOreDensity = 0;
            this.MaxOreDensity = 20;
            this.MinRareOreDensity = 0;
            this.MaxRareOreDensity = 20;
            this.MaxDistanceFromWater = 7;
        }

        protected override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            if (this.Parents.Length < 5 || this.Parents[0] == null || this.Parents[1] == null || this.Parents[2] == null || this.Parents[3] == null || this.Parents[4] == null)
                return new int[width * height];

            int[] soilfertility = this.Parents[0].GenerateData(x, y, width, height);
            int[] oredensity = this.Parents[1].GenerateData(x, y, width, height);
            int[] rareoredensity = this.Parents[2].GenerateData(x, y, width, height);
            int[] distancefromwater = this.Parents[3].GenerateData(x, y, width, height);
            int[] townscatter = this.Parents[4].GenerateData(x, y, width, height);
            int[] data = new int[width * height];
            List<int> ViableTowns = new List<int>();

            // Write out the secondary biomes.
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if (townscatter[i + j * width] == 1)
                    {
                        if (distancefromwater[i + j * width] <= MaxDistanceFromWater)
                        {
                            // Normalize values.
                            double nsoilfertility = (soilfertility[i + j * width] - this.MinSoilFertility) / (double)(this.MaxSoilFertility - this.MinSoilFertility);
                            double noredensity = (oredensity[i + j * width] - this.MinOreDensity) / (double)(this.MaxOreDensity - this.MinOreDensity);
                            double nrareoredensity = (rareoredensity[i + j * width] - this.MinRareOreDensity) / (double)(this.MaxRareOreDensity - this.MinRareOreDensity);
                            double ndistancefromwater = (distancefromwater[i + j * width]);

                            // Store result.
                            ViableTowns = TownEngine.GetTownsForCell(nsoilfertility, noredensity, nrareoredensity, ndistancefromwater);

                            // If no towns are viable then it returns 0
                            if (ViableTowns.Count == 0)
                            {
                                data[i + j * width] = 0;
                            }
                            else
                            {

                                // Define Variables
                                double[] TownScore = new double[ViableTowns.Count];
                                int currentbesttown = 0;
                                double currentbesttownscore = 0;

                                // Checks the list of viable towns.
                                for (int k = 0; k < ViableTowns.Count; k++)
                                {
                                    TownScore[k] = TownEngine.Towns[ViableTowns[k]].MinOreDensity + TownEngine.Towns[ViableTowns[k]].MinRareOreDensity + TownEngine.Towns[ViableTowns[k]].MinSoilFertility;
                                }

                                // Checks each town score to check which is the highest
                                for (int l = 0; l < ViableTowns.Count; l++)
                                {
                                    if (TownScore[l] > currentbesttownscore)
                                    {
                                        currentbesttownscore = TownScore[l];
                                        currentbesttown = l;
                                    }
                                    else if (TownScore[l] == currentbesttownscore)
                                    {
                                        int selected = this.GetRandomRange(x + i, y + j, 0, 2);

                                        if (selected == 0)
                                        {
                                            currentbesttown = l;
                                        }
                                    }
                                }

                                // Set the point to be equal to the town that is placed.
                                data[i + j * width] = ViableTowns[currentbesttown] + 1;
                            }
                        }
                    }
                    else
                    {
                        data[i + j * width] = 0;
                    }
                }

            return data;
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            return TownEngine.GetTownBrushes();
        }

        public override string[] GetParentsRequired()
        {
            return new string[]
            {
                "Soil Fertility",
                "Ore Density",
                "Rare Ore Density",
                "Distance from water",
                "Towns Map"
            };
        }

        public override string ToString()
        {
            return "Town Selection";
        }
    }
}
