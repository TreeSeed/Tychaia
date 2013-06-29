//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Protogame;
using System;

namespace Tychaia.UI
{
    public class MenuItem : IContainer
    {
        protected List<MenuItem> m_Items = new List<MenuItem>();

        public IContainer[] Children { get { return this.m_Items.Cast<IContainer>().ToArray(); } }
        public IContainer Parent { get; set; }
        public int Order { get; set; }
        public string Text { get; set; }
        public bool Hovered { get; set; }
        public int HoverCountdown { get; set; }
        public bool Active { get; set; }
        public int TextWidth { get; set; }
        public event EventHandler Click;

        public MenuItem()
        {
            this.Active = false;

            // Give menu items a higher visibility over other things.
            this.Order = 10;
        }

        public void AddChild(MenuItem item)
        {
            this.m_Items.Add(item);
            item.Parent = this;
        }

        public Rectangle? GetMenuListLayout(ISkin skin, Rectangle layout)
        {
            // The location of the child items depends on whether we're owned
            // by a main menu or not.
            if (this.m_Items.Count == 0)
                return null;
            var maxWidth = this.m_Items.Max(x => x.TextWidth) + skin.AdditionalMenuItemWidth;
            var maxHeight = this.m_Items.Count * skin.MenuItemHeight;
            if (this.Parent is MainMenu)
            {
                return new Rectangle(
                    layout.X,
                    layout.Y + layout.Height,
                    maxWidth,
                    maxHeight);
            }
            return new Rectangle(
                layout.X + layout.Width,
                layout.Y,
                maxWidth,
                maxHeight);
        }

        public IEnumerable<KeyValuePair<MenuItem, Rectangle>> GetMenuChildren(ISkin skin, Rectangle layout)
        {
            var childLayout = this.GetMenuListLayout(skin, layout);
            if (childLayout == null)
                yield break;
            var accumulated = 0;
            foreach (var child in this.m_Items)
            {
                yield return new KeyValuePair<MenuItem, Rectangle>(
                    child,
                    new Rectangle(
                        childLayout.Value.X,
                        childLayout.Value.Y + accumulated,
                        childLayout.Value.Width,
                        skin.MenuItemHeight));
                accumulated += skin.MenuItemHeight;
            }
        }

        public IEnumerable<Rectangle> GetActiveChildrenLayouts(ISkin skin, Rectangle layout)
        {
            yield return layout;
            if (!this.Active)
                yield break;
            var childrenLayout = this.GetMenuListLayout(skin, layout);
            if (childrenLayout == null)
                yield break;
            yield return childrenLayout.Value;
            foreach (var kv in this.GetMenuChildren(skin, layout))
                foreach (var childLayout in kv.Key.GetActiveChildrenLayouts(skin, kv.Value))
                    yield return childLayout;
        }

        public virtual void Update(ISkin skin, Rectangle layout, ref bool stealFocus)
        {
            var mouse = Mouse.GetState();
            var leftPressed = mouse.LeftPressed(this);

            if (layout.Contains(mouse.X, mouse.Y))
            {
                this.Hovered = true;
                this.HoverCountdown = 5;
                if (leftPressed)
                {
                    if (this.Click != null)
                        this.Click(this, new EventArgs());
                    this.Active = true;
                }
            }
            var deactivate = true;
            foreach (var activeLayout in this.GetActiveChildrenLayouts(skin, layout))
            {
                if (activeLayout.Contains(mouse.X, mouse.Y))
                {
                    deactivate = false;
                    break;
                }
            }
            this.Hovered = !deactivate;
            if (leftPressed)
                this.Active = !deactivate;

            if (this.HoverCountdown == 0)
                this.Hovered = false;

            if (!(this.Parent is MainMenu))
            {
                this.Active = this.Hovered;
            }

            if (this.Active)
                foreach (var kv in this.GetMenuChildren(skin, layout))
                    kv.Key.Update(skin, kv.Value, ref stealFocus);

            // If the menu item is active, we steal focus from any further updating by our parent.
            if (this.Active)
                stealFocus = true;
        }

        public virtual void Draw(XnaGraphics graphics, ISkin skin, Rectangle layout)
        {
            this.TextWidth = (int)Math.Ceiling(graphics.MeasureString(this.Text).X);
            skin.DrawMenuItem(graphics, layout, this);

            var childrenLayout = this.GetMenuListLayout(skin, layout);
            if (this.Active && childrenLayout != null)
            {
                skin.DrawMenuList(graphics, childrenLayout.Value, this);
                foreach (var kv in this.GetMenuChildren(skin, layout))
                {
                    kv.Key.Draw(graphics, skin, kv.Value);
                }
            }
        }
    }
}

