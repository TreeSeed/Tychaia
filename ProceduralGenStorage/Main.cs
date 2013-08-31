// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Tychaia.ProceduralGeneration;
using System.IO;
using Ninject;

namespace ProceduralGenStorage
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load<TychaiaProceduralGenerationIoCModule>();
            
            var factory = kernel.Get<IRuntimeLayerFactory>();
            var storageAccess = kernel.Get<IStorageAccess>();
        
            // Create algorithm setup.
            var algorithmZoom1 = factory.CreateRuntimeLayer(new AlgorithmZoom2D());
            var algorithmZoom2 = factory.CreateRuntimeLayer(new AlgorithmZoom2D());
            var algorithmZoom3 = factory.CreateRuntimeLayer(new AlgorithmZoom2D());
            var algorithmZoom4 = factory.CreateRuntimeLayer(new AlgorithmZoom2D());
            var algorithmInitialLand = factory.CreateRuntimeLayer(new AlgorithmInitialBool());
            algorithmZoom4.SetInput(0, algorithmInitialLand);
            algorithmZoom3.SetInput(0, algorithmZoom4);
            algorithmZoom2.SetInput(0, algorithmZoom3);
            algorithmZoom1.SetInput(0, algorithmZoom2);

            StorageLayer[] storage = null;
            Console.WriteLine("Storing...");
            using (var writer = new StreamWriter("WorldConfig.xml", false))
                storageAccess.SaveStorage(
                    new StorageLayer[] { storageAccess.FromRuntime(algorithmZoom1) }, writer);

            Console.WriteLine("Loading...");
            using (var reader = new StreamReader("WorldConfig.xml"))
                storage = storageAccess.LoadStorage(reader);
            foreach (var l in storage)
            {
                Console.WriteLine(l.Algorithm.GetType().FullName);
            }
        }
    }
}
