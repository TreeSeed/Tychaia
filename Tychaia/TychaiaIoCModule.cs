//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Ninject.Modules;
using Ninject.Extensions.Factory;
using Protogame;

namespace Tychaia
{
    public class TychaiaIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IChunkSizePolicy>().To<DefaultChunkSizePolicy>();
            this.Bind<IIsometricifier>().To<DefaultIsometricifier>();
            this.Bind<IPerformancePolicy>().To<DefaultPerformancePolicy>();
            this.Bind<IRelativeChunkRendering>().To<DefaultRelativeChunkRendering>();
            this.Bind<IRenderingBuffers>().To<DefaultRenderingBuffers>().InSingletonScope();
            this.Bind<IUniqueRenderCache>().To<DefaultUniqueRenderCache>().InSingletonScope();
            this.Bind<IBackgroundCubeEntityFactory>().ToFactory();
            this.Bind<ISkin>().To<TychaiaSkin>();
            this.Bind<IRenderTargetFactory>().To<DefaultRenderTargetFactory>().InSingletonScope();
        }
    }
}

