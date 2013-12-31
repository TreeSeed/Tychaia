// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Protogame;
using Tychaia.Game;
using Tychaia.Globals;

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

        private readonly IFilteredFeatures m_FilteredFeatures;

        private readonly InventoryUIEntity m_InventoryUIEntity;

        private readonly PlayerEntity m_Player;

        private readonly IProfiler m_Profiler;

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
            byte[] initialState, 
            Action cleanup)
        {
            this.m_3DRenderUtilities = threedRenderUtilities;
            this.m_FilteredFeatures = filteredFeatures;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_Profiler = profiler;
            this.m_Console = console;
            this.m_AssetManagerProvider = assetManagerProvider;
            this.m_Cleanup = cleanup;
            this.Level = levelAPI.NewLevel("test");

            this.ChunkOctree = chunkOctreeFactory.CreateChunkOctree();
            var chunk = chunkFactory.CreateChunk(this.Level, this.ChunkOctree, 0, 0, 0);
            this.IsometricCamera = isometricCameraFactory.CreateIsometricCamera(this.ChunkOctree, chunk);
            this.m_ChunkManagerEntity = chunkManagerEntityFactory.CreateChunkManagerEntity(this);

            /*
            // Deserialize the initial game state.
            var state = new InitialGameState();
            var serializer = new TychaiaDataSerializer();
            using (var memory = new MemoryStream(initialState))
            {
                if (serializer.Deserialize(memory, state, typeof(InitialGameState)) == null)
                {
                    throw new InvalidOperationException("invalid initial game state");
                }
            }
            
            // Load initial game state.
            if (state.EntityNames == null)
                state.EntityNames = new string[0];
            if (state.EntityTypes == null)
                state.EntityTypes = new string[0];
            if (state.EntityNames.Length != state.EntityTypes.Length)
            {
                throw new InvalidOperationException("game state not valid; arrays not same length");
            }
            
            for (var i = 0; i < state.EntityNames.Length; i++)
            {
                var name = state.EntityNames[i];
                var type = state.EntityTypes[i];
                
                // We have the name of our synchronised type (i.e. that sits inside
                // Tychaia.Game), but we need to create the client type.
                if (type == typeof(Player).AssemblyQualifiedName)
                {
                    var data = new Player();
                    
                    // TODO: If this player is for this client, then we should be authoritive.
                    var player = new PlayerEntity(
                        this.m_AssetManagerProvider,
                        this.m_3DRenderUtilities,
                        this.m_ChunkSizePolicy,
                        this.m_Console,
                        this.m_FilteredFeatures,
                        data);
                    this.m_Player = player;
                }
            }
            */
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
        }
    }
}