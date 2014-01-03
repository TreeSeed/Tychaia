// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Ninject.Extensions.Factory;
using Ninject.Modules;

namespace Tychaia.Runtime
{
    public class TychaiaRuntimeIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IChunkGenerator>().To<ClientChunkGenerator>().InSingletonScope();
            this.Bind<IChunkCompressor>().To<DefaultChunkCompressor>();
            this.Bind<IEdgePointCalculator>().To<DefaultEdgePointCalculator>();

            this.Bind<IChunkOctreeFactory>().ToFactory();

            this.Bind<IPredeterminedChunkPositions>().To<DefaultPredeterminedChunkPositions>();

            this.Bind<IChunkConverter>().To<DefaultChunkConverter>();

            this.Bind<ILevelAPI>().To<CombinedLevelAPI>();
            this.Bind<ILevelAPIImpl>().To<TychaiaLevelAPIImpl>().Named("Default");
            this.Bind<ITychaiaLevelFactory>().ToFactory();

            this.Bind<ITerrainSurfaceCalculator>().To<DefaultTerrainSurfaceCalculator>();
        }
    }
}