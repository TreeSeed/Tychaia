// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using ManyConsole;
using Ninject.Modules;

namespace TychaiaTool
{
    public class TychaiaToolIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ConsoleCommand>().To<ProceduralCompilerCommand>();
            this.Bind<ConsoleCommand>().To<ProceduralStorageCommand>();
            this.Bind<ConsoleCommand>().To<ProceduralPlannerCommand>();
            this.Bind<ConsoleCommand>().To<ProceduralPerformanceCommand>();
            this.Bind<ConsoleCommand>().To<ProceduralTracingCommand>();
            this.Bind<ConsoleCommand>().To<QuickAssetImportCommand>();

            this.Bind<IConfigurationHelper>().To<DefaultConfigurationHelper>();
            this.Bind<IProceduralConfiguration>().To<ActualProceduralConfiguration>();
            this.Bind<IProceduralConfiguration>().To<LandProceduralConfiguration>();
            this.Bind<IProceduralConfiguration>().To<ZoomProceduralConfiguration>();
            this.Bind<IProceduralConfiguration>().To<DoubleZoomProceduralConfiguration>();
            this.Bind<IProceduralConfiguration>().To<QuadZoomProceduralConfiguration>();
            this.Bind<IProceduralConfiguration>().To<ExtendProceduralConfiguration>();
            this.Bind<IProceduralConfiguration>().To<TestProceduralConfiguration>();
            this.Bind<IProceduralConfiguration>().To<HeightChangeProceduralConfiguration>();
            this.Bind<IProceduralConfiguration>().To<HeightChange3DProceduralConfiguration>();
        }
    }
}
