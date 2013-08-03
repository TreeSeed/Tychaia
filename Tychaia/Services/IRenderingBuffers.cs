//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Microsoft.Xna.Framework.Graphics;
using Protogame;

namespace Tychaia
{
    public interface IRenderingBuffers
    {
        RenderTarget2D ScreenBuffer { get; }
        RenderTarget2D DepthBuffer { get; }
        
        void Initialize(IGameContext gameContext);
    }
}

