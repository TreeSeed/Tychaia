// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Ninject.Extensions.Factory;
using Ninject.Modules;
using Protogame;
using Tychaia.Client;
using Tychaia.Runtime;

namespace Tychaia
{
    public class TychaiaIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IBackgroundCubeEntityFactory>().ToFactory();
            this.Bind<ISkin>().To<TychaiaSkin>();
            this.Bind<IChunkManagerEntityFactory>().ToFactory();
            this.Bind<ITextureAtlasAssetFactory>().To<DefaultTextureAtlasAssetFactory>();
            this.Bind<ICommand>().To<CameraCommand>();
            this.Bind<IChunkAI>().To<PredeterminedChunkGeneratorAI>();
            this.Bind<IChunkAI>().To<PredeterminedChunkRenderPickerAI>();
            this.Bind<ICommand>().To<ChunkAICommand>();
            this.Bind<IWorldFactory>().ToFactory();
            this.Bind<ICommand>().To<ProfilingCommand>();
            this.Bind<IGameUIFactory>().ToFactory();
            this.Bind<IViewportMode>().To<DefaultViewportMode>().InSingletonScope();
            this.Bind<IEventBinder<IGameContext>>().To<TychaiaDefaultDesktopBinder>();
            this.Bind<ICommand>().To<GiveCommand>();
            this.Bind<ICommand>().To<EquipCommand>();
            this.Bind<ICommand>().To<StatsCommand>();
            this.Bind<ICommand>().To<ServerCommand>();
            this.Bind<ICommand>().To<SaveCommand>();
            this.Bind<ICommand>().To<ShaderCommand>();
            this.Bind<ICommand>().To<NameCommand>();
            this.Bind<IBasicSkin>().To<TychaiaBasicSkin>();
            this.Bind<ICommand>().To<ReportCommand>();
            this.Bind<ICaptureService>().To<DefaultCaptureService>().InSingletonScope();
            this.Bind<IEntityFactory>().ToFactory();
            this.Bind<IDebugCubeRenderer>().To<DefaultDebugCubeRenderer>();
            this.Bind<IClientChunkFactory>().To<DefaultClientChunkFactory>();
            this.Bind<IChunkRenderer>().To<DefaultChunkRenderer>();
        }
    }
}