//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia.UI
{
    public class SingleContainer : IContainer
    {
        private IContainer m_Child;

        public IContainer[] Children
        {
            get
            {
                return new[] { this.m_Child };
            }
        }

        public IContainer Parent { get; set; }
        public int Order { get; set; }
        public bool Focused { get; set; }

        public void SetChild(IContainer child)
        {
            if (child == null)
                throw new ArgumentNullException("child");
            if (child.Parent != null)
                throw new InvalidOperationException();
            this.m_Child = child;
            this.m_Child.Parent = this;
        }

        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            if (this.m_Child != null)
                this.m_Child.Update(skin, layout, gameTime, ref stealFocus);
        }

        public void Draw(XnaGraphics graphics, ISkin skin, Rectangle layout)
        {
            skin.DrawSingleContainer(graphics, layout, this);
            if (this.m_Child != null)
                this.m_Child.Draw(graphics, skin, layout);
        }
    }
}

