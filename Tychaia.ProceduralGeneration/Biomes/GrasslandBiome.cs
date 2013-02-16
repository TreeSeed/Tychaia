//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Drawing;

namespace Tychaia.ProceduralGeneration.Biomes
{
    public class GrasslandBiome : Biome
    {
        public GrasslandBiome()
        {
            this.Rainfall = 0.5;
            this.Temperature = 0.5;
            this.BrushColor = Color.Green;
        }
    }
}

