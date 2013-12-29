// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Ninject.Modules;
using Tychaia.Globals;
using Tychaia.Runtime;

namespace Tychaia.Network
{
    public class TychaiaServerIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Kernel.Rebind<IPersistentStorage>().To<ServerPersistentStorage>().InSingletonScope();

            // TODO: Move this to it's own module.
            this.Bind<IServerWorld>().To<DefaultServerWorld>();
        }
    }
}
