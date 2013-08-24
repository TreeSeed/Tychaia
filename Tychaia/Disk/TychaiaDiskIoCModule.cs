// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Ninject.Modules;
using Ninject.Extensions.Factory;

namespace Tychaia
{
    public class TychaiaDiskIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ILevelAPI>().To<CombinedLevelAPI>();
            this.Bind<ILevelAPIImpl>().To<TychaiaLevelAPIImpl>().Named("Default");
            this.Bind<ITychaiaLevelFactory>().ToFactory();
            this.Bind<IFlowBundleSerializer>().To<DefaultFlowBundleSerializer>();
        }
    }
}
