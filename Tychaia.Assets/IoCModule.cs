//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Ninject.Modules;

namespace Tychaia.Assets
{
    public class IoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IAssetLoader>().To<TextAssetLoader>();
            this.Bind<IRawAssetLoader>().To<RawAssetLoader>();
        }
    }
}

