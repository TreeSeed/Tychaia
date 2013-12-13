// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Protogame;

namespace Tychaia.Asset
{
    public class BeingDefinitionAssetLoader : IAssetLoader
    {
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(BeingDefinitionAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            return new BeingDefinitionAsset(
                assetManager,
                name,
                (string)data.DisplayNameName,
                (string)data.DescriptionName,
                (string)data.TextureName,
                (string)data.HealthPerLevel,
                (string)data.MovementSpeed,
                (bool)data.Enemy);
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
            return new BeingDefinitionAsset(
                assetManager,
                name,
                null,
                null,
                null,
                null,
                null,
                true);
        }
    }
}
