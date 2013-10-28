// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Dx.Runtime;
using Ninject;
using Ninject.Parameters;

namespace Tychaia.Network
{
    public class DependencyInjectedDxFactory : DefaultDxFactory
    {
        private readonly IKernel m_Kernel;
        
        public DependencyInjectedDxFactory(
            IKernel kernel)
        {
            this.m_Kernel = kernel;
        }
    
        public override INetworkProvider CreateNetworkProvider(ILocalNode localNode)
        {
            return this.m_Kernel.Get<INetworkProvider>(
                new ConstructorArgument("node", localNode));
        }
    }
}
