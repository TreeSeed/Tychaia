//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Microsoft.Xna.Framework;

namespace Tychaia.UI
{
    public interface IBasicSkin
    {
        Color LightEdgeColor { get; }
        Color SurfaceColor { get; }
        Color DarkSurfaceColor { get; }
        Color BackSurfaceColor { get; }
        Color DarkEdgeColor { get; }
        Color TextColor { get; }
    }
}

