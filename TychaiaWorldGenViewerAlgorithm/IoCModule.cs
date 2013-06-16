//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Ninject.Modules;
using Tychaia.ProceduralGeneration.Flow;

namespace TychaiaWorldGenViewerAlgorithm
{
    public class IoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IFlowProcessingPipeline>().To<FlowProcessingPipeline>().InSingletonScope();
        }
    }
}

