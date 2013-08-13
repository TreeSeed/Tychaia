// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System;
using System.Diagnostics;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Extensions.Interception.Infrastructure.Language;
using Ninject.Modules;
using Protogame;

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
            this.Bind<IChunkManagerEntityFactory>().ToFactory();
            this.Bind<IChunkGenerator>().To<DefaultChunkGenerator>().InSingletonScope();
            this.Bind<ITextureAtlasAssetFactory>().To<DefaultTextureAtlasAssetFactory>();
            this.Bind<ICommand>().To<CameraCommand>();

#if DEBUG
            var profiler = this.Kernel.Get<TychaiaProfiler>();
            this.Bind<IProfiler>().ToMethod(x => profiler);
            this.Bind<TychaiaProfiler>().ToMethod(x => profiler);
                
            // Presence of the interception library interferes with the Mono Debugger because
            // it can't seem to handle the intercepted call stack.  Therefore, under Mono, we
            // disable the profiler if the debugger is attached.
            if (!Debugger.IsAttached || Type.GetType("Mono.Runtime") == null)
            {
                this.Kernel.Intercept(p => p.Request.Service.IsInterface)
                    .With(new TychaiaProfilingInterceptor(profiler));
            }
#else
            this.Bind<IProfiler>().To<NullProfiler>().InSingletonScope();
#endif
        }
    }
}
