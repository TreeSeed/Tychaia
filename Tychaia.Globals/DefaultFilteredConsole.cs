// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System;
using System.Linq;

namespace Tychaia.Globals
{
    internal class DefaultFilteredConsole : IFilteredConsole
    {
        private readonly FilterCategory[] m_PermittedCategories;

        public DefaultFilteredConsole()
        {
            // Change this to effect what is outputted to the console.
            this.m_PermittedCategories = new[]
            {
                FilterCategory.Optimization,
                FilterCategory.Rendering,
                FilterCategory.RenderingActive,
                FilterCategory.OptimizationTiming,
                FilterCategory.UniqueRendering,
                FilterCategory.Player,
                FilterCategory.GraphicsMemoryUsage,
                FilterCategory.OptimizationTiming,
                FilterCategory.ChunkValidation,
                FilterCategory.OctreeGetTracing,
                FilterCategory.OctreeSetTracing
            };
        }

        public void Write(FilterCategory category, string message)
        {
            if (this.m_PermittedCategories.Contains(category))
                Console.Write(message);
        }

        public void WriteLine(FilterCategory category, string message)
        {
            if (this.m_PermittedCategories.Contains(category))
                Console.WriteLine(message);
        }
    }
}