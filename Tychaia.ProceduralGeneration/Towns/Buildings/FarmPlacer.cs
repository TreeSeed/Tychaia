using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tychaia.ProceduralGeneration.Buildings
{
    public class FarmPlacer : Building
    {
        public FarmPlacer()
        {
            this.Name = "Farm Placer";      
            this.BuildingPlacer = true;
            this.BuildingValue = 10;
            this.MinTownLocation = 10;
            this.MaxTownLocation = 20;
            this.MinSoilFertility = 0.0;
            this.GenerationType = 1;
            this.BuildingValue = 10;
            this.MinWaterValue = 0.0;
            this.MaxWaterValue = 1;
            this.Population = 10;
            this.BrushColor = Color.Green;
        }
    }
}
