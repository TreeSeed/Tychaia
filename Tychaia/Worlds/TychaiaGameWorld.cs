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
using Tychaia.Data;
using Tychaia.Game;
using Tychaia.Globals;
using Tychaia.Network;
using Tychaia.Runtime;

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

        private readonly IEntityFactory m_EntityFactory;

        private readonly IChunkConverter m_ChunkConverter;

        private readonly IChunkCompressor m_ChunkCompressor;

        private readonly IChunkGenerator m_ChunkGenerator;

        private readonly ITerrainSurfaceCalculator m_TerrainSurfaceCalculator;

        private readonly int m_UniqueClientIdentifier;

        private readonly IFilteredFeatures m_FilteredFeatures;

        private readonly InventoryUIEntity m_InventoryUIEntity;

        private readonly IProfiler m_Profiler;

        private readonly FontAsset m_DefaultFontAsset;
        
        private readonly IViewportMode m_ViewportMode;

        public TychaiaGameWorld(
            IAssetManagerProvider assetManagerProvider, 
            I3DRenderUtilities threedRenderUtilities, 
            IFilteredFeatures filteredFeatures, 
            IChunkOctreeFactory chunkOctreeFactory, 
            IIsometricCameraFactory isometricCameraFactory, 
            IChunkSizePolicy chunkSizePolicy, 
            IChunkManagerEntityFactory chunkManagerEntityFactory, 
            IProfiler profiler, 
            IConsole console, 
            ILevelAPI levelAPI /* temporary */, 
            IGameUIFactory gameUIFactory, 
            I2DRenderUtilities twodRenderUtilities,
            IClientNetworkAPI networkAPI,
            IEntityFactory entityFactory,
            IChunkConverter chunkConverter,
            IChunkCompressor chunkCompressor,
            IChunkGenerator chunkGenerator,
            ITerrainSurfaceCalculator terrainSurfaceCalculator,
            int uniqueClientIdentifier,
            Action cleanup,
            IViewportMode viewportMode)
        {
            this.m_3DRenderUtilities = threedRenderUtilities;
            this.m_FilteredFeatures = filteredFeatures;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_Profiler = profiler;
            this.m_Console = console;
            this.m_2DRenderUtilities = twodRenderUtilities;
            this.m_NetworkAPI = networkAPI;
            this.m_EntityFactory = entityFactory;
            this.m_ChunkConverter = chunkConverter;
            this.m_ChunkCompressor = chunkCompressor;
            this.m_ChunkGenerator = chunkGenerator;
            this.m_TerrainSurfaceCalculator = terrainSurfaceCalculator;
            this.m_UniqueClientIdentifier = uniqueClientIdentifier;
            this.m_AssetManagerProvider = assetManagerProvider;
            this.m_Cleanup = cleanup;
            this.Level = levelAPI.NewLevel("test");
            this.m_ViewportMode = viewportMode;

            this.m_DefaultFontAsset = this.m_AssetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");

            this.ChunkOctree = chunkOctreeFactory.CreateChunkOctree<ClientChunk>();
            var chunk = new ClientChunk(0, 0, 0);
            this.IsometricCamera = isometricCameraFactory.CreateIsometricCamera(this.ChunkOctree, chunk);
            this.m_ChunkManagerEntity = chunkManagerEntityFactory.CreateChunkManagerEntity(this);

            this.m_InventoryUIEntity = gameUIFactory.CreateInventoryUIEntity();
            this.Entities = new List<IEntity> { this.m_ChunkManagerEntity, this.m_InventoryUIEntity };

            // TODO: Map back to multiple player entities...
            this.m_NetworkAPI.ListenForMessage(
                "player update",
                (client, data) =>
                {
                    var playerState = InMemorySerializer.Deserialize<PlayerServerEntity.PlayerServerState>(data);

                    // Lookup the player entity for this unique client ID if we have one.
                    var player =
                        this.Entities.OfType<PlayerEntity>()
                            .FirstOrDefault(x => x.RuntimeData.UniqueClientIdentifier == playerState.UniqueClientID);

                    if (player == null)
                    {
                        // Need to create a new player entity.
                        player =
                            this.m_EntityFactory.CreatePlayerEntity(
                                new Player { UniqueClientIdentifier = playerState.UniqueClientID });
                        this.Entities.Add(player);
                    }

                    player.X = playerState.X;
                    player.Y = playerState.Y;
                    player.Z = playerState.Z;
                });

            // TODO: Move this somewhere better.
            this.m_NetworkAPI.ListenForMessage(
                "chunk available",
                (client, data) =>
                {
                    var dataChunk = this.m_ChunkCompressor.Decompress(data);

                    var clientChunk = this.ChunkOctree.Get(dataChunk.X, dataChunk.Y, dataChunk.Z);
                    if (clientChunk == null)
                    {
                        clientChunk = new ClientChunk(dataChunk.X, dataChunk.Y, dataChunk.Z);
                        this.ChunkOctree.Set(clientChunk);
                    }
                    else if (clientChunk.Generated)
                    {
                        // TODO: We already have this chunk.  The server shouldn't announce it to
                        // us because we've already had it sent before, but at the moment the server
                        // doesn't track this.  We just ignore it for now (so we don't recompute
                        // graphics data).
                        Console.WriteLine("Chunk is marked as generated, will not reload from server");
                        return;
                    }

                    this.m_ChunkConverter.FromChunk(dataChunk, clientChunk);

                    this.m_ChunkGenerator.Generate(clientChunk);
                });
        }

        public ChunkOctree<ClientChunk> ChunkOctree { get; private set; }

        public List<IEntity> Entities { get; private set; }

        public IsometricCamera<ClientChunk> IsometricCamera { get; private set; }

        public ILevel Level
        {
            get;
            private set;
        }

        private PlayerEntity LocalPlayer
        {
            get
            {
                return
                    this.Entities.OfType<PlayerEntity>()
                        .FirstOrDefault(x => x.RuntimeData.UniqueClientIdentifier == this.m_UniqueClientIdentifier);
            }
        }

        public void Dispose()
        {
            this.m_NetworkAPI.StopListeningForMessage("player update");
            this.m_ViewportMode.SetViewportMode(ViewportMode.Full);

            if (this.m_Cleanup != null)
            {
                this.m_Cleanup();
            }
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            if (this.m_FilteredFeatures.IsEnabled(Feature.RenderWireframe))
            {
                renderContext.GraphicsDevice.RasterizerState = new RasterizerState { FillMode = FillMode.Solid };
            }

            if (!renderContext.Is3DContext)
            {
                this.m_2DRenderUtilities.RenderText(
                    renderContext,
                    new Vector2(20, 600),
                    "Unique ID: " + this.m_UniqueClientIdentifier,
                    this.m_DefaultFontAsset);

                if (this.m_NetworkAPI.IsPotentiallyDisconnecting)
                {
                    this.m_2DRenderUtilities.RenderText(
                        renderContext,
                        new Vector2(20, 620),
                        this.m_NetworkAPI.DisconnectingForSeconds.ToString("F2") + " secs disconnected",
                        this.m_DefaultFontAsset,
                        textColor: Color.Red);
                }

                var i = 0;
                foreach (var player in this.m_NetworkAPI.PlayersInGame)
                {
                    this.m_2DRenderUtilities.RenderText(
                        renderContext,
                        new Vector2(20, 640 + (i * 20)),
                        player,
                        this.m_DefaultFontAsset);
                    i++;
                }
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

            // Update the camera to focus on the local player.
            if (this.LocalPlayer != null)
            {
                this.UpdateCamera(gameContext);
            }

            this.m_NetworkAPI.Update();
        }

        private void UpdateCamera(IGameContext gameContext)
        {
            // Focus the camera.
            var current = this.IsometricCamera.CurrentFocus;
            if (Math.Abs(current.Y - this.LocalPlayer.Y) < 2)
            {
                this.IsometricCamera.Focus((long)this.LocalPlayer.X, (long)this.LocalPlayer.Y, (long)this.LocalPlayer.Z);
            }
            else
            {
                this.IsometricCamera.Focus(
                    (long)this.LocalPlayer.X,
                    (long)MathHelper.Lerp(current.Y, this.LocalPlayer.Y, 0.1f),
                    (long)this.LocalPlayer.Z);
            }

            if (this.IsometricCamera.Chunk.Generated)
            {
                var newY = this.m_TerrainSurfaceCalculator.GetSurfaceY(this.ChunkOctree, this.LocalPlayer.X, this.LocalPlayer.Z);
                this.LocalPlayer.InaccurateY = newY == null;
                if (newY != null)
                {
                    this.LocalPlayer.Y = newY.Value;
                }
            }
        }
    }
}