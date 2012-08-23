using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Spells.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Random r;
            if (args.Length == 1)
                r = new Random(Convert.ToInt32(args[0]));
            else
                r = new Random();

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(SpellGenerator.Generate(r.Next()));
            }
        }
    }
}
