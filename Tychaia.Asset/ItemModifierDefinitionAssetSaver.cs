// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.Asset
{
    public class ItemModifierDefinitionAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is ItemModifierDefinitionAsset;
        }

        public dynamic Handle(IAsset asset, AssetTarget target)
        {
            var itemModifierDefinitionAsset = (ItemModifierDefinitionAsset)asset;

            return new
            {
                Loader = typeof(SpellDefinitionAssetLoader).FullName,
                Description = itemModifierDefinitionAsset.DisplayName,
                Target = itemModifierDefinitionAsset.Category,
                Effect = itemModifierDefinitionAsset.Effect,
                EffectPerLevel = itemModifierDefinitionAsset.EffectPerLevel
            };
        }
    }
}
