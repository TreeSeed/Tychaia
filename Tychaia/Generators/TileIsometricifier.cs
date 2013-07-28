//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;

namespace Tychaia
{
    public static class TileIsometricifier
    {
        public const int TILE_TOP_WIDTH = 32 * 2;
        public const int TILE_TOP_HEIGHT = 24 * 2;
        public const int TILE_SIDE_WIDTH = 16 * 2;
        public const int TILE_SIDE_HEIGHT = 48 * 2;
        public const int TILE_CUBE_HEIGHT = TileIsometricifier.TILE_SIDE_HEIGHT / 2 - 16;
        public const int CHUNK_TOP_WIDTH = TileIsometricifier.TILE_TOP_WIDTH * Chunk.Width;
        public const int CHUNK_TOP_HEIGHT = TileIsometricifier.TILE_TOP_HEIGHT * Chunk.Height;
        public const int SKEW_SCALE = 2;
        public const float SKEW_MAGIC = 1.4f;
        public const int CHUNK_HEIGHT_ALLOWANCE = 64;

        public static int TILE_LEFT = -23 * 2;
        public static int TILE_TOP = 10 * 2;

        #if NOT_MIGRATED
        public static void Isometricify(string name, IGameContext gameContext)
        {
            Texture2D original = gameContext.Textures[name];

            #region Top Tile Generation

            // First rotate.
            int rotSize = (int)Math.Sqrt(Math.Pow(original.Width, 2) + Math.Pow(original.Height, 2));
            RenderTarget2D rotatedTarget = RenderTargetFactory.Create(
                gameContext.Graphics.GraphicsDevice,
                31,
                31,
                true,
                gameContext.Graphics.GraphicsDevice.DisplayMode.Format,
                DepthFormat.Depth24);
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(rotatedTarget);
            gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
            gameContext.SpriteBatch.Begin();
            gameContext.SpriteBatch.Draw(
                original,
                new Rectangle(0, 0, rotSize, rotSize),
                null,
                Color.White,
                MathHelper.ToRadians(45),
                //new Vector2(TILE_LEFT, TILE_TOP),
                new Vector2(-8, 8),
                SpriteEffects.None,
                0);
            gameContext.SpriteBatch.End();
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);

            // Then squash.
            RenderTarget2D squashedTarget = RenderTargetFactory.Create(
                gameContext.Graphics.GraphicsDevice,
                TILE_TOP_WIDTH,
                TILE_TOP_HEIGHT,
                true,
                gameContext.Graphics.GraphicsDevice.DisplayMode.Format,
                DepthFormat.Depth24);
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(squashedTarget);
            gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
            gameContext.SpriteBatch.Begin();
            gameContext.SpriteBatch.Draw(
                rotatedTarget,
                new Rectangle(0, 0, TILE_TOP_WIDTH, TILE_TOP_HEIGHT),
                new Rectangle(0, 0, rotatedTarget.Width, rotatedTarget.Height),
                Color.White
            );
            gameContext.SpriteBatch.End();
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);

            #endregion

            #region Side Tile Generation

            // Skew with matrix.
            Matrix m = Matrix.Identity;
            m.M11 = 1.0f;
            m.M12 = 0.7f;
            m.M21 = 0.0f;
            m.M22 = 1.0f;
            RenderTarget2D shearedLeftTarget = RenderTargetFactory.Create(
                gameContext.Graphics.GraphicsDevice,
                TILE_SIDE_WIDTH,
                TILE_SIDE_HEIGHT,
                true,
                gameContext.Graphics.GraphicsDevice.DisplayMode.Format,
                DepthFormat.Depth24);
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(shearedLeftTarget);
            gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
            gameContext.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, m);
            gameContext.SpriteBatch.Draw(
                original,
                new Rectangle(0, 0,
                    original.Width * SKEW_SCALE, original.Height * SKEW_SCALE),
                null,
                new Color(63, 63, 63)
            );
            gameContext.SpriteBatch.End();
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);

            // Skew with matrix.
            m = Matrix.Identity;
            m.M11 = 1.0f;
            m.M12 = -0.7f;
            m.M21 = 0.0f;
            m.M22 = 1.0f;
            RenderTarget2D shearedRightTarget = RenderTargetFactory.Create(
                gameContext.Graphics.GraphicsDevice,
                TILE_SIDE_WIDTH,
                TILE_SIDE_HEIGHT,
                true,
                gameContext.Graphics.GraphicsDevice.DisplayMode.Format,
                DepthFormat.Depth24);
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(shearedRightTarget);
            gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
            gameContext.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, m);
            gameContext.SpriteBatch.Draw(
                original,
                new Rectangle(0, (int)(original.Height * SKEW_MAGIC),
                    original.Width * SKEW_SCALE, original.Height * SKEW_SCALE),
                null,
                new Color(127, 127, 127)
            );
            gameContext.SpriteBatch.End();
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);

            #endregion

            gameContext.Textures.Add(name + ".isometric.top", squashedTarget);
            gameContext.Textures.Add(name + ".isometric.sideL", shearedLeftTarget);
            gameContext.Textures.Add(name + ".isometric.sideR", shearedRightTarget);
        }
        #endif
    }
}
