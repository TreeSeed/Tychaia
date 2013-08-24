// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
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
            this.Bind<IBackgroundCubeEntityFactory>().ToFactory();
            this.Bind<IChunkOctreeFactory>().ToFactory();
            this.Bind<IChunkFactory>().ToFactory();
            this.Bind<ISkin>().To<TychaiaSkin>();
            this.Bind<IRenderTargetFactory>().To<DefaultRenderTargetFactory>().InSingletonScope();
            this.Bind<IChunkManagerEntityFactory>().ToFactory();
            this.Bind<IChunkGenerator>().To<DefaultChunkGenerator>().InSingletonScope();
            this.Bind<ITextureAtlasAssetFactory>().To<DefaultTextureAtlasAssetFactory>();
            this.Bind<ICommand>().To<CameraCommand>();
            this.Bind<IChunkAI>().To<PredeterminedChunkGeneratorAI>();
            this.Bind<IChunkAI>().To<PredeterminedChunkRenderPickerAI>();
            this.Bind<IFrustumChunkCache>().To<DefaultFrustumChunkCache>();
            this.Bind<IDebugCubeRenderer>().To<DefaultDebugCubeRenderer>();
            this.Bind<ICommand>().To<ChunkAICommand>();
            this.Bind<IPredeterminedChunkPositions>().To<DefaultPredeterminedChunkPositions>();
            this.Bind<IWorldFactory>().ToFactory();
            this.Bind<IFlowBundleToCellConverter>().To<DefaultFlowBundleToCellConverter>();
        }
    }
}
