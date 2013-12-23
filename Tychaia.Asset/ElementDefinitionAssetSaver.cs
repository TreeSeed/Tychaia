// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.Asset
{
    public class ElementDefinitionAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is ElementDefinitionAsset;
        }

        public dynamic Handle(IAsset asset, AssetTarget target)
        {
            var elementDefinitionAsset = (ElementDefinitionAsset)asset;

            return new
            {
                Loader = typeof(ElementDefinitionAssetLoader).FullName,
                DisplayName = elementDefinitionAsset.DisplayName != null ? elementDefinitionAsset.DisplayName.Name : null
            };
        }
    }
}
