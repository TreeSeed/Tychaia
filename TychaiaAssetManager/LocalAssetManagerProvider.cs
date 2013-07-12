//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Tychaia.Assets;
using System.IO;
using System.Reflection;

namespace TychaiaAssetManager
{
    public class LocalAssetManagerProvider : IAssetManagerProvider
    {
        private LocalAssetManager m_AssetManager;

        public LocalAssetManagerProvider()
        {
            var file = new FileInfo(Assembly.GetExecutingAssembly().Location);
            this.m_AssetManager = new LocalAssetManager(
                Path.Combine(file.Directory.FullName, "Content"));
        }

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
