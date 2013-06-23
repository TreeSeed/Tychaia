//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Tychaia.Assets;

namespace TychaiaAssetManager
{
    public class GameAssetManagerProvider : IAssetManagerProvider
    {
        private LocalAssetManager m_AssetManager = new LocalAssetManager();

        public bool IsReady
        {
            get
            {
                return true;
            }
        }

        public IAssetManager GetAssetManager(bool permitCreate)
        {
            return this.m_AssetManager;
        }
    }
}

