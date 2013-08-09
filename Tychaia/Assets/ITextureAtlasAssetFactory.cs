// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Protogame;

namespace Tychaia
{
    public interface ITextureAtlasAssetFactory
    {
        TextureAtlasAsset CreateTextureAtlasAsset(
            string name,
            GraphicsDevice graphicsDevice,
            IEnumerable<TextureAsset> textures);
    }
}
