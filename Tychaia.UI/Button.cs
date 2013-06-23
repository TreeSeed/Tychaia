//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Protogame;

namespace Tychaia.UI
{
    public class Button : IContainer
    {
        public IContainer[] Children { get { return new IContainer[0]; } }
        public IContainer Parent { get; set; }
        public ButtonState State { get; private set; }
        public string Text { get; set; }
        public event EventHandler Click;

        public Button()
        {
            this.State = ButtonState.None;
        }

        public void Update(Rectangle layout)
        {
            var mouse = Mouse.GetState();
            if (layout.Contains(mouse.X, mouse.Y))
            {
                if (mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    if (this.Click != null && this.State != ButtonState.Clicked)
                        this.Click(this, new EventArgs());
                    this.State = ButtonState.Clicked;
                }
                else
                    this.State = ButtonState.Hover;
            }
            else
                this.State = ButtonState.None;
        }

        public void Draw(XnaGraphics graphics, ISkin skin, Rectangle layout)
        {
            skin.DrawButton(graphics, layout, this);
        }
    }
}

