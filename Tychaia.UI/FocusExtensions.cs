//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Linq;
using System;

namespace Tychaia.UI
{
    public static class FocusExtensions
    {
        public static void Focus(this IContainer container)
        {
            DefocusGraph(container);
            container.Focused = true;
        }

        public static void Blur(this IContainer container)
        {
            DefocusGraph(container);
            container.Focused = false;
        }

        private static IContainer GetRootContainer(IContainer container)
        {
            var current = container;
            while (current.Parent != null)
                current = current.Parent;
            return current;
        }

        private static void DefocusGraph(IContainer container)
        {
            DefocusNode(GetRootContainer(container));
        }

        private static void DefocusNode(IContainer container)
        {
            container.Focused = false;
            foreach (var child in container.Children.Where(x => x != null))
                DefocusNode(child);
        }
    }
}

