// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using Tychaia.ProceduralGeneration.Flow;

namespace TychaiaWorldGenViewerAlgorithm
{
    public class TychaiaWorldGenViewerAlgorithmIoCModule : NinjectModule
    {
        public override void Load()
        {
            FlowProcessingPipeline flowProcessingPipeline = null;
            Func<IContext, FlowProcessingPipeline> load = x => flowProcessingPipeline ??
                                                               (flowProcessingPipeline = new FlowProcessingPipeline(x.Kernel.Get<IFlowProcessingRequestHandler>()));
            this.Bind<FlowProcessingPipeline>().ToMethod(load);
            this.Bind<IFlowProcessingPipeline>().ToMethod(load);
            this.Bind<IFormFactory>().ToFactory();
        }
    }
}
