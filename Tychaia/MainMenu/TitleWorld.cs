// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia
{
    public class TitleWorld : MenuWorld
    {
        public TitleWorld(
            I2DRenderUtilities _2DRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IBackgroundCubeEntityFactory backgroundCubeEntityFactory,
            ISkin skin)
            : base(_2DRenderUtilities, assetManagerProvider, backgroundCubeEntityFactory, skin)
        {
            this.AssetManager = assetManagerProvider.GetAssetManager(false);

            this.AddMenuItem(
                this.AssetManager.Get<LanguageAsset>("language.GENERATE_WORLD"),
                () =>
                {
                    if (this.GameContext != null)
                        this.TargetWorld = this.GameContext.CreateWorld<TychaiaGameWorld>();
                });
            this.AddMenuItem(
                this.AssetManager.Get<LanguageAsset>("language.LOAD_EXISTING_WORLD"),
                () =>
                {
                    if (this.GameContext != null)
                        this.TargetWorld = this.GameContext.CreateWorld<LoadWorld>();
                });
            this.AddMenuItem(
                this.AssetManager.Get<LanguageAsset>("language.RANDOMIZE_SEED"),
                () => { StaticSeed = Random.Next(); });
            this.AddMenuItem(
                this.AssetManager.Get<LanguageAsset>("language.EXIT"),
                () =>
                {
                    if (this.GameContext != null)
                        this.GameContext.Game.Exit();
                });
        }
    }
}
