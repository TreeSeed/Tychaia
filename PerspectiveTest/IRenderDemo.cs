using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PerspectiveTest
{
    public interface IRenderDemo
    {
        void LoadContent(Game game);
        void Update(Game game);
        void Draw(Game game);
    }
}

