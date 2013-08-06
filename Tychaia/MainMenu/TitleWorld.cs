//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Protogame;

namespace Tychaia
{
    public class TitleWorld : MenuWorld
    {
        private static bool m_GameJustStarted = true;
        private float m_FadeAmount = 0.0f;
        
        public TitleWorld(
            I2DRenderUtilities _2dRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IBackgroundCubeEntityFactory backgroundCubeEntityFactory,
            ISkin skin)
            : base(_2dRenderUtilities, assetManagerProvider, backgroundCubeEntityFactory, skin)
        {
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
            
            this.AddMenuItem(
                this.m_AssetManager.Get<LanguageAsset>("language.GENERATE_WORLD"),
                () => {
                    if (this.m_GameContext != null)
                        this.m_TargetWorld = this.m_GameContext.CreateWorld<TychaiaGameWorld>();
                });
            this.AddMenuItem(
                this.m_AssetManager.Get<LanguageAsset>("language.LOAD_EXISTING_WORLD"),
                () => {
                    if (this.m_GameContext != null)
                        this.m_TargetWorld = this.m_GameContext.CreateWorld<LoadWorld>();
                });
            this.AddMenuItem(
                this.m_AssetManager.Get<LanguageAsset>("language.RANDOMIZE_SEED"),
                () => {
                    MenuWorld.m_StaticSeed = MenuWorld.m_Random.Next();
                });
            this.AddMenuItem(
                this.m_AssetManager.Get<LanguageAsset>("language.EXIT"),
                () => {
                    if (this.m_GameContext != null)
                        this.m_GameContext.Game.Exit();
                });
        }
    }
}

