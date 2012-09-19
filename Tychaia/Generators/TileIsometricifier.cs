using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Protogame;
using Microsoft.Xna.Framework;

namespace Tychaia.Generators
{
    public static class TileIsometricifier
    {
        public const int TILE_TOP_WIDTH = 32;
        public const int TILE_TOP_HEIGHT = 24;
        public const int TILE_SIDE_WIDTH = 16;
        public const int TILE_SIDE_HEIGHT = 48;
        public const int TILE_CUBE_HEIGHT = TileIsometricifier.TILE_SIDE_HEIGHT / 2 - 8;
        public const int CHUNK_TOP_WIDTH = TileIsometricifier.TILE_TOP_WIDTH * Chunk.Width;
        public const int CHUNK_TOP_HEIGHT = TileIsometricifier.TILE_TOP_HEIGHT * Chunk.Height;

        public static int TILE_LEFT = -23;
        public static int TILE_TOP = 10;

        public static void Isometricify(string name, GameContext context)
        {
            Texture2D original = context.Textures[name];

            #region Top Tile Generation

            // First rotate.
            int rotSize = (int)Math.Sqrt(Math.Pow(original.Width, 2) + Math.Pow(original.Height, 2));
            RenderTarget2D rotatedTarget = RenderTargetFactory.Create(
                context.Graphics.GraphicsDevice,
                31,
                31,
                true,
                context.Graphics.GraphicsDevice.DisplayMode.Format,
                DepthFormat.Depth24);
            context.Graphics.GraphicsDevice.SetRenderTarget(rotatedTarget);
            context.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
            context.SpriteBatch.Begin();
            context.SpriteBatch.Draw(
                original,
                new Rectangle(0, 0, rotSize, rotSize),
                new Rectangle(0, 0, 16, 16),
                Color.White,
                MathHelper.ToRadians(45),
                //new Vector2(TILE_LEFT, TILE_TOP),
                new Vector2(-8, 8),
                SpriteEffects.None,
                0);
            context.SpriteBatch.End();

            // Then squash.
            RenderTarget2D squashedTarget = RenderTargetFactory.Create(
                context.Graphics.GraphicsDevice,
                TILE_TOP_WIDTH,
                TILE_TOP_HEIGHT,
                true,
                context.Graphics.GraphicsDevice.DisplayMode.Format,
                DepthFormat.Depth24);
            context.Graphics.GraphicsDevice.SetRenderTarget(squashedTarget);
            context.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
            context.SpriteBatch.Begin();
            context.SpriteBatch.Draw(
                rotatedTarget,
                new Rectangle(0, 0, TILE_TOP_WIDTH, TILE_TOP_HEIGHT),
                new Rectangle(0, 0, rotatedTarget.Width, rotatedTarget.Height),
                Color.White
                );
            context.SpriteBatch.End();
            context.Graphics.GraphicsDevice.SetRenderTarget(null);

            #endregion

            #region Side Tile Generation

            // Skew with matrix.
            Matrix m = Matrix.Identity;
            m.M11 = 1.0f; m.M12 = 0.7f;
            m.M21 = 0.0f; m.M22 = 1.0f;
            RenderTarget2D shearedLeftTarget = RenderTargetFactory.Create(
                context.Graphics.GraphicsDevice,
                TILE_SIDE_WIDTH,
                TILE_SIDE_HEIGHT,
                true,
                context.Graphics.GraphicsDevice.DisplayMode.Format,
                DepthFormat.Depth24);
            context.Graphics.GraphicsDevice.SetRenderTarget(shearedLeftTarget);
            context.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
            context.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, m);
            context.SpriteBatch.Draw(
                original,
                new Rectangle(0, 0, original.Width, original.Height),
                null,
                new Color(63, 63, 63)
                );
            context.SpriteBatch.End();
            context.Graphics.GraphicsDevice.SetRenderTarget(null);

            // Skew with matrix.
            m = Matrix.Identity;
            m.M11 = 1.0f; m.M12 = -0.7f;
            m.M21 = 0.0f; m.M22 = 1.0f;
            RenderTarget2D shearedRightTarget = RenderTargetFactory.Create(
                context.Graphics.GraphicsDevice,
                TILE_SIDE_WIDTH,
                TILE_SIDE_HEIGHT,
                true,
                context.Graphics.GraphicsDevice.DisplayMode.Format,
                DepthFormat.Depth24);
            context.Graphics.GraphicsDevice.SetRenderTarget(shearedRightTarget);
            context.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
            context.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, m);
            context.SpriteBatch.Draw(
                original,
                new Rectangle(0, (int)(original.Height * 0.7), original.Width, original.Height),
                null,
                new Color(127, 127, 127)
                );
            context.SpriteBatch.End();
            context.Graphics.GraphicsDevice.SetRenderTarget(null);

            #endregion

            context.Textures.Add(name + ".isometric.top", squashedTarget);
            context.Textures.Add(name + ".isometric.sideL", shearedLeftTarget);
            context.Textures.Add(name + ".isometric.sideR", shearedRightTarget);
        }
    }
}
