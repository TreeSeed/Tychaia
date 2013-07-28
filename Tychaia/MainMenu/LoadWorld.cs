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
            IRenderUtilities renderUtilities,
            IAssetManagerProvider assetManagerProvider,
            ISkin skin)
            : base(renderUtilities, assetManagerProvider, skin)
        {
            var assetManager = assetManagerProvider.GetAssetManager();
            var returnText = assetManager.Get<LanguageAsset>("language.RETURN");

            this.AddMenuItem(returnText, () =>
            {
                this.m_TargetWorld = this.m_GameContext.CreateWorld<TitleWorld>();
            });

            // Get all available levels.
            foreach (LevelReference levelRef in LevelAPI.GetAvailableLevels())
            {
                this.AddMenuItem(new LanguageAsset(levelRef.Name, levelRef.Name), () =>
                {
                    this.m_TargetWorld = this.m_GameContext.CreateWorld<TychaiaGameWorld>();
                });
            }
        }
    }
}
