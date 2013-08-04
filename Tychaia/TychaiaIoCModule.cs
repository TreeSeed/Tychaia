//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Ninject.Extensions.Factory;
using Ninject.Modules;
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
            this.Bind<IChunkOctreeFactory>().ToFactory();
            this.Bind<IChunkFactory>().ToFactory();
            this.Bind<ISkin>().To<TychaiaSkin>();
            this.Bind<IRenderTargetFactory>().To<DefaultRenderTargetFactory>().InSingletonScope();
            this.Bind<IIsometricRenderUtilities>().To<DefaultIsometricRenderUtilities>();
            this.Bind<IChunkRendererFactory>().ToFactory();
            this.Bind<IChunkProviderFactory>().ToFactory();
            this.Bind<ICellRenderOrderCalculator>().To<DefaultCellRenderOrderCalculator>();
            
#if DEBUG && FALSE
            var profiler = this.Kernel.Get<TychaiaProfiler>();
            this.Bind<IProfiler>().ToMethod(x => profiler);
            this.Bind<TychaiaProfiler>().ToMethod(x => profiler);
            this.Kernel.Intercept(p => p.Request.Service.IsInterface)
                .With(new TychaiaProfilingInterceptor(profiler));
#elif RELEASE
            this.Bind<IProfiler>().To<NullProfiler>().InSingletonScope();
#endif
        }
    }
}

