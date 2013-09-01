// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.IO;
using System.Linq;
using System.Reflection;

namespace Tychaia.ProceduralGeneration
{
    public class DefaultGeneratorResolver : IGeneratorResolver
    {
        private readonly IStorageAccess m_StorageAccess;
        private StorageLayer[] m_LoadedLayers;

        private void EnsureLoaded(bool throwException = true)
        {
            if (this.m_LoadedLayers != null)
                return;
            var path = Path.Combine(
                new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName,
                "WorldConfig.xml");
            if (File.Exists(path))
                using (var reader = new StreamReader(path))
                    this.m_LoadedLayers = this.m_StorageAccess.LoadStorage(reader);
            else if (throwException)
                throw new FileNotFoundException("Unable to locate world configuration", path);
        }

        public DefaultGeneratorResolver(IStorageAccess storageAccess)
        {
            this.m_StorageAccess = storageAccess;
            
            this.EnsureLoaded(false);
        }

        public RuntimeLayer[] GetGenerators()
        {
            this.EnsureLoaded();

            return (from storage in this.m_LoadedLayers
                select this.m_StorageAccess.ToRuntime(storage)).ToArray();
        }

        public IGenerator GetGeneratorForGame()
        {
            this.EnsureLoaded();

            return (from storage in this.m_LoadedLayers
                where storage.Algorithm is AlgorithmBundleOutput
                select this.m_StorageAccess.ToRuntime(storage)).First();
        }

        public IGenerator GetGeneratorForExport()
        {
            this.EnsureLoaded();

            return (from storage in this.m_LoadedLayers
                where storage.Algorithm is AlgorithmBundleOutput
                select this.m_StorageAccess.ToRuntime(storage)).First();
        }
    }
}
