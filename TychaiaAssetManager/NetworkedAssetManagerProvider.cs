//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Tychaia.Assets;
using Process4;
using Process4.Collections;

namespace TychaiaAssetManager
{
    public class NetworkedAssetManagerProvider : IAssetManagerProvider
    {
        private LocalNode m_Node;

        public bool IsReady
        {
            get
            {
                var assetManager = (NetworkAssetManager)
                    new Distributed<NetworkAssetManager>("asset-manager", true);
                if (assetManager == null)
                    return false;
                return assetManager.IsReady();
            }
        }

        public NetworkedAssetManagerProvider(LocalNode node)
        {
            this.m_Node = node;
        }

        public IAssetManager GetAssetManager(bool permitCreate = false)
        {
            return (NetworkAssetManager)
                (new Distributed<NetworkAssetManager>("asset-manager", !permitCreate));
        }
    }
}

