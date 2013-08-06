//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Ninject.Modules;
using Ninject.Extensions.Factory;

namespace Tychaia
{
    public class TychaiaIsometricIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IIsometricCameraFactory>().ToFactory();
        }
    }
}

