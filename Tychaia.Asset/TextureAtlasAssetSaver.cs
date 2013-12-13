// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.Asset
{
    public class TextureAtlasAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is TextureAtlasAsset;
        }

        public dynamic Handle(IAsset asset)
        {
            return null;
        }
    }
}
