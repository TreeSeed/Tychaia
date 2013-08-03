//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Protogame;

namespace Tychaia
{
    public interface IIsometricifier
    {
        IsometricCubeAsset Isometricify(IGameContext gameContext, TextureAsset flatTexture);
    }
}

