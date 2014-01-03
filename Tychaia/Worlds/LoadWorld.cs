// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;
using Tychaia.Runtime;

namespace Tychaia
{
    public class LoadWorld : MenuWorld
    {
        public LoadWorld(
            I2DRenderUtilities twodRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IBackgroundCubeEntityFactory backgroundCubeEntityFactory,
            ILevelAPI levelAPI,
            ISkin skin)
            : base(twodRenderUtilities, assetManagerProvider, backgroundCubeEntityFactory, skin)
        {
            var assetManager = assetManagerProvider.GetAssetManager();
            var returnText = assetManager.Get<LanguageAsset>("language.RETURN");

            this.AddMenuItem(returnText, () => { this.TargetWorld = this.GameContext.CreateWorld<TitleWorld>(); });

            // Get all available levels.
            foreach (var levelRef in levelAPI.GetAvailableLevels())
            {
                this.AddMenuItem(
                    new LanguageAsset(levelRef, levelRef),
                    () => { this.TargetWorld = this.GameContext.CreateWorld<TychaiaGameWorld>(); });
            }
        }
    }
}
