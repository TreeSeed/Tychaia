using System;
using Tychaia.Generators;
using System.Windows.Forms;

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
                try
                {
                    game.Run();
                }
                catch (EntryPointNotFoundException)
                {
#if PLATFORM_WINDOWS || PLATFORM_LINUX
                    if (!game.HasTicked)
                        MessageBox.Show("Tychaia requires a graphics card with 3D hardware acceleration.", "Sorry!");
                    else
#endif
                        throw;
                }
            }
        }
    }
}

