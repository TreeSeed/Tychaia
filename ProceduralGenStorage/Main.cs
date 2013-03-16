//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Tychaia.ProceduralGeneration;
using System.IO;

namespace ProceduralGenStorage
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // Create algorithm setup.
            var algorithmZoom1 = new RuntimeLayer(new AlgorithmZoom2D());
            var algorithmZoom2 = new RuntimeLayer(new AlgorithmZoom2D());
            var algorithmZoom3 = new RuntimeLayer(new AlgorithmZoom2D());
            var algorithmZoom4 = new RuntimeLayer(new AlgorithmZoom2D());
            var algorithmInitialLand = new RuntimeLayer(new AlgorithmInitial());
            algorithmZoom4.SetInput(0, algorithmInitialLand);
            algorithmZoom3.SetInput(0, algorithmZoom4);
            algorithmZoom2.SetInput(0, algorithmZoom3);
            algorithmZoom1.SetInput(0, algorithmZoom2);

            StorageLayer[] storage = null;
            Console.WriteLine("Storing...");
            using (var writer = new StreamWriter("WorldConfig.xml", false))
                StorageAccess.SaveStorage(
                    new StorageLayer[] { StorageAccess.FromRuntime(algorithmZoom1) }, writer);

            Console.WriteLine("Loading...");
            using (var reader = new StreamReader("WorldConfig.xml"))
                storage = StorageAccess.LoadStorage(reader);
            foreach (var l in storage)
            {
                Console.WriteLine(l.Algorithm.GetType().FullName);
            }
        }
    }
}
