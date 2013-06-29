using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class XnaGraphics
    {
        private GameContext m_Context = null;
        private Texture2D m_Pixel = null;

        public XnaGraphics(GameContext context)
        {
            this.m_Context = context;
            this.m_Pixel = new Texture2D(context.Graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            this.m_Pixel.SetData(new[] { Color.White });
        }

        public SpriteBatch SpriteBatch
        {
            get { return this.m_Context.SpriteBatch; }
        }

        // Copied from GameContext; TODO: unify this.
        public void EndSpriteBatch()
        {
            this.m_Context.SpriteBatch.End();
        }

        // Copied from GameContext; TODO: unify this.
        public void StartSpriteBatch()
        {
            this.m_Context.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                null,
                null,
                null,
                null,
                null,
                this.m_Context.Camera.GetTransformationMatrix());
        }

        public void DrawStringLeft(
            int x,
            int y,
            string text,
            string font = "Arial",
            bool centerVertical = false,
            Color? textColor = null,
            Color? shadowColor = null)
        {
            if (textColor == null) textColor = Color.White;
            if (shadowColor == null) shadowColor = Color.Black;
            if (string.IsNullOrEmpty(text))
                return;
            var size = this.m_Context.Fonts[font].MeasureString(text);
            var yy = centerVertical ? (int)(y - size.Y / 2) : y;
            this.m_Context.SpriteBatch.DrawString(
                this.m_Context.Fonts[font],
                text,
                new Vector2((int)(x + 1), (int)(yy + 1)),
                shadowColor.Value);
            this.m_Context.SpriteBatch.DrawString(
                this.m_Context.Fonts[font],
                text,
                new Vector2((int)x, (int)yy),
                textColor.Value);
        }

        public Vector2 MeasureString(string text, string font = "Arial")
        {
            return this.m_Context.Fonts[font].MeasureString(text ?? "");
        }

        public void DrawStringCentered(
            int x,
            int y,
            string text,
            string font = "Arial",
            bool centerVertical = false,
            bool drawShadow = true,
            Color? color = null,
            Color? shadowColor = null,
            SpriteBatch spriteBatch = null)
        {
            if (color == null) color = Color.White;
            if (shadowColor == null) shadowColor = Color.Black;
            if (spriteBatch == null) spriteBatch = this.m_Context.SpriteBatch;
            if (string.IsNullOrEmpty(text))
                return;
            Vector2 size = this.m_Context.Fonts[font].MeasureString(text);
            var xx = (int)(x - size.X / 2);
            var yy = centerVertical ? (int)(y - size.Y / 2) : y;
            if (drawShadow)
                spriteBatch.DrawString(
                    this.m_Context.Fonts[font],
                    text,
                    new Vector2(xx + 1, yy + 1),
                    shadowColor.Value);
            spriteBatch.DrawString(this.m_Context.Fonts[font], text, new Vector2(xx, yy), color.Value);
        }

        public void DrawSprite(int x, int y, string image)
        {
            this.DrawSprite(x, y, this.m_Context.Textures[image].Width, this.m_Context.Textures[image].Height, image, Color.White);
        }

        public void DrawSprite(int x, int y, int width, int height, string image)
        {
            this.DrawSprite(x, y, width, height, image, Color.White);
        }

        public void DrawSprite(int x, int y, int width, int height, string image, Color color)
        {
            this.DrawSprite(x, y, width, height, image, color, false);
        }

        public void DrawSprite(int x, int y, int width, int height, string image, Color color, bool flipX)
        {
            this.m_Context.SpriteBatch.Draw(
                this.m_Context.Textures[image],
                new Rectangle(x, y, width, height),
                null,
                color.ToPremultiplied(),
                0,
                new Vector2(0, 0),
                SpriteEffects.FlipHorizontally,
                0
                );
        }

        public void DrawLine(Vector2 a, Vector2 b, Color color, bool useDirect = false)
        {
            this.DrawLine(
                a.X,
                a.Y,
                b.X,
                b.Y,
                1.0f,
                color,
                useDirect);
        }

        public void DrawLine(float x1, float y1, float x2, float y2, float width, Color color, bool useDirect = false)
        {
            if (useDirect && width == 1f)
            {
                var primitive = new[]
                {
                    new VertexPositionColor(new Vector3(x1, y1, 0), color),
                    new VertexPositionColor(new Vector3(x2, y2, 0), color),
                };
                var indicies = new short[] { 0, 1 };
                this.m_Context.Graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    primitive,
                    0,
                    2,
                    indicies,
                    0,
                    1);
            }
            else
            {
                var angle = (float)Math.Atan2(y2 - y1, x2 - x1);
                var length = Vector2.Distance(new Vector2(x1, y1), new Vector2(x2, y2));

                this.m_Context.SpriteBatch.Draw(this.m_Pixel, new Vector2(x1 + 1, y1), null, color,
                           angle, Vector2.Zero, new Vector2(length, width),
                           SpriteEffects.None, 0);
            }
        }

        public void DrawRectangle(Rectangle rectangle, Color color, bool useDirect = false)
        {
            this.DrawRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color, useDirect);
        }

        public void DrawRectangle(Vector2 from, Vector2 to, Color color, bool useDirect = false)
        {
            this.DrawRectangle(from.X, from.Y, to.X - from.X, to.Y - from.Y, color, useDirect);
        }

        public void DrawRectangle(float x, float y, float width, float height, Color color, bool useDirect = false)
        {
            this.DrawLine(x, y, x + width, y, 1, color, useDirect);
            this.DrawLine(x, y, x, y + height, 1, color, useDirect);
            this.DrawLine(x, y + height, x + width, y + height, 1, color, useDirect);
            this.DrawLine(x + width, y, x + width, y + height, 1, color, useDirect);
        }

        public void FillRectangle(Rectangle rectangle, Color color, bool useDirect = false)
        {
            this.FillRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color, useDirect);
        }

        public void FillRectangle(Vector2 from, Vector2 to, Color color, bool useDirect = false)
        {
            this.FillRectangle(from.X, from.Y, to.X - from.X, to.Y - from.Y, color, useDirect);
        }

        public void FillRectangle(float x, float y, float width, float height, Color color, bool useDirect = false)
        {
            if (useDirect)
            {
                var primitive = new[]
                {
                    new VertexPositionColor(new Vector3(x, y, 0), color),
                    new VertexPositionColor(new Vector3(x + width, y, 0), color),
                    new VertexPositionColor(new Vector3(x + width, y + height, 0), color),
                    new VertexPositionColor(new Vector3(x, y + height, 0), color),
                    new VertexPositionColor(new Vector3(x, y, 0), color),
                };
                var indicies = new short[] { 0, 1, 2, 3, 4 };
                this.m_Context.Graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    primitive,
                    0,
                    5,
                    indicies,
                    0,
                    3);
            }
            else
            {
                this.m_Context.SpriteBatch.Draw(
                    this.m_Pixel,
                    new Vector2(x, y),
                    null,
                    color,
                    0,
                    Vector2.Zero,
                    new Vector2(width, height),
                    SpriteEffects.None, 0);
            }
        }

        public int SpriteHeight(string image)
        {
            return this.m_Context.Textures[image].Height;
        }

        public int SpriteWidth(string image)
        {
            return this.m_Context.Textures[image].Width;
        }
    }
}
