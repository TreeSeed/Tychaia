//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Ninject.Modules;
using Tychaia.Website.Cachable;

namespace Tychaia.Website
{
    public class IoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IPhabricator>().To<Cachable.Phabricator>().InSingletonScope();
            this.Bind<IBuildServer>().To<BuildServer>().InSingletonScope();
        }
    }
}

