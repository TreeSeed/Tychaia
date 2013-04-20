using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Reflection;
using Tychaia.ProceduralGeneration.Professions;

namespace Tychaia.ProceduralGeneration.Towns
{
    public class Town
    {
        // Placement
        public int TownSize;

        // Info
        public string TownName;


        // Color that this town draws
        public Color BrushColor;

        public Town()
        {
            TownSize = 1;
            TownName = GenerateTownName();
            BrushColor = GenerateBrushColor();
        }

        public string GenerateTownName()
        {
            return "Bobs ville";
        }

        public Color GenerateBrushColor()
        {
            return Color.Blue;
        }
    }
}
