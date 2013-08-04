//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Protogame;

namespace Tychaia
{
    public interface IIsometricRenderUtilities
    {
        void RenderEntity(
            IRenderContext renderContext,
            TychaiaGameWorld gameWorld,
            IsometricEntity entity,
            IsometricCamera camera,
            OccludingSpriteBatch occludingSpriteBatch,
            TextureAsset textureAsset);
    }
}

