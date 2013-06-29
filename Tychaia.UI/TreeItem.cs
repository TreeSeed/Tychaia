//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Protogame;

namespace Tychaia.UI
{
    public class TreeItem : IContainer
    {
        public IContainer[] Children { get { return new IContainer[0]; } }
        public IContainer Parent { get; set; }
        public int Order { get; set; }
        public virtual string Text { get; set; }
        public bool Focused { get; set; }

        public int Indent
        {
            get { return (this.Text ?? "").Split('.').Length; }
        }

        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            var mouse = Mouse.GetState();
            if (layout.Contains(mouse.X, mouse.Y) && mouse.LeftPressed(this))
                (this.Parent as TreeView).SelectedItem = this;
        }

        public void Draw(XnaGraphics graphics, ISkin skin, Rectangle layout)
        {
            skin.DrawTreeItem(graphics, layout, this);
        }
    }
}

