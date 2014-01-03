// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.Asset
{
    public class BlockAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is BlockAsset;
        }

        public dynamic Handle(IAsset asset, AssetTarget target)
        {
            var blockAsset = (BlockAsset)asset;

            return new
            {
                Loader = typeof(BlockAssetLoader).FullName, blockAsset.BlockID,
                TopTextureName = blockAsset.TopTexture != null ? blockAsset.TopTexture.Name : null,
                BottomTextureName = blockAsset.BottomTexture != null ? blockAsset.BottomTexture.Name : null,
                LeftTextureName = blockAsset.LeftTexture != null ? blockAsset.LeftTexture.Name : null,
                RightTextureName = blockAsset.RightTexture != null ? blockAsset.RightTexture.Name : null,
                FrontTextureName = blockAsset.FrontTexture != null ? blockAsset.FrontTexture.Name : null,
                BackTextureName = blockAsset.BackTexture != null ? blockAsset.BackTexture.Name : null,
                blockAsset.Impassable
            };
        }
    }
}
