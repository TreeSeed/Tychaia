// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public interface IDebugCubeRenderer
    {
        void RenderWireframeCube(
            IRenderContext renderContext,
            Microsoft.Xna.Framework.BoundingBox boundingBox,
            Color? color = null);

        void RenderWireframeCube(
            IRenderContext renderContext,
            IBoundingBox boundingBox,
            Color? color = null);
    }
}
