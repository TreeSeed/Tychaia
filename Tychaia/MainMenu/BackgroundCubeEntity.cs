//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public class BackgroundCubeEntity : Entity
    {
        private int m_Distance;
        private static Random m_Random = new Random();
        private double m_ScreenX;
        private double m_ScreenY;
        private IRenderUtilities m_RenderUtilities;
        private IsometricCubeAsset m_CubeAsset;

        public BackgroundCubeEntity(
            IRenderUtilities renderUtilities,
            IAssetManager assetManager,
            bool atTop = false)
        {
            this.m_RenderUtilities = renderUtilities;
            this.m_ScreenX = m_Random.NextDouble();
            this.m_ScreenY = atTop ? 1.0 : m_Random.NextDouble();
            this.m_Distance = m_Random.Next(1, 50);
            this.m_CubeAsset = assetManager.Get<IsometricCubeAsset>("tiles.Grass");
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.m_ScreenY -= (100.0f / m_Distance) / 5000.0f;
            this.X = (float)(this.m_ScreenX * gameContext.Window.ClientBounds.Width);
            this.Y = (float)(this.m_ScreenY * gameContext.Window.ClientBounds.Height);

            if ((int)this.Y + (int)(TileIsometricifier.TILE_TOP_HEIGHT / this.m_Distance) +
                (int)(TileIsometricifier.TILE_SIDE_HEIGHT * 2.0 / this.m_Distance) < 0)
                gameContext.World.Entities.Remove(this);

            base.Update(gameContext, updateContext);
        }
        
        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            this.m_RenderUtilities.RenderTexture(
                renderContext,
                new Vector2(
                    (int)this.X,
                    (int)this.Y),
                this.m_CubeAsset.TopTexture,
                size: new Vector2(
                    (int)(TileIsometricifier.TILE_TOP_WIDTH * 2.0 / this.m_Distance),
                    (int)(TileIsometricifier.TILE_TOP_HEIGHT * 2.0 / this.m_Distance)),
                color: new Color(1.0f, 1.0f, 1.0f, 1.0f - (m_Distance / 100.0f)));
            this.m_RenderUtilities.RenderTexture(
                renderContext,
                new Vector2(
                    (int)this.X + (int)(TileIsometricifier.TILE_SIDE_WIDTH * 2.0 / this.m_Distance),
                    (int)this.Y + (int)(TileIsometricifier.TILE_TOP_HEIGHT / this.m_Distance)),
                this.m_CubeAsset.LeftTexture,
                size: new Vector2(
                    (int)(TileIsometricifier.TILE_SIDE_WIDTH * 2.0 / this.m_Distance),
                    (int)(TileIsometricifier.TILE_SIDE_HEIGHT * 2.0 / this.m_Distance)),
                color: new Color(1.0f, 1.0f, 1.0f, 1.0f - (m_Distance / 100.0f)));
            this.m_RenderUtilities.RenderTexture(
                renderContext,
                new Vector2(
                    (int)this.X,
                    (int)this.Y + (int)(TileIsometricifier.TILE_TOP_HEIGHT / this.m_Distance)),
                this.m_CubeAsset.RightTexture,
                size: new Vector2(
                    (int)(TileIsometricifier.TILE_SIDE_WIDTH * 2.0 / this.m_Distance),
                    (int)(TileIsometricifier.TILE_SIDE_HEIGHT * 2.0 / this.m_Distance)),
                color: new Color(1.0f, 1.0f, 1.0f, 1.0f - (m_Distance / 100.0f)));
        }
    }
}

