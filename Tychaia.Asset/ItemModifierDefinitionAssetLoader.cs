// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Protogame;

namespace Tychaia.Asset
{
    public class ItemModifierDefinitionAssetLoader : IAssetLoader
    {
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(ItemModifierDefinitionAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            return new ItemModifierDefinitionAsset(
                assetManager,
                name,
                (string)data.DisplayName,
                (string)data.Category,
                (string)data.Effect,
                (string)data.EffectPerLevel);
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
            return new ItemModifierDefinitionAsset(
                assetManager,
                name,
                null, 
                null, 
                null,
                null);
        }
    }
}
