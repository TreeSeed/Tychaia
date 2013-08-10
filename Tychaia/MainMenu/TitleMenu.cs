// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public class TitleMenu : IContainer
    {
        private readonly List<IContainer> m_Children = new List<IContainer>();

        public IContainer[] Children
        {
            get { return this.m_Children.ToArray(); }
        }

        public IContainer Parent { get; set; }
        public int Order { get; set; }
        public bool Focused { get; set; }

        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            var buttonLayout = new Rectangle(layout.Center.X - 150, layout.Bottom - this.m_Children.Count * 45 - 30, 300,
                30);
            foreach (var button in this.m_Children)
            {
                button.Update(skin, buttonLayout, gameTime, ref stealFocus);
                buttonLayout.Y += 45;
            }
        }

        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            var buttonLayout = new Rectangle(layout.Center.X - 150, layout.Bottom - this.m_Children.Count * 45 - 30, 300,
                30);
            foreach (var button in this.m_Children)
            {
                button.Draw(context, skin, buttonLayout);
                buttonLayout.Y += 45;
            }
        }

        public void AddChild(string text, EventHandler handler)
        {
            var button = new Button { Text = text };
            button.Click += handler;
            this.m_Children.Add(button);
        }
    }
}