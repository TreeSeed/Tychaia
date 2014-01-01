using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;

namespace Tychaia.Network
{
    public class CurrentNetworkAPIProvider : INetworkAPIProvider
    {
        private readonly IKernel m_Kernel;

        public CurrentNetworkAPIProvider(IKernel kernel)
        {
            this.m_Kernel = kernel;
        }

        public bool IsAvailable
        {
            get
            {
                return this.m_Kernel.TryGet<INetworkAPI>() != null;
            }
        }

        public INetworkAPI GetAPI()
        {
            return this.m_Kernel.Get<INetworkAPI>();
        }
    }
}
