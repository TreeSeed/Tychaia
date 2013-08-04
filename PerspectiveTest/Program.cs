using System;

namespace PerspectiveTest
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (var game = new PerspectiveGame())
            {
                game.Run();
            }
        }
    }
}

