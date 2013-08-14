// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;
using BoundingBox = Protogame.BoundingBox;

namespace Tychaia
{
    /// <summary>
    /// This is an asset representing a generated texture atlas.
    /// </summary>
    public class TextureAtlasAsset : MarshalByRefObject, IAsset
    {
        public string Name { get; private set; }
        public TextureAsset TextureAtlas { get; private set; }
        public Dictionary<string, Rectangle> Mappings { get; private set; } 

        public TextureAtlasAsset(
            string name,
            Texture2D textureAtlas,
            Dictionary<string, Rectangle> mappings)
        {
            this.Name = name;
            this.TextureAtlas = new TextureAsset(textureAtlas);
            this.Mappings = mappings;
        }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(TextureAtlasAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to TextureAtlasAsset.");
        }

        public IBoundingBox GetUVBounds(string name)
        {
            var mapping = this.Mappings[name];
            return new BoundingBox
            {
                X = mapping.X / (float)this.TextureAtlas.Texture.Width,
                Y = mapping.Y / (float)this.TextureAtlas.Texture.Height,
                Width = mapping.Width / (float)this.TextureAtlas.Texture.Width,
                Height = mapping.Height / (float)this.TextureAtlas.Texture.Height,
            };
        }
    }
}
