//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Ninject.Modules;
using Tychaia.ProceduralGeneration.Flow;

namespace Tychaia.ProceduralGeneration
{
    public class TychaiaProceduralGenerationIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IFlowProcessingRequestHandler>().To<FlowProcessingRequestHandler>().InSingletonScope();
            this.Bind<IGeneratorResolver>().To<DefaultGeneratorResolver>();
        }
    }
}

