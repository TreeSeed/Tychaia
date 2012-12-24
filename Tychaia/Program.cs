using System;
using Tychaia.Generators;

namespace Tychaia
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (RuntimeGame game = new RuntimeGame())
            {
                game.Run();
            }
        }
    }
}

