// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Protogame;

namespace Tychaia
{
    public class BlockAssetLoader : IAssetLoader
    {
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(BlockAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            return new BlockAsset(
                assetManager,
                name,
                data.TopTextureName,
                data.BottomTextureName,
                data.LeftTextureName,
                data.RightTextureName,
                data.FrontTextureName,
                data.BackTextureName,
                data.Impassable);
        }

        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            throw new InvalidOperationException();
        }

        public bool CanNew()
        {
            return true;
        }

        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            return new BlockAsset(
                assetManager,
                name,
                null,
                null,
                null,
                null,
                null,
                null,
                true);
        }
    }
}
