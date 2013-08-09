// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
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

        public Player(
            IFilteredFeatures filteredFeatures)
        {
            this.m_FilteredFeatures = filteredFeatures;

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
            // Update player and refocus screen.
            var state = Keyboard.GetState();
            var gpstate = GamePad.GetState(PlayerIndex.One);
            var mv = (float) Math.Sqrt(this.MovementSpeed);
            if (state.IsKeyDown(Keys.W))
            {
                this.Y -= mv;
                this.X -= mv;
            }
            if (state.IsKeyDown(Keys.S) || this.m_FilteredFeatures.IsEnabled(Feature.DebugMovement))
            {
                this.Y += mv;
                this.X += mv;
            }
            if (state.IsKeyDown(Keys.A))
            {
                this.Y += mv;
                this.X -= mv;
            }
            if (state.IsKeyDown(Keys.D))
            {
                this.Y -= mv;
                this.X += mv;
            }
            if (state.IsKeyDown(Keys.I))
            {
                this.Z += 4f;
            }
            if (state.IsKeyDown(Keys.K))
            {
                this.Z -= 4f;
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
            /*this.m_IsometricRenderingUtilities.RenderEntity(
                renderContext,
                this.m_World,
                this,
                this.m_World.IsometricCamera,
                this.m_World.OccludingSpriteBatch,
                this.m_PlayerAsset);
        
            this.m_RenderUtilities.RenderTexture(
                renderContext,
                new Vector2(this.X, this.Y),
                this.m_PlayerAsset);*/
        }
    }
}