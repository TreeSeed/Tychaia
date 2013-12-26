// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Protogame;
using Tychaia.Globals;

namespace Tychaia
{
    [NoProfile]
    public interface ICaptureService
    {
        void RenderBelow(ICoreGame coreGame);

        void RenderAbove(ICoreGame coreGame);

        void Render2D(ICoreGame coreGame);

        void Update(ICoreGame coreGame);

        void CaptureFrame(IGameContext gameContext, Action<byte[]> action);
    }
}
