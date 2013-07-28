//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Ninject.Modules;
using Protogame;

namespace Tychaia
{
    public class TychaiaIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IChunkSizePolicy>().To<DefaultChunkSizePolicy>();
            this.Bind<IPerformancePolicy>().To<DefaultPerformancePolicy>();
            this.Bind<IRelativeChunkRendering>().To<DefaultRelativeChunkRendering>();
            this.Bind<ISkin>().To<TychaiaSkin>();
        }
    }
}

