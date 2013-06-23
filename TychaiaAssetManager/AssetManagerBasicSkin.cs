//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Tychaia.UI;
using Microsoft.Xna.Framework;

namespace TychaiaAssetManager
{
    public class AssetManagerBasicSkin : IBasicSkin
    {
        public Color LightEdgeColor { get { return new Color(160, 160, 160); } }
        public Color BackSurfaceColor { get { return new Color(96, 96, 96); } }
        public Color SurfaceColor { get { return new Color(128, 128, 128); } }
        public Color DarkEdgeColor { get { return new Color(32, 32, 32); } }
        public Color TextColor { get { return Color.Black; } }
    }
}

