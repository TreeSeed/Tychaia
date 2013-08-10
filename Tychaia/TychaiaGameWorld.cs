// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ninject;
using Protogame;
using Tychaia.Disk;
using Tychaia.Globals;

namespace Tychaia
{
    public class TychaiaGameWorld : IWorld
    {
        private readonly IFilteredFeatures m_FilteredFeatures;
        private I2DRenderUtilities m_2DRenderUtilities;
        private I3DRenderUtilities m_3DRenderUtilities;
        private IAssetManager m_AssetManager;
        private ChunkManagerEntity m_ChunkManagerEntity;
        private IChunkSizePolicy m_ChunkSizePolicy;

        private FontAsset m_DefaultFont;

        private ILevel m_DiskLevel;
        private IFilteredConsole m_FilteredConsole;
        private IKernel m_Kernel;
        private Player m_Player;

        public TychaiaGameWorld(
            IAssetManagerProvider assetManagerProvider,
            I2DRenderUtilities _2DRenderUtilities,
            I3DRenderUtilities _3DRenderUtilities,
            IFilteredFeatures filteredFeatures,
            IFilteredConsole filteredConsole,
            IChunkOctreeFactory chunkOctreeFactory,
            IChunkFactory chunkFactory,
            IIsometricCameraFactory isometricCameraFactory,
            IChunkSizePolicy chunkSizePolicy,
            IChunkManagerEntityFactory chunkManagerEntityFactory)
        {
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
            this.m_2DRenderUtilities = _2DRenderUtilities;
            this.m_3DRenderUtilities = _3DRenderUtilities;
            this.m_FilteredFeatures = filteredFeatures;
            this.m_FilteredConsole = filteredConsole;
            this.m_ChunkSizePolicy = chunkSizePolicy;

            this.m_DiskLevel = null;
            this.ChunkOctree = chunkOctreeFactory.CreateChunkOctree();
            var chunk = chunkFactory.CreateChunk(this.m_DiskLevel, this.ChunkOctree, 0, 0, 0);
            this.m_DefaultFont = this.m_AssetManager.Get<FontAsset>("font.Default");
            this.IsometricCamera = isometricCameraFactory.CreateIsometricCamera(this.ChunkOctree, chunk);
            this.m_ChunkManagerEntity = chunkManagerEntityFactory.CreateChunkManagerEntity(this);

            this.m_Player = new Player(
                this.m_FilteredFeatures);
            this.Entities = new List<IEntity> { this.m_ChunkManagerEntity, this.m_Player };
        }

        public ChunkOctree ChunkOctree { get; private set; }
        public IsometricCamera IsometricCamera { get; private set; }
        public List<IEntity> Entities { get; private set; }

        public void Dispose()
        {
        }

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
                return;

            if (this.m_FilteredFeatures.IsEnabled(Feature.RenderWireframe))
                renderContext.GraphicsDevice.RasterizerState = new RasterizerState { FillMode = FillMode.WireFrame };

            this.IsometricCamera.InitializeRenderContext(renderContext);

            renderContext.GraphicsDevice.Clear(Color.Black);
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            if (this.m_FilteredFeatures.IsEnabled(Feature.RenderWireframe))
                renderContext.GraphicsDevice.RasterizerState = new RasterizerState { FillMode = FillMode.Solid };
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            // Perform chunk provider / renderer updates.  Ideally we want to expose these
            // to the intelligence components, but not too sure how to do this yet.
            //this.m_ChunkProvider.Process(gameContext);
            //this.m_ChunkRenderer.Process(gameContext);

            var keyboard = Keyboard.GetState();

            // Handle escape.
            if (keyboard.IsKeyPressed(Keys.Escape))
            {
                gameContext.SwitchWorld<TitleWorld>();
            }

            // Focus the camera.
            //this.IsometricCamera.Focus(this.m_Player.X, this.m_Player.Y, this.m_Player.Z);

            //this.m_Player.Z = this.GetSurfaceZ(context, this.m_Player.X, this.m_Player.Y) * Scale.CUBE_Z;

            // Run the intelligence components.
            //foreach (var component in this.m_IntelligenceComponents)
            //   component.Update(gameContext, updateContext);
        }

        private float GetSurfaceZ(IGameContext context, float xx, float yy)
        {
            /*int x = (int)((xx < 0 ? xx + 1 : xx) % (Chunk.Width * Scale.CUBE_X) / Scale.CUBE_X);
            int y = (int)((yy < 0 ? yy + 1 : yy) % (Chunk.Height * Scale.CUBE_Y) / Scale.CUBE_Y);
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if ((context.WorldManager as IsometricWorldManager).Chunk == null)
                return 0;
            for (int z = 0; z < Chunk.Depth; z++)
                if ((context.WorldManager as IsometricWorldManager).Chunk.m_Blocks[x, y, z] != null)
                    return z - 1;*/
            return 0;
        }
    }
}