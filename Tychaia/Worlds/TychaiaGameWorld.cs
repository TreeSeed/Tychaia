// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Protogame;
using Tychaia.Game;
using Tychaia.Globals;
using Tychaia.Network;

namespace Tychaia
{
    public class TychaiaGameWorld : IWorld
    {
        private readonly I3DRenderUtilities m_3DRenderUtilities;

        private readonly IAssetManagerProvider m_AssetManagerProvider;

        private readonly ChunkManagerEntity m_ChunkManagerEntity;

        private readonly IChunkSizePolicy m_ChunkSizePolicy;

        private readonly Action m_Cleanup;

        private readonly IConsole m_Console;

        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly IClientNetworkAPI m_NetworkAPI;

        private readonly IFilteredFeatures m_FilteredFeatures;

        private readonly InventoryUIEntity m_InventoryUIEntity;

        private readonly PlayerEntity m_Player;

        private readonly IProfiler m_Profiler;

        private readonly FontAsset m_DefaultFontAsset;

        public TychaiaGameWorld(
            IAssetManagerProvider assetManagerProvider, 
            I3DRenderUtilities threedRenderUtilities, 
            IFilteredFeatures filteredFeatures, 
            IChunkOctreeFactory chunkOctreeFactory, 
            IChunkFactory chunkFactory, 
            IIsometricCameraFactory isometricCameraFactory, 
            IChunkSizePolicy chunkSizePolicy, 
            IChunkManagerEntityFactory chunkManagerEntityFactory, 
            IProfiler profiler, 
            IConsole console, 
            ILevelAPI levelAPI /* temporary */, 
            IGameUIFactory gameUIFactory, 
            I2DRenderUtilities twodRenderUtilities,
            IClientNetworkAPI networkAPI,
            byte[] initialState, 
            Action cleanup)
        {
            this.m_3DRenderUtilities = threedRenderUtilities;
            this.m_FilteredFeatures = filteredFeatures;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_Profiler = profiler;
            this.m_Console = console;
            this.m_2DRenderUtilities = twodRenderUtilities;
            this.m_NetworkAPI = networkAPI;
            this.m_AssetManagerProvider = assetManagerProvider;
            this.m_Cleanup = cleanup;
            this.Level = levelAPI.NewLevel("test");

            this.m_DefaultFontAsset = this.m_AssetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");

            this.ChunkOctree = chunkOctreeFactory.CreateChunkOctree();
            var chunk = chunkFactory.CreateChunk(this.Level, this.ChunkOctree, 0, 0, 0);
            this.IsometricCamera = isometricCameraFactory.CreateIsometricCamera(this.ChunkOctree, chunk);
            this.m_ChunkManagerEntity = chunkManagerEntityFactory.CreateChunkManagerEntity(this);

            var player = new PlayerEntity(
                this.m_AssetManagerProvider, 
                this.m_3DRenderUtilities, 
                this.m_ChunkSizePolicy, 
                this.m_Console, 
                this.m_FilteredFeatures, 
                new Player());
            this.m_Player = player;

            this.m_InventoryUIEntity = gameUIFactory.CreateInventoryUIEntity();
            this.Entities = new List<IEntity> { this.m_ChunkManagerEntity, this.m_InventoryUIEntity };
            if (this.m_Player != null)
            {
                this.Entities.Add(this.m_Player);
            }
        }

        public ChunkOctree ChunkOctree { get; private set; }

        public List<IEntity> Entities { get; private set; }

        public IsometricCamera IsometricCamera { get; private set; }

        public ILevel Level { get; private set; }

        public void Dispose()
        {
            if (this.m_Cleanup != null)
            {
                this.m_Cleanup();
            }
        }

        /// <summary>
        /// Returns the surface Y based on the chunk that is currently active in the camera.  Will
        /// return null if you attempt to determine the height of a location that is not within
        /// the available area.
        /// </summary>
        public float? GetSurfaceY(IGameContext context, float xx, float zz)
        {
            var ax = (int)(xx - this.IsometricCamera.Chunk.X) / this.m_ChunkSizePolicy.CellVoxelWidth;
            var az = (int)(zz - this.IsometricCamera.Chunk.Z) / this.m_ChunkSizePolicy.CellVoxelDepth;

            if (ax >= 0 && ax < this.m_ChunkSizePolicy.ChunkCellWidth && az >= 0
                && az < this.m_ChunkSizePolicy.ChunkCellDepth)
            {
                return this.IsometricCamera.Chunk.Cells[ax, 0, az].HeightMap * this.m_ChunkSizePolicy.CellVoxelDepth;
            }

            if (ax >= this.m_ChunkSizePolicy.ChunkCellWidth && ax < this.m_ChunkSizePolicy.ChunkCellWidth * 2 && az >= 0
                && az < this.m_ChunkSizePolicy.ChunkCellDepth)
            {
                return
                    this.IsometricCamera.Chunk.East.Cells[ax - this.m_ChunkSizePolicy.ChunkCellWidth, 0, az].HeightMap
                    * this.m_ChunkSizePolicy.CellVoxelDepth;
            }

            if (ax >= 0 && ax < this.m_ChunkSizePolicy.ChunkCellWidth && az >= this.m_ChunkSizePolicy.ChunkCellWidth
                && az < this.m_ChunkSizePolicy.ChunkCellDepth * 2)
            {
                return
                    this.IsometricCamera.Chunk.South.Cells[ax, 0, az - this.m_ChunkSizePolicy.ChunkCellWidth].HeightMap
                    * this.m_ChunkSizePolicy.CellVoxelDepth;
            }

            if (ax >= this.m_ChunkSizePolicy.ChunkCellWidth && ax < this.m_ChunkSizePolicy.ChunkCellWidth * 2
                && az >= this.m_ChunkSizePolicy.ChunkCellWidth && az < this.m_ChunkSizePolicy.ChunkCellDepth * 2)
            {
                return
                    this.IsometricCamera.Chunk.South.East.Cells[ax - this.m_ChunkSizePolicy.ChunkCellWidth, 0, az - this.m_ChunkSizePolicy.ChunkCellWidth].HeightMap * this.m_ChunkSizePolicy.CellVoxelDepth;
            }

            return null;
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            if (this.m_FilteredFeatures.IsEnabled(Feature.RenderWireframe))
            {
                renderContext.GraphicsDevice.RasterizerState = new RasterizerState { FillMode = FillMode.Solid };
            }

            if (!renderContext.Is3DContext)
            {
                if (this.m_NetworkAPI.IsPotentiallyDisconnecting)
                {
                    this.m_2DRenderUtilities.RenderText(
                        renderContext,
                        new Vector2(20, 600),
                        this.m_NetworkAPI.DisconnectingForSeconds.ToString("F2") + " secs disconnected",
                        this.m_DefaultFontAsset,
                        textColor: Color.Red);
                }

                this.m_2DRenderUtilities.RenderText(
                    renderContext,
                    new Vector2(20, 620),
                    this.m_NetworkAPI.PlayersInGame.Aggregate(string.Empty, (a, b) => a + " " + b).Trim(),
                    this.m_DefaultFontAsset);
            }
        }

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
            {
                return;
            }

            if (this.m_FilteredFeatures.IsEnabled(Feature.RenderWireframe))
            {
                renderContext.GraphicsDevice.RasterizerState = new RasterizerState { FillMode = FillMode.WireFrame };
            }

            this.IsometricCamera.InitializeRenderContext(renderContext);

            using (this.m_Profiler.Measure("tychaia-clear_and_vsync"))
            {
                renderContext.GraphicsDevice.Clear(Color.Black);
            }
        }

        public string SendInternalServerMessage(string message)
        {
            return string.Empty;
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            var keyboard = Keyboard.GetState();

            // Handle escape.
            if (keyboard.IsKeyPressed(this, Keys.Escape))
            {
                gameContext.SwitchWorld<TitleWorld>();
            }

            // Handle disconnection.
            if (this.m_NetworkAPI.IsDisconnected)
            {
                gameContext.SwitchWorld<TitleWorld>();
                return;
            }

            if (this.m_Player == null)
            {
                return;
            }

            // Focus the camera.
            var current = this.IsometricCamera.CurrentFocus;
            if (Math.Abs(current.Y - this.m_Player.Y) < 2)
            {
                this.IsometricCamera.Focus((long)this.m_Player.X, (long)this.m_Player.Y, (long)this.m_Player.Z);
            }
            else
            {
                this.IsometricCamera.Focus(
                    (long)this.m_Player.X, 
                    (long)MathHelper.Lerp(current.Y, this.m_Player.Y, 0.1f), 
                    (long)this.m_Player.Z);
            }

            if (this.IsometricCamera.Chunk.Generated)
            {
                var newY = this.GetSurfaceY(gameContext, this.m_Player.X, this.m_Player.Z);
                this.m_Player.InaccurateY = newY == null;
                if (newY != null)
                {
                    this.m_Player.Y = newY.Value;
                }
            }

            this.m_NetworkAPI.Update();
        }
    }
}