//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Protogame;

namespace Tychaia
{
    public class BlockAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is BlockAsset;
        }

        public dynamic Handle(IAsset asset)
        {
            var blockAsset = asset as BlockAsset;
            
            return new
            {
                Loader = typeof(BlockAssetLoader).FullName,
                TopTextureName = blockAsset.IsometricCube != null ? blockAsset.IsometricCube.Name : null,
                Impassable = blockAsset.Impassable
            };
        }
    }
}

