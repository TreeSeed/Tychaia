// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Ninject.Modules;
using Protogame;

namespace Tychaia.Asset
{
    public class TychaiaAssetIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IAssetLoader>().To<BlockAssetLoader>();
            this.Bind<IAssetSaver>().To<BlockAssetSaver>();
            this.Bind<IAssetSaver>().To<TextureAtlasAssetSaver>();
        }
    }
}
