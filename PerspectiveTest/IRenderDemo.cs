// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework;

namespace PerspectiveTest
{
    public interface IRenderDemo
    {
        void LoadContent(Game game);
        void Update(Game game);
        void Draw(Game game);
    }
}
