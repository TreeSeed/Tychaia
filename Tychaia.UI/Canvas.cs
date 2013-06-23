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
    public class Canvas : IContainer
    {
        private IContainer m_Child;

        public IContainer[] Children
        {
            get
            {
                return new[] { this.m_Child };
            }
        }

        public IContainer Parent
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public void SetChild(IContainer child)
        {
            if (child == null)
                throw new ArgumentNullException("child");
            this.m_Child = child;
            this.m_Child.Parent = this;
        }

        public void Update(Rectangle layout)
        {
            if (this.m_Child != null)
                this.m_Child.Update(layout);
        }

        public void Draw(XnaGraphics graphics, ISkin skin, Rectangle layout)
        {
            skin.DrawCanvas(graphics, layout, this);
            if (this.m_Child != null)
                this.m_Child.Draw(graphics, skin, layout);
        }
    }
}

