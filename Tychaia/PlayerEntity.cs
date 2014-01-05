// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Protogame;
using Tychaia.Game;
using Tychaia.Globals;
using Tychaia.Network;

namespace Tychaia
{
    public class PlayerEntity : Entity, IHasRuntimeData<Player>
    {
        private readonly I3DRenderUtilities m_3DRenderUtilities;
        private readonly TextureAsset m_PlayerTexture;
        private readonly IChunkSizePolicy m_ChunkSizePolicy;
        private readonly IConsole m_Console;
        private readonly IFilteredFeatures m_FilteredFeatures;

        private readonly INetworkAPI m_NetworkAPI;

        private double m_DemoTicks = 0;

        private bool m_IgnoreGamePad = false;

        public PlayerEntity(
            IAssetManagerProvider assetManagerProvider,
            I3DRenderUtilities threedRenderUtilities,
            IChunkSizePolicy chunkSizePolicy,
            IConsole console,
            IFilteredFeatures filteredFeatures,
            INetworkAPI networkAPI,
            Player runtimeData)
        {
            this.m_FilteredFeatures = filteredFeatures;
            this.m_NetworkAPI = networkAPI;
            this.m_3DRenderUtilities = threedRenderUtilities;
            this.m_PlayerTexture = assetManagerProvider.GetAssetManager().Get<TextureAsset>("chars.player.Player");
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_Console = console;
            this.RuntimeData = runtimeData;

            this.Width = 16;
            this.Height = 16;
            this.Depth = 16;
        }
        
        public bool InaccurateY { get; set; }

        public float MovementSpeed
        {
            get { return 4; }
        }

        public Player RuntimeData
        {
            get;
            set;
        }

        public void MoveInDirection(IGameContext context, int directionInDegrees)
        {
            var input = new UserInput();
            input.SetAction(UserInputAction.Move);
            input.DirectionInDegrees = directionInDegrees;

            this.m_NetworkAPI.SendMessage("user input", InMemorySerializer.Serialize(input));
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (this.m_Console.Open)
                return;
            
            // Handle gamepad (if present).
            var mv = (float)Math.Sqrt(this.MovementSpeed);
            if (!this.m_IgnoreGamePad)
            {
                try
                {
                    var gpstate = GamePad.GetState(PlayerIndex.One);
            
                    if (gpstate.IsConnected)
                    {
                        if (gpstate.ThumbSticks.Left.LengthSquared() > 0.5)
                        {
                            var angle = Math.Atan2(
                                gpstate.ThumbSticks.Left.X,
                                gpstate.ThumbSticks.Left.Y);
                            this.MoveInDirection(gameContext, (int)MathHelper.ToDegrees((float)angle));
                        }
                    }
                }
                catch (DllNotFoundException)
                {
                    // The user might not have a version of DirectX that supports game pads.
                    this.m_IgnoreGamePad = true;
                }
            }
            
            if (this.m_FilteredFeatures.IsEnabled(Feature.DemoMovement))
            {
                var t = Math.Cos(this.m_DemoTicks++ / 10000);
                this.Z += (float)t * 4;
                this.X += (float)t * 4;
            }
            
            this.RuntimeData.X = this.X;
            this.RuntimeData.Y = this.Y;
            this.RuntimeData.Z = this.Z;
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
                return;
            
            var matrix = Matrix.CreateTranslation(-0.5f, 1, 0);
            matrix *= Matrix.CreateScale((float)Math.Sqrt(2f) / 2f, 1, 1);
            matrix *= Matrix.CreateRotationY(MathHelper.PiOver4);
            matrix *= this.InaccurateY ? Matrix.CreateRotationY(MathHelper.PiOver4 * 0.75f) : Matrix.Identity;
            matrix *= Matrix.CreateTranslation(
                this.X / this.m_ChunkSizePolicy.CellVoxelWidth,
                this.Y / this.m_ChunkSizePolicy.CellVoxelHeight,
                this.Z / this.m_ChunkSizePolicy.CellVoxelDepth);
                
            this.m_3DRenderUtilities.RenderTexture(
                renderContext,
                matrix,
                this.m_PlayerTexture);
        }
    }
}
