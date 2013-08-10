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
using System;

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
        private IProfiler m_Profiler;

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
            IChunkManagerEntityFactory chunkManagerEntityFactory,
            IProfiler profiler)
        {
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
            this.m_2DRenderUtilities = _2DRenderUtilities;
            this.m_3DRenderUtilities = _3DRenderUtilities;
            this.m_FilteredFeatures = filteredFeatures;
            this.m_FilteredConsole = filteredConsole;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_Profiler = profiler;

            this.m_DiskLevel = null;
            this.ChunkOctree = chunkOctreeFactory.CreateChunkOctree();
            var chunk = chunkFactory.CreateChunk(this.m_DiskLevel, this.ChunkOctree, 0, 0, 0);
            this.m_DefaultFont = this.m_AssetManager.Get<FontAsset>("font.Default");
            this.IsometricCamera = isometricCameraFactory.CreateIsometricCamera(this.ChunkOctree, chunk);
            this.m_ChunkManagerEntity = chunkManagerEntityFactory.CreateChunkManagerEntity(this);

            this.m_Player = new Player(
                this.m_FilteredFeatures,
                assetManagerProvider,
                this.m_3DRenderUtilities,
                this.m_ChunkSizePolicy);
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

            using (this.m_Profiler.Measure("tychaia-clear_and_vsync"))
            {
                renderContext.GraphicsDevice.Clear(Color.Black);
            }
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
            var current = this.IsometricCamera.CurrentFocus;
            if (Math.Abs(current.Y - this.m_Player.Y) < 2)
                this.IsometricCamera.Focus((long)this.m_Player.X, (long)this.m_Player.Y, (long)this.m_Player.Z);
            else
                this.IsometricCamera.Focus((long)this.m_Player.X, (long)MathHelper.Lerp(current.Y, this.m_Player.Y, 0.1f), (long)this.m_Player.Z);
                

            if (this.IsometricCamera.Chunk.Generated)
            {
                var newY = this.GetSurfaceY(gameContext, this.m_Player.X, this.m_Player.Z);
                this.m_Player.InaccurateY = (newY == null);
                if (newY != null)
                    this.m_Player.Y = newY.Value;
            }

            // Run the intelligence components.
            //foreach (var component in this.m_IntelligenceComponents)
            //   component.Update(gameContext, updateContext);
        }

        private float? GetSurfaceY(IGameContext context, float xx, float zz)
        {
            var ax = (int)(xx - this.IsometricCamera.Chunk.X) / this.m_ChunkSizePolicy.CellVoxelWidth;
            var az = (int)(zz - this.IsometricCamera.Chunk.Z) / this.m_ChunkSizePolicy.CellVoxelDepth;
            if (ax >= 0 && ax < this.m_ChunkSizePolicy.ChunkCellWidth &&
                az >= 0 && az < this.m_ChunkSizePolicy.ChunkCellDepth)
                return this.IsometricCamera.Chunk.Cells[ax, 0, az].Get("HeightMap") * this.m_ChunkSizePolicy.CellVoxelDepth;
            return null;
        }
    }
}