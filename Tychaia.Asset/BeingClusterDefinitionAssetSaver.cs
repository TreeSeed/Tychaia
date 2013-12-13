// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Linq;
using Protogame;

namespace Tychaia.Asset
{
    public class BeingClusterDefinitionAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is BeingClusterDefinitionAsset;
        }

        public dynamic Handle(IAsset asset)
        {
            var beingClusterDefinitionAsset = (BeingClusterDefinitionAsset)asset;

            string[] beingDefinitions = null;
            if (beingClusterDefinitionAsset.BeingDefinitions != null)
            {
                beingDefinitions = beingClusterDefinitionAsset.BeingDefinitions.Select(x => x == null ? null : x.Name).ToArray();
            }

            return new
            {
                Loader = typeof(BeingClusterDefinitionAssetLoader).FullName,
                Keyword = beingClusterDefinitionAsset.Keyword,
                LevelRequirement = beingClusterDefinitionAsset.LevelRequirement,
                Enemy = beingClusterDefinitionAsset.Enemy,
                BeingDefinitions = beingDefinitions,
                Minimum = beingClusterDefinitionAsset.Minimum,
                Maximum = beingClusterDefinitionAsset.Maximum
            };
        }
    }
}
