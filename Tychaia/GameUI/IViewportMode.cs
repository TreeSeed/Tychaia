// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework.Graphics;

namespace Tychaia
{
    public interface IViewportMode
    {
        int SidebarWidth { get; set; }

        Viewport Get3DViewport(Viewport original);

        void SetViewportMode(ViewportMode mode);
    }
}

