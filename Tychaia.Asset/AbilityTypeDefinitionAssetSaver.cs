// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.Asset
{
    public class AbilityTypeDefinitionAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is AbilityTypeDefinitionAsset;
        }

        public dynamic Handle(IAsset asset, AssetTarget target)
        {
            var abilityTypeDefinitionAsset = (AbilityTypeDefinitionAsset)asset;

            return new
            {
                Loader = typeof(AbilityTypeDefinitionAssetLoader).FullName,
                DisplayName = abilityTypeDefinitionAsset.DisplayName != null ? abilityTypeDefinitionAsset.DisplayName.Name : null,
                Description = abilityTypeDefinitionAsset.Description != null ? abilityTypeDefinitionAsset.Description.Name : null,
                AI = abilityTypeDefinitionAsset.AI != null ? abilityTypeDefinitionAsset.AI.Name : null, 
                Category = abilityTypeDefinitionAsset.Category != null ? abilityTypeDefinitionAsset.Category : null
            };
        }
    }
}
