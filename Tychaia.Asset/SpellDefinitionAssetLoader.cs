// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Protogame;

namespace Tychaia.Asset
{
    public class SpellDefinitionAssetLoader : IAssetLoader
    {
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(SpellDefinitionAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            return new SpellDefinitionAsset(
                assetManager,
                name,
                (string)data.Description,
                (string)data.Target,
                (string)data.Type,
                (string)data.Range,
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
            return new SpellDefinitionAsset(
                assetManager,
                name,
                null, 
                null, 
                null,
                null, 
                null, 
                null);
        }
    }
}
