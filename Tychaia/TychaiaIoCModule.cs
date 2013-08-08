//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Ninject.Extensions.Factory;
using Ninject.Modules;
using Protogame;
using Ninject;
using Ninject.Extensions.Interception.Infrastructure.Language;

namespace Tychaia
{
    public class TychaiaIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IChunkSizePolicy>().To<DefaultChunkSizePolicy>();
            this.Bind<IBackgroundCubeEntityFactory>().ToFactory();
            this.Bind<IChunkOctreeFactory>().ToFactory();
            this.Bind<IChunkFactory>().ToFactory();
            this.Bind<ISkin>().To<TychaiaSkin>();
            this.Bind<IRenderTargetFactory>().To<DefaultRenderTargetFactory>().InSingletonScope();
            this.Bind<IChunkProviderFactory>().ToFactory();
            this.Bind<IChunkManagerEntityFactory>().ToFactory();
            this.Bind<IChunkGenerator>().To<DefaultChunkGenerator>().InSingletonScope();
            
#if DEBUG
            // Presence of the interception library interferes with the Mono Debugger because
            // it can't seem to handle the intercepted call stack.  Therefore, under Mono, we
            // disable the profiler if the debugger is attached.
            if (!System.Diagnostics.Debugger.IsAttached || System.Type.GetType("Mono.Runtime") == null)
            {
                var profiler = this.Kernel.Get<TychaiaProfiler>();
                this.Bind<IProfiler>().ToMethod(x => profiler);
                this.Bind<TychaiaProfiler>().ToMethod(x => profiler);
                this.Kernel.Intercept(p => p.Request.Service.IsInterface)
                    .With(new TychaiaProfilingInterceptor(profiler));
            }
            else
                this.Bind<IProfiler>().To<NullProfiler>().InSingletonScope();
#else
            this.Bind<IProfiler>().To<NullProfiler>().InSingletonScope();
#endif
        }
    }
}

