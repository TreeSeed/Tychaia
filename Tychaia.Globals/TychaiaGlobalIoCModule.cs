// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using Ninject.Modules;

namespace Tychaia.Globals
{
    public class TychaiaGlobalIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IFilteredConsole>().To<DefaultFilteredConsole>();
            this.Bind<IFilteredFeatures>().To<DefaultFilteredFeatures>();
        }
    }
}