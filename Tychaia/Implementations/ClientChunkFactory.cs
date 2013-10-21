// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Ninject;
using Ninject.Parameters;

namespace Tychaia
{
    public class ClientChunkFactory : IChunkFactory
    {
        private IKernel m_Kernel;
    
        public ClientChunkFactory(IKernel kernel)
        {
            this.m_Kernel = kernel;
        }

        public RuntimeChunk CreateChunk(ILevel level, ChunkOctree octree, long x, long y, long z)
        {
            return this.m_Kernel.Get<ClientRuntimeChunk>(
                new ConstructorArgument("level", level),
                new ConstructorArgument("octree", octree),
                new ConstructorArgument("x", x),
                new ConstructorArgument("y", y),
                new ConstructorArgument("z", z));
        }
    }
}
