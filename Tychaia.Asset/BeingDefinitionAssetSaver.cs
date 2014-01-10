// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.Asset
{
    public class BeingDefinitionAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is BeingDefinitionAsset;
        }

        public dynamic Handle(IAsset asset, AssetTarget target)
        {
            var beingDefinitionAsset = (BeingDefinitionAsset)asset;

            return new
            {
                Loader = typeof(BeingDefinitionAssetLoader).FullName,
                DisplayName = beingDefinitionAsset.DisplayName != null ? beingDefinitionAsset.DisplayName.Name : null,
                Description = beingDefinitionAsset.Description != null ? beingDefinitionAsset.Description.Name : null,
                TextureName = beingDefinitionAsset.Texture != null ? beingDefinitionAsset.Texture.Name : null,
                ModelName = beingDefinitionAsset.Model != null ? beingDefinitionAsset.Model.Name : null,
                HealthPerLevel = beingDefinitionAsset.HealthPerLevel,
                MovementSpeed = beingDefinitionAsset.MovementSpeed,
                Enemy = beingDefinitionAsset.Enemy
            };
        }
    }
}
