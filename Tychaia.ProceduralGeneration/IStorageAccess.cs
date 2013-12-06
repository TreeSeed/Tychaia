// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Generic;
using System.IO;

namespace Tychaia.ProceduralGeneration
{
    public interface IStorageAccess
    {
        StorageLayer FromRuntime(RuntimeLayer layer);
        RuntimeLayer ToRuntime(StorageLayer layer);
        IGenerator ToCompiled(StorageLayer layer);
        IGenerator ToCompiled(RuntimeLayer layer);
        
        void AddRecursiveStorage(List<StorageLayer> allLayers, StorageLayer layer);
        void SaveStorage(StorageLayer layer, StreamWriter output);
        void SaveStorage(StorageLayer[] layers, StreamWriter output);
        StorageLayer[] LoadStorage(StreamReader input);
    }
}
