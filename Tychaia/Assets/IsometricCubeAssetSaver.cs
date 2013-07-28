//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Protogame;

namespace Tychaia
{
    public class IsometricCubeAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is IsometricCubeAsset;
        }

        public dynamic Handle(IAsset asset)
        {
            var isometricCubeAsset = asset as IsometricCubeAsset;
            
            return new
            {
                Loader = typeof(IsometricCubeAssetLoader).FullName,
                TopTextureName = isometricCubeAsset.TopTexture != null ? isometricCubeAsset.TopTexture.Name : null,
                LeftTextureName = isometricCubeAsset.LeftTexture != null ? isometricCubeAsset.LeftTexture.Name : null,
                RightTextureName = isometricCubeAsset.RightTexture != null ? isometricCubeAsset.RightTexture.Name : null
            };
        }
    }
}

