//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia.UI
{
    public abstract class FlowContainer : IContainer
    {
        private List<IContainer> m_Children = new List<IContainer>();
        private List<string> m_Sizes = new List<string>();
        public IContainer[] Children { get { return this.m_Children.ToArray(); } }
        public IContainer Parent { get; set; }
        public int Order { get; set; }

        protected abstract int GetMaximumContainerSize(Rectangle layout);
        protected abstract Rectangle CreateChildLayout(Rectangle layout, int accumulated, int size);

        public IEnumerable<KeyValuePair<IContainer, Rectangle>> ChildrenWithLayouts(Rectangle layout)
        {
            var initialPass = new List<int?>();
            var finalPass = new List<int>();
            var variedCount = 0;
            foreach (var s in this.m_Sizes)
            {
                if (s.EndsWith("%", StringComparison.Ordinal))
                    initialPass.Add((int)(
                        this.GetMaximumContainerSize(layout) * (Convert.ToInt32(s.TrimEnd('%')) / 100f)));
                else if (s == "*")
                {
                    variedCount += 1;
                    initialPass.Add(null);
                }
                else
                    initialPass.Add(Convert.ToInt32(s));
            }
            var total = initialPass.Where(x => x != null).Select(x => x.Value).Sum();
            var remaining = Math.Max(0, this.GetMaximumContainerSize(layout) - total);
            foreach (var i in initialPass)
            {
                if (i == null)
                    finalPass.Add(remaining / variedCount);
                else
                    finalPass.Add(i.Value);
            }
            var accumulated = 0;
            for (var i = 0; i < this.m_Children.Count; i++)
            {
                var childLayout = this.CreateChildLayout(layout, accumulated, finalPass[i]);
                yield return new KeyValuePair<IContainer, Rectangle>(
                    this.m_Children[i],
                    childLayout);
                accumulated += finalPass[i];
            }
        }

        public void AddChild(IContainer child, string size)
        {
            this.m_Children.Add(child);
            this.m_Sizes.Add(size);
        }

        public void RemoveChild(IContainer child)
        {
            this.m_Sizes.RemoveAt(this.m_Children.IndexOf(child));
            this.m_Children.Remove(child);
        }

        public void SetChildSize(IContainer child, string size)
        {
            var index = this.m_Children.IndexOf(child);
            this.m_Sizes.RemoveAt(index);
            this.m_Sizes.Insert(index, size);
        }

        public void Update(ISkin skin, Rectangle layout, ref bool stealFocus)
        {
            foreach (var kv in this.ChildrenWithLayouts(layout))
            {
                kv.Key.Update(skin, kv.Value, ref stealFocus);
                if (stealFocus)
                    break;
            }
        }

        public abstract void Draw(XnaGraphics graphics, ISkin skin, Rectangle layout);
    }
}
