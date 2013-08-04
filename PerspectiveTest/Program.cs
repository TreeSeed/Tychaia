using System;
using Ninject;

namespace PerspectiveTest
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Bind<IRenderDemo>().To<UncachedChunkDemo>();
            kernel.Bind<IRenderDemo>().To<EverythingDemo>();
            kernel.Bind<IRenderDemo>().To<SingleCubeDemo>();
        
            using (var game = new PerspectiveGame(kernel.GetAll<IRenderDemo>()))
            {
                game.Run();
            }
        }
    }
}

