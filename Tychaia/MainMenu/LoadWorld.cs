// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using Protogame;
using Tychaia.Disk;

namespace Tychaia
{
    public class LoadWorld : MenuWorld
    {
        public LoadWorld(
            I2DRenderUtilities _2DRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IBackgroundCubeEntityFactory backgroundCubeEntityFactory,
            ISkin skin)
            : base(_2DRenderUtilities, assetManagerProvider, backgroundCubeEntityFactory, skin)
        {
            var assetManager = assetManagerProvider.GetAssetManager();
            var returnText = assetManager.Get<LanguageAsset>("language.RETURN");

            this.AddMenuItem(returnText, () => { this.TargetWorld = this.GameContext.CreateWorld<TitleWorld>(); });

            // Get all available levels.
            foreach (var levelRef in LevelAPI.GetAvailableLevels())
            {
                this.AddMenuItem(new LanguageAsset(levelRef.Name, levelRef.Name),
                    () => { this.TargetWorld = this.GameContext.CreateWorld<TychaiaGameWorld>(); });
            }
        }
    }
}