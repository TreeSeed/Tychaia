// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Diagnostics;
using Ninject;
using Ninject.Extensions.Interception.Infrastructure.Language;
using Ninject.Modules;
using Protogame;
using Tychaia.Globals;

namespace Tychaia
{
    public class TychaiaProfilingIoCModule : NinjectModule
    {
        public override void Load()
        {
#if DEBUG
            var profiler = this.Kernel.Get<TychaiaProfiler>();
            this.Bind<IProfiler>().ToMethod(x => profiler);
            this.Bind<TychaiaProfiler>().ToMethod(x => profiler);

            // Check if per-method profiling is disabled in the settings.
            var storage = this.Kernel.Get<IPersistentStorage>();
            if (storage.Settings.PerMethodProfiling ?? true)
            {
                // Presence of the interception library interferes with the Mono Debugger because
                // it can't seem to handle the intercepted call stack.  Therefore, under Mono, we
                // disable the profiler if the debugger is attached.
                if (!Debugger.IsAttached || Type.GetType("Mono.Runtime") == null)
                {
                    this.Kernel.Intercept(p => p.Request.Service.IsInterface)
                        .With(new TychaiaProfilingInterceptor(profiler));
                }
            }
#else
            this.Bind<IProfiler>().To<NullProfiler>().InSingletonScope();
#endif
        }
    }
}
