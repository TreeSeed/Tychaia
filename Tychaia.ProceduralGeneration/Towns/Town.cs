// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    public class Town
    {
        // Placement


        // Color that this town draws
        public Color BrushColor;
        public string TownName;
        public int TownSize;

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
