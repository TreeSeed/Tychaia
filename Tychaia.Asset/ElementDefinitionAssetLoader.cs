// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Protogame;

namespace Tychaia.Asset
{
    public class ElementDefinitionAssetLoader : IAssetLoader
    {
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(ElementDefinitionAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            return new ElementDefinitionAsset(
                assetManager,
                name,
                (string)data.DisplayNameName);
        }

        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            return null;
        }

        public bool CanNew()
        {
            return true;
        }

        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            return new ElementDefinitionAsset(
                assetManager,
                name,
                null);
        }
    }
}
