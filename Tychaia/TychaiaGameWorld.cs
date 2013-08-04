//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ninject;
using Protogame;
using Tychaia.Disk;
using Tychaia.Globals;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Tychaia
{
    public class TychaiaGameWorld : IWorld, IOccludingSpriteBatchContainer
    {
        private IKernel m_Kernel;
        private IAssetManager m_AssetManager;
        private IRenderUtilities m_RenderUtilities;
        private IFilteredFeatures m_FilteredFeatures;
        private IFilteredConsole m_FilteredConsole;
        private IIntelligenceComponent[] m_IntelligenceComponents;
        private IRelativeChunkRendering m_RelativeChunkRendering;
        private IIsometricRenderUtilities m_IsometricRenderUtilities;
        private IChunkSizePolicy m_ChunkSizePolicy;
        private IRenderingBuffers m_RenderingBuffers;
        
        private FontAsset m_DefaultFont;
    
        private Player m_Player = null;
        private ILevel m_DiskLevel = null;
        private ChunkRenderer m_ChunkRenderer = null;
        private ChunkProvider m_ChunkProvider = null;
        
        public List<IEntity> Entities { get; private set; }
        public ChunkOctree ChunkOctree { get; private set; }
        public IsometricCamera IsometricCamera { get; private set; }
        public OccludingSpriteBatch OccludingSpriteBatch { get; private set; }
        
        public TychaiaGameWorld(
            IKernel kernel,
            IAssetManagerProvider assetManagerProvider,
            IRenderUtilities renderUtilities,
            IFilteredFeatures filteredFeatures,
            IFilteredConsole filteredConsole,
            IIntelligenceComponent[] intelligenceComponents,
            IChunkOctreeFactory chunkOctreeFactory,
            IChunkFactory chunkFactory,
            IIsometricCameraFactory isometricCameraFactory,
            IRelativeChunkRendering relativeChunkRendering,
            IIsometricRenderUtilities isometricRenderingUtilities,
            IChunkProviderFactory chunkProviderFactory,
            IChunkRendererFactory chunkRendererFactory,
            IChunkSizePolicy chunkSizePolicy,
            IRenderingBuffers renderingBuffers)
        {
            this.m_Kernel = kernel;
            kernel.Rebind<IOccludingSpriteBatchContainer>().ToMethod(x => this);
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
            this.m_RenderUtilities = renderUtilities;
            this.m_FilteredFeatures = filteredFeatures;
            this.m_FilteredConsole = filteredConsole;
            this.m_IntelligenceComponents = intelligenceComponents;
            this.m_RelativeChunkRendering = relativeChunkRendering;
            this.m_IsometricRenderUtilities = isometricRenderingUtilities;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_RenderingBuffers = renderingBuffers;
         
            this.m_DiskLevel = null;
            this.ChunkOctree = chunkOctreeFactory.CreateChunkOctree();
            var chunk = chunkFactory.CreateChunk(this.m_DiskLevel, this.ChunkOctree, 0, 0, 0);
            this.m_DefaultFont = this.m_AssetManager.Get<FontAsset>("font.Default");
            this.IsometricCamera = isometricCameraFactory.CreateIsometricCamera(this.ChunkOctree, chunk);
            
            this.m_ChunkProvider = chunkProviderFactory.CreateChunkProvider();
            this.m_ChunkRenderer = chunkRendererFactory.CreateChunkRenderer();
        
            this.m_Player = new Player(
                this,
                this.m_AssetManager,
                this.m_RenderUtilities,
                this.m_FilteredFeatures,
                this.m_IsometricRenderUtilities);
            this.Entities = new List<IEntity>();
            this.Entities.Add(this.m_Player);
        }
        
        public void Dispose()
        {
            this.m_Kernel.Unbind<IOccludingSpriteBatchContainer>();
        }

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            #region IsometricWorldManager.PreBegin
        
            // Ensure we have an occluding sprite batch.
            if (this.m_FilteredFeatures.IsEnabled(Feature.IsometricOcclusion))
            {
                if (this.OccludingSpriteBatch == null)
                    this.OccludingSpriteBatch = new OccludingSpriteBatch(gameContext.Graphics.GraphicsDevice);
                this.OccludingSpriteBatch.Begin(true);
            }
            else
                gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.Black, 1f, 0);
            
            #endregion
            
            #region IsometricWorldManager.DrawTilesBelow
            
            // Determine our Z offset.
            int zlevel = 0;
            int zoffset = -(this.m_ChunkSizePolicy.ChunkCellDepth - 0) * this.m_ChunkSizePolicy.CellCubePixelHeight;

            // Get rendering information.
            //this.m_ChunkRenderer.ResetNeeded();
            var renders = this.m_RelativeChunkRendering.GetRelativeRenderInformation(gameContext, this.IsometricCamera.Chunk, this.IsometricCamera.CurrentFocus).ToArray();
            //this.m_ChunkRenderer.LastRenderedCountOnScreen = renders.Count();

            // Render chunks.
            if (this.m_FilteredFeatures.IsEnabled(Feature.DepthBuffer))
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(this.m_RenderingBuffers.ScreenBuffer);
            renderContext.SpriteBatch.Begin();
            foreach (var ri in renders)
            {
                if (ri.Target == this.IsometricCamera.Chunk)
                {
                    this.IsometricCamera.ChunkCenterX = ri.X + this.m_ChunkSizePolicy.CellTextureTopPixelWidth / 2;
                    this.IsometricCamera.ChunkCenterY = ri.Y;
                }
                Texture2D tex = ri.Target.Texture;
                //ChunkRenderer.MarkNeeded(ri.Target);
                if (tex != null)
                {
                    //ChunkRenderer.MarkUsed(ri.Target);
                    if (this.m_FilteredFeatures.IsEnabled(Feature.DepthBuffer))
                        renderContext.SpriteBatch.Draw(tex, new Vector2(ri.X, ri.Y + zoffset), Color.White);
                    else
                    {
                        if (this.m_FilteredFeatures.IsEnabled(Feature.RenderingBuffers))
                            gameContext.Graphics.GraphicsDevice.SetRenderTarget(this.m_RenderingBuffers.ScreenBuffer);
                        renderContext.SpriteBatch.Draw(tex, new Vector2(ri.X, ri.Y + zoffset), Color.White);
                    }
                    this.m_FilteredConsole.WriteLine(FilterCategory.RenderingActive, "Rendering chunk at " + ri.X + ", " + ri.Y + ".");
                }
                else
                    this.m_FilteredConsole.WriteLine(FilterCategory.Rendering, "No texture yet for chunk to render at " + ri.X + ", " + ri.Y + ".");
            }
            renderContext.SpriteBatch.End();

            // Render depth maps.
            if (this.m_FilteredFeatures.IsEnabled(Feature.DepthBuffer))
            {
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(this.m_RenderingBuffers.DepthBuffer);
                renderContext.SpriteBatch.Begin();
                foreach (RelativeRenderInformation ri in renders)
                {
                    Texture2D depth = ri.Target.DepthMap;
                    if (depth != null)
                    {
                        //ChunkRenderer.MarkUsed(ri.Target);
                        if (this.m_FilteredFeatures.IsEnabled(Feature.DepthBuffer))
                        {
                            renderContext.SpriteBatch.Draw(depth, new Vector2(ri.X, ri.Y + zoffset), Color.White);
                        }
                    }
                }
                renderContext.SpriteBatch.End();
            }

            // Finish drawing.
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);
            
            #endregion
            
            #region IsometricWorldManager.DrawTilesAbove
            
            // Draw the current rendering buffers.
            if (this.m_FilteredFeatures.IsEnabled(Feature.IsometricOcclusion))
            {
                if (this.OccludingSpriteBatch.DepthTexture != this.m_RenderingBuffers.DepthBuffer &&
                    this.m_RenderingBuffers.DepthBuffer != null)
                    this.OccludingSpriteBatch.DepthTexture = this.m_RenderingBuffers.DepthBuffer;
                if (this.m_RenderingBuffers.ScreenBuffer != null &&
                    this.m_FilteredFeatures.IsEnabled(Feature.RenderWorld))
                    this.OccludingSpriteBatch.DrawOccluding(this.m_RenderingBuffers.ScreenBuffer, Vector2.Zero, Color.White);
                this.OccludingSpriteBatch.End();
            }
            else
            {
                if (this.m_FilteredFeatures.IsEnabled(Feature.RenderWorld))
                {
                    if (this.m_FilteredFeatures.IsEnabled(Feature.RenderingBuffers))
                        renderContext.SpriteBatch.Draw(this.m_RenderingBuffers.ScreenBuffer, Vector2.Zero, Color.White);
                }
                //context.SpriteBatch.End();
            }
            
            #endregion
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            // This function is *NEVER* called by TychaiaWorldManager since the game world
            // owns all of the rendering logic in RenderBelow.
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            // Perform chunk provider / renderer updates.  Ideally we want to expose these
            // to the intelligence components, but not too sure how to do this yet.
            this.m_ChunkProvider.Process(gameContext);
            this.m_ChunkRenderer.Process(gameContext);
        
            var keyboard = Keyboard.GetState();
        
            // Handle escape.
            if (keyboard.IsKeyPressed(Keys.Escape))
            {
                gameContext.SwitchWorld<TitleWorld>();
                return;
            }
            
            // Focus the camera.
            this.IsometricCamera.Focus(this.m_Player.X, this.m_Player.Y, this.m_Player.Z);
            
            //this.m_Player.Z = this.GetSurfaceZ(context, this.m_Player.X, this.m_Player.Y) * Scale.CUBE_Z;
            
            // Run the intelligence components.
            foreach (var component in this.m_IntelligenceComponents)
                component.Update(gameContext, updateContext);
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

