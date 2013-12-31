// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public class TychaiaBasicSkin : IBasicSkin
    {
        public Color BackSurfaceColor
        {
            get
            {
                return new Color(96, 96, 96);
            }
        }

        public Color DarkEdgeColor
        {
            get
            {
                return new Color(32, 32, 32);
            }
        }

        public Color DarkSurfaceColor
        {
            get
            {
                return new Color(96, 96, 96);
            }
        }

        public Color LightEdgeColor
        {
            get
            {
                return new Color(160, 160, 160);
            }
        }

        public Color SurfaceColor
        {
            get
            {
                return new Color(128, 128, 128);
            }
        }

        public Color TextColor
        {
            get
            {
                return Color.Black;
            }
        }
    }
}