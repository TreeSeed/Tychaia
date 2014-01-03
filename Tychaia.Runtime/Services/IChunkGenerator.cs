// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;

namespace Tychaia.Runtime
{
    public interface IChunkGenerator
    {
        void Generate(IChunk chunk, Action callback = null);

        void InputConnect();

        void InputDisconnect();
    }
}