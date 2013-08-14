// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Protogame;
using Tychaia.Globals;

namespace Tychaia
{
    public class Player : Entity
    {
        private readonly IFilteredFeatures m_FilteredFeatures;
        private readonly I3DRenderUtilities m_3DRenderUtilities;
        private readonly TextureAsset m_PlayerTexture;
        private readonly IChunkSizePolicy m_ChunkSizePolicy;
        private readonly IConsole m_Console;
        
        public bool InaccurateY { get; set; }

        public Player(
            IFilteredFeatures filteredFeatures,
            IAssetManagerProvider assetManagerProvider,
            I3DRenderUtilities _3DRenderUtilities,
            IChunkSizePolicy chunkSizePolicy,
            IConsole console)
        {
            this.m_FilteredFeatures = filteredFeatures;
            this.m_3DRenderUtilities = _3DRenderUtilities;
            this.m_PlayerTexture = assetManagerProvider.GetAssetManager().Get<TextureAsset>("chars.player.Player");
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_Console = console;

            this.Width = 16;
            this.Height = 16;
            this.Depth = 16;
        }

        public float MovementSpeed
        {
            get { return 4; }
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (this.m_Console.Open)
                return;

            // Update player and refocus screen.
            var state = Keyboard.GetState();
            var gpstate = GamePad.GetState(PlayerIndex.One);
            var mv = (float) Math.Sqrt(this.MovementSpeed);
            if (state.IsKeyDown(Keys.W))
            {
                this.Z -= mv;
                this.X -= mv;
            }
            if (state.IsKeyDown(Keys.S) || this.m_FilteredFeatures.IsEnabled(Feature.DebugMovement))
            {
                this.Z += mv;
                this.X += mv;
            }
            if (state.IsKeyDown(Keys.A))
            {
                this.Z += mv;
                this.X -= mv;
            }
            if (state.IsKeyDown(Keys.D))
            {
                this.Z -= mv;
                this.X += mv;
            }
            if (state.IsKeyDown(Keys.I))
            {
                this.Y += 4f;
            }
            if (state.IsKeyDown(Keys.K))
            {
                this.Y -= 4f;
            }
            var v = new Vector2(
                gpstate.ThumbSticks.Left.X,
                -gpstate.ThumbSticks.Left.Y
                );
            var m = Matrix.CreateRotationZ(MathHelper.ToRadians(-45));
            v = Vector2.Transform(v, m);
            this.X += v.X * mv * (float) (Math.Sqrt(2) / 1.0);
            this.Y += v.Y * mv * (float) (Math.Sqrt(2) / 1.0);
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
                return;
                
            this.m_3DRenderUtilities.RenderTexture(
                renderContext,
                Matrix.CreateTranslation(-0.5f, 1, 0) *
                Matrix.CreateScale((float)Math.Sqrt(2f) / 2f, 1, 1) *
                Matrix.CreateRotationY(MathHelper.PiOver4) *
                (this.InaccurateY ? Matrix.CreateRotationY(MathHelper.PiOver4 * 0.75f) : Matrix.Identity) *
                Matrix.CreateTranslation(
                    this.X / this.m_ChunkSizePolicy.CellVoxelWidth,
                    this.Y / this.m_ChunkSizePolicy.CellVoxelHeight,
                    this.Z / this.m_ChunkSizePolicy.CellVoxelDepth),
                this.m_PlayerTexture);
            /* Backface only
            this.m_3DRenderUtilities.RenderTexture(
                renderContext,
                Matrix.CreateRotationY(MathHelper.Pi) *
                Matrix.CreateRotationY(MathHelper.PiOver4) *
                Matrix.CreateTranslation(0.5f, 1, 0) *
                Matrix.CreateTranslation(
                    this.X / this.m_ChunkSizePolicy.CellVoxelWidth,
                    this.Y / this.m_ChunkSizePolicy.CellVoxelHeight,
                    this.Z / this.m_ChunkSizePolicy.CellVoxelDepth),
                this.m_PlayerTexture);
            */
        }
    }
}
