// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System;
using ICSharpCode.NRefactory;

namespace Tychaia.ProceduralGeneration.Analysis
{
    public static class TextLocationExtensions
    {
        public static int GetAbsolutePosition(this TextLocation location, string body)
        {
            var line = location.Line;
            var column = location.Column;
            var count = 0;
            Console.WriteLine("Calculating absolute position...");
            Console.WriteLine("line - " + line);
            Console.WriteLine("column - " + column);
            for (var i = 0; i < body.Length && (line > 0 || column > 0); i++)
            {
                count += 1;
                if (line == 0)
                    column -= 1;
                else if (body[i] == '\n')
                    line -= 1;
            }
            Console.WriteLine("count - " + count);
            return count;
        }
    }
}