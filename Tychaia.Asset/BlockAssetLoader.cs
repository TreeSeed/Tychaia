// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.Asset
{
    public class BlockAssetLoader : IAssetLoader
    {
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(BlockAssetLoader).FullName;
        }

        public bool CanNew()
        {
            return true;
        }

        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            return null;
        }

        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            return new BlockAsset(assetManager, name, 0, null, null, null, null, null, null, true);
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            return new BlockAsset(
                assetManager, 
                name, 
                (int)data.BlockID, 
                (string)data.TopTextureName, 
                (string)data.BottomTextureName, 
                (string)data.LeftTextureName, 
                (string)data.RightTextureName, 
                (string)data.FrontTextureName, 
                (string)data.BackTextureName, 
                (bool)data.Impassable);
        }
    }
}