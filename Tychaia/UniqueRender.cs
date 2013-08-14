// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework.Graphics;

namespace Tychaia
{
    public class UniqueRender
    {
        public RenderTarget2D DepthMap;
        public RenderTarget2D Target;

        public UniqueRender(RenderTarget2D target, RenderTarget2D depthMap)
        {
            this.Target = target;
            this.DepthMap = depthMap;
            target.Disposing += (sender, e) => { this.Target = null; };
            depthMap.Disposing += (sender, e) => { this.DepthMap = null; };
        }
    }
}
