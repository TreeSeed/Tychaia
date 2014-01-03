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
using Protogame;
using Tychaia.Asset;

namespace Tychaia
{
    public class DefaultTextureAtlasAssetFactory : ITextureAtlasAssetFactory
    {
        public TextureAtlasAsset CreateTextureAtlasAsset(
            string name,
            GraphicsDevice graphicsDevice,
            IEnumerable<TextureAsset> textures)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (graphicsDevice == null) throw new ArgumentNullException("graphicsDevice");
            if (textures == null) throw new ArgumentNullException("textures");

            var textureArray = textures.ToArray();
            foreach (var texture in textureArray)
            {
                if (texture.Texture.Width != 16 ||
                    texture.Texture.Height != 16)
                {
                    throw new InvalidOperationException("Texture atlas can only support textures 16x16.");
                }
            }

            var size = this.CalculateSizeForTextures(textureArray);

            var mappings = new Dictionary<string, Rectangle>();
            var renderTarget = new RenderTarget2D(graphicsDevice, (int)size.X, (int)size.Y);

            try
            {
                var x = 0;
                var y = 0;
                graphicsDevice.SetRenderTarget(renderTarget);
                graphicsDevice.Clear(Color.Transparent);
                
                using (var spriteBatch = new SpriteBatch(graphicsDevice))
                {
                    spriteBatch.Begin();
                    
                    foreach (var texture in textureArray)
                    {
                        spriteBatch.Draw(texture.Texture, new Vector2(x, y));
                        mappings.Add(texture.Name, new Rectangle(x, y, 16, 16));
                        x += 16;
                        if (x >= size.X)
                        {
                            x = 0;
                            y += 16;
                        }
                    }
                    
                    spriteBatch.End();
                }
            }
            catch (InvalidOperationException)
            {
            }
            
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            return new TextureAtlasAsset(
                name,
                renderTarget,
                mappings);
        }

        private Vector2 CalculateSizeForTextures(TextureAsset[] textures)
        {
            // TODO: Texture atlas can only handle textures of 16 pixels high and wide.
            int size = 16;
            int count = (int)Math.Ceiling(Math.Sqrt(textures.Length));
            return new Vector2(size * count, size * count);
        }
    }
}
