// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Ninject.Extensions.Factory;
using Ninject.Modules;
using Protogame;
using Tychaia.Globals;
using Tychaia.Runtime;

namespace Tychaia.Network
{
    public class TychaiaServerIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Kernel.Rebind<IChunkGenerator>().To<ServerChunkGenerator>().InSingletonScope();

            this.Kernel.Rebind<IPersistentStorage>().To<ServerPersistentStorage>().InSingletonScope();

            this.Kernel.Bind<IAssetContentManager>().To<NullAssetContentManager>();

            this.Kernel.Bind<IServerEntityFactory>().ToFactory();
        }
    }
}