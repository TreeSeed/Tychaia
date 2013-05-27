using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Globals
{
    public static class FilteredConsole
    {
        private static FilterCategory[] m_PermittedCategories = null;

        static FilteredConsole()
        {
            // Change this to effect what is outputted to the console.
            m_PermittedCategories = new FilterCategory[]
            {
                //FilterCategory.Optimization,
                //FilterCategory.Rendering,
                //FilterCategory.RenderingActive,
                //FilterCategory.OptimizationTiming,
                //FilterCategory.UniqueRendering,
                //FilterCategory.Player,
                //FilterCategory.GraphicsMemoryUsage,
                //FilterCategory.OptimizationTiming
                //FilterCategory.ChunkValidation,
                //FilterCategory.OctreeGetTracing,
                //FilterCategory.OctreeSetTracing,
            };
        }

        public static void Write(FilterCategory category, string message)
        {
            if (m_PermittedCategories.Contains(category))
                Console.Write(message);
        }

        public static void WriteLine(FilterCategory category, string message)
        {
            if (m_PermittedCategories.Contains(category))
                Console.WriteLine(message);
        }
    }

    public enum FilterCategory
    {
        Optimization,
        OptimizationTiming,
        Rendering,
        RenderingActive,
        Player,
        ChunkValidation,
        GraphicsMemoryUsage,
        UniqueRendering,
        OctreeGetTracing,
        OctreeSetTracing,
    }
}
