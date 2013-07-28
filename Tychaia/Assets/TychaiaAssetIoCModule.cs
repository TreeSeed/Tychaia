//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Ninject.Modules;
using Protogame;

namespace Tychaia
{
    public class TychaiaAssetIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IAssetLoader>().To<IsometricCubeAssetLoader>();
            this.Bind<IAssetSaver>().To<IsometricCubeAssetSaver>();
            this.Bind<IAssetLoader>().To<BlockAssetLoader>();
            this.Bind<IAssetSaver>().To<BlockAssetSaver>();
        }
    }
}

