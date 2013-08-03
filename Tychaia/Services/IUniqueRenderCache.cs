//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Microsoft.Xna.Framework.Graphics;

namespace Tychaia
{
    public interface IUniqueRenderCache
    {
        bool Has(int[] data);
        bool IsWaiting(int[] data);
        UniqueRender Store(int[] data, RenderTarget2D target, RenderTarget2D depth);
        void StoreWaiting(int[] data);
        UniqueRender Grab(int[] data);
        void Release(int[] data);
        void ReleaseWaiting(int[] data);
    }
}

