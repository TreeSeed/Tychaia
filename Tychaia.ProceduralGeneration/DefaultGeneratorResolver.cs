// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System.IO;
using System.Linq;

namespace Tychaia.ProceduralGeneration
{
    internal class DefaultGeneratorResolver : IGeneratorResolver
    {
        private readonly StorageLayer[] m_LoadedLayers;

        public DefaultGeneratorResolver()
        {
            using (var reader = new StreamReader("WorldConfig.xml"))
                this.m_LoadedLayers = StorageAccess.LoadStorage(reader);
        }

        public RuntimeLayer[] GetGenerators()
        {
            return (from storage in this.m_LoadedLayers
                select StorageAccess.ToRuntime(storage)).ToArray();
        }

        public IGenerator GetGeneratorForGame()
        {
            return (from storage in this.m_LoadedLayers
                where storage.Algorithm is AlgorithmBundleOutput
                select StorageAccess.ToRuntime(storage)).First();
        }

        public IGenerator GetGeneratorForExport()
        {
            return (from storage in this.m_LoadedLayers
                where storage.Algorithm is AlgorithmBundleOutput
                select StorageAccess.ToRuntime(storage)).First();
        }
    }
}