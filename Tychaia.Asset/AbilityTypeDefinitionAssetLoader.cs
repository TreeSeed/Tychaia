// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Protogame;

namespace Tychaia.Asset
{
    public class AbilityTypeDefinitionAssetLoader : IAssetLoader
    {
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(AbilityTypeDefinitionAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            return new AbilityTypeDefinitionAsset(
                assetManager,
                name,
                (string)data.DisplayName,
                (string)data.Description,
                (string)data.AIName,
                (string)data.Category);
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
            return new AbilityTypeDefinitionAsset(
                assetManager,
                name,
                null,
                null,
                null,
                null);
        }
    }
}
