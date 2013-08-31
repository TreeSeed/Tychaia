// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.IO;
using System.Linq;

namespace Tychaia.ProceduralGeneration
{
    public class DefaultGeneratorResolver : IGeneratorResolver
    {
        private readonly StorageLayer[] m_LoadedLayers;
        private readonly IStorageAccess m_StorageAccess;

        public DefaultGeneratorResolver(IStorageAccess storageAccess)
        {
            this.m_StorageAccess = storageAccess;
            
            using (var reader = new StreamReader("WorldConfig.xml"))
                this.m_LoadedLayers = this.m_StorageAccess.LoadStorage(reader);
        }

        public RuntimeLayer[] GetGenerators()
        {
            return (from storage in this.m_LoadedLayers
                select this.m_StorageAccess.ToRuntime(storage)).ToArray();
        }

        public IGenerator GetGeneratorForGame()
        {
            return (from storage in this.m_LoadedLayers
                where storage.Algorithm is AlgorithmBundleOutput
                select this.m_StorageAccess.ToRuntime(storage)).First();
        }

        public IGenerator GetGeneratorForExport()
        {
            return (from storage in this.m_LoadedLayers
                where storage.Algorithm is AlgorithmBundleOutput
                select this.m_StorageAccess.ToRuntime(storage)).First();
        }
    }
}
