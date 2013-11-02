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
            I2DRenderUtilities twodRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IBackgroundCubeEntityFactory backgroundCubeEntityFactory,
            ILevelAPI levelAPI,
            ISkin skin)
            : base(twodRenderUtilities, assetManagerProvider, backgroundCubeEntityFactory, skin)
        {
            this.AddMenuItem(
                this.AssetManager.Get<LanguageAsset>("language.PREGENERATE_WORLD"),
                () =>
                {
                    var level = levelAPI.NewLevel("test");
                    if (this.GameContext != null)
                        this.TargetWorld = this.GameContext.CreateWorld<IWorldFactory>(x => x.CreatePregenerateWorld(level));
                });
            this.AddMenuItem(
                this.AssetManager.Get<LanguageAsset>("language.CONNECT_TEST"),
                () =>
                {
                    if (this.GameContext != null)
                        this.TargetWorld = this.GameContext.CreateWorld<IWorldFactory>(x => x.CreateConnectWorld());
                });
            this.AddMenuItem(
                this.AssetManager.Get<LanguageAsset>("language.LOAD_EXISTING_WORLD"),
                () =>
                {
                    if (this.GameContext != null)
                        this.TargetWorld = this.GameContext.CreateWorld<LoadWorld>();
                });
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
