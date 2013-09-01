// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Ninject;
using Tychaia.Globals;
using Tychaia.ProceduralGeneration.Compiler;

namespace Tychaia.ProceduralGeneration.Tests
{
    public class TestBase
    {
        protected RuntimeLayer CreateRuntimeLayer(IAlgorithm algorithm)
        {
            var kernel = new StandardKernel();
            kernel.Load<TychaiaGlobalIoCModule>();
            kernel.Load<TychaiaProceduralGenerationIoCModule>();
            return kernel.Get<IRuntimeLayerFactory>().CreateRuntimeLayer(algorithm);
        }
        
        protected IGenerator CompileLayer(RuntimeLayer layer, bool optimize = true)
        {
            return LayerCompiler.Compile(layer, optimize);
        }
    }
}

