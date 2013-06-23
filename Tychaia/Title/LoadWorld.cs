//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Tychaia.Disk;
using Tychaia.Assets;
using Tychaia.Globals;
using Ninject;

namespace Tychaia.Title
{
    public class LoadWorld : MenuWorld
    {
        public LoadWorld()
        {
            var assetManagerProvider = IoC.Kernel.Get<IAssetManagerProvider>();
            var assetManager = assetManagerProvider.GetAssetManager();
            var returnText = assetManager.Get("language.RETURN").Resolve<TextAsset>();

            this.AddMenuItem(returnText, () =>
            {
                this.m_TargetWorld = IoC.Kernel.Get<TitleWorld>();
            });

            // Get all available levels.
            foreach (LevelReference levelRef in LevelAPI.GetAvailableLevels())
            {
                this.AddMenuItem(returnText/*levelRef.Name*/, () =>
                {
                    this.m_TargetWorld = new RPGWorld(levelRef);
                });
            }
        }
    }
}
