using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia
{
    public static class FilteredConsole
    {
        private static FilterCategory[] m_PermittedCategories = null;

        static FilteredConsole()
        {
            // Change this to effect what is outputted to the console.
            m_PermittedCategories = new FilterCategory[]
            {
                FilterCategory.Optimization
            };
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
        Player,
        ChunkValidation
    }
}
