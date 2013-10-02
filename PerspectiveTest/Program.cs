// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Ninject;

namespace PerspectiveTest
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Bind<IRenderDemo>().To<SingleCubeDemo>();
            kernel.Bind<IRenderDemo>().To<UncachedChunkDemo>();
            kernel.Bind<IRenderDemo>().To<EverythingDemo>();
        
            using (var game = new PerspectiveGame(kernel.GetAll<IRenderDemo>()))
            {
                game.Run();
            }
        }
    }
}
