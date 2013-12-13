// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia.Asset
{
    public class SpellDefinitionAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is SpellDefinitionAsset;
        }

        public dynamic Handle(IAsset asset)
        {
            var spellDefinitionAsset = (SpellDefinitionAsset)asset;

            return new
            {
                Loader = typeof(SpellDefinitionAssetLoader).FullName,
                Description = spellDefinitionAsset.Description,
                Target = spellDefinitionAsset.Target,
                Type = spellDefinitionAsset.Type,
                Range = spellDefinitionAsset.Range,
                Effect = spellDefinitionAsset.Effect,
                EffectPerLevel = spellDefinitionAsset.EffectPerLevel
            };
        }
    }
}
