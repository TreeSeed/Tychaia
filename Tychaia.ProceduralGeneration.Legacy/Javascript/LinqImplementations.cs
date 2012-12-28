using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// JSIL doesn't implement all of LINQ, so we need to make up for any missing functionality
    /// that we use in the procedural generator.
    /// </summary>
    public static class LinqImplementations
    {
        public static Int32 Sum(this IEnumerable<Int32> v)
        {
            Int32 r = 0;
            foreach (var x in v)
                r += x;
            return r;
        }
    }
}
