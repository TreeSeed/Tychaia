//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Protogame;

namespace Tychaia.UI
{
    public class CanvasEntity : Entity
    {
        private ISkin m_Skin;
        public Canvas Canvas { get; set; }

        public CanvasEntity(ISkin skin)
        {
            if (skin == null)
                throw new ArgumentNullException("skin");
            this.m_Skin = skin;
        }

        public CanvasEntity(ISkin skin, Canvas canvas)
            : this(skin)
        {
            this.Canvas = canvas;
        }

        public override void Update(World world)
        {
            base.Update(world);

            if (this.Canvas != null)
            {
                var stealFocus = false;
                this.Canvas.Update(
                    this.m_Skin,
                    world.GameContext.Window.ClientBounds,
                    world.GameContext.GameTime,
                    ref stealFocus);
            }
        }

        public override void Draw(World world, XnaGraphics graphics)
        {
            base.Draw(world, graphics);

            if (this.Canvas != null)
                this.Canvas.Draw(graphics, this.m_Skin, world.GameContext.Window.ClientBounds);
        }
    }
}

