// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using Redpoint.FlowGraph;
using Tychaia.Globals;
using Tychaia.ProceduralGeneration.Compiler;
using Tychaia.ProceduralGeneration.Flow;

namespace Tychaia.ProceduralGeneration
{
    internal class StorageAccess : IStorageAccess
    {
        private readonly IPool m_Pool;
        private readonly IArrayPool m_ArrayPool;
        
        public StorageAccess(
            IPool pool,
            IArrayPool arrayPool)
        {
            this.m_Pool = pool;
            this.m_ArrayPool = arrayPool;
        }
    
        #region Conversion to runtime and compiled layers

        /// <summary>
        /// Converts the runtime layer representation into
        /// a storage layer so that it can be saved.
        /// </summary>
        public StorageLayer FromRuntime(RuntimeLayer layer)
        {
            // Handle null conversion.
            if (layer == null)
                return null;

            // Create storage.
            var storage = new StorageLayer
            {
                Algorithm = layer.Algorithm
            };

            // Convert inputs.
            for (var i = 0; i < layer.GetInputs().Length; i++)
                storage.Inputs[i] = FromRuntime(layer.GetInputs()[i]);

            // Return storage.
            return storage;
        }

        /// <summary>
        /// Converts the loaded storage layer into a runtime layer.
        /// </summary>
        public RuntimeLayer ToRuntime(StorageLayer layer)
        {
            // Handle null conversion.
            if (layer == null)
                return null;

            // Convert runtime layer.
            var runtime = new RuntimeLayer(
                this.m_Pool,
                this.m_ArrayPool,
                layer.Algorithm);
            if (layer.Inputs != null)
                for (var i = 0; i < layer.Inputs.Length; i++)
                    runtime.SetInput(i, ToRuntime(layer.Inputs[i]));
            return runtime;
        }

        /// <summary>
        /// Compiles the storage layer.
        /// </summary>
        public IGenerator ToCompiled(StorageLayer layer)
        {
            return ToCompiled(ToRuntime(layer));
        }

        /// <summary>
        /// Compiles the runtime layer.
        /// </summary>
        public IGenerator ToCompiled(RuntimeLayer layer)
        {
            var result = LayerCompiler.Compile(layer);
            result.SetSeed(layer.Seed);
            return result;
        }

        #endregion

        #region Saving and loading of storage representation

        /// <summary>
        /// Adds all the layers recursively to a list.
        /// </summary>
        public void AddRecursiveStorage(List<StorageLayer> allLayers, StorageLayer layer)
        {
            if (!allLayers.Contains(layer))
                allLayers.Add(layer);
            if (layer != null && layer.Inputs != null)
                foreach (var input in layer.Inputs)
                    AddRecursiveStorage(allLayers, input);
        }

        /// <summary>
        /// Saves the storage layer to file.
        /// </summary>
        public void SaveStorage(StorageLayer layer, StreamWriter output)
        {
            SaveStorage(new[] { layer }, output);
        }

        /// <summary>
        /// Saves the storage layers to file.
        /// </summary>
        public void SaveStorage(StorageLayer[] layers, StreamWriter output)
        {
            // Find all possible layers that need to saved.
            var allLayers = layers.ToList();
            foreach (var layer in layers)
                AddRecursiveStorage(allLayers, layer);

            // Save all.
            var x = new DataContractSerializer(
                typeof(StorageLayer[]),
                SerializableTypes,
                Int32.MaxValue,
                false,
                true,
                null);
            x.WriteObject(output.BaseStream, allLayers.ToArray());
        }

        /// <summary>
        /// Loads storage layers from a stream.
        /// </summary>
        public StorageLayer[] LoadStorage(StreamReader input)
        {
            var x = new DataContractSerializer(typeof(StorageLayer[]), SerializableTypes);
            using (
                var reader = XmlDictionaryReader.CreateTextReader(input.BaseStream,
                    new XmlDictionaryReaderQuotas { MaxDepth = 1000 }))
                return x.ReadObject(reader, true) as StorageLayer[];
        }

        #endregion

        #region Initialization of static loading and saving logic

        private static Type[] SerializableTypes = null;

        static StorageAccess()
        {
            // Dynamically generate a list of serializable types for the
            // data contract.
            var types = new List<Type>
            {
                // Flow system classes
                typeof(FlowConnector),
                typeof(FlowElement),
                typeof(AlgorithmFlowConnector),
                typeof(AlgorithmFlowElement),
                typeof(StorageLayer),
                typeof(StorageLayer[]),
            };
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] assemblyTypes;
                try
                {
                    assemblyTypes = a.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // We couldn't load all the types from this assembly, but
                    // then again, we may not have to.  Use whatever types we
                    // could load and then skip over the nulls in the foreach.
                    assemblyTypes = ex.Types;
                    Console.WriteLine("errors while loading assembly:");
                    Console.WriteLine("-- " + a.FullName);
                    foreach (var exx in ex.LoaderExceptions)
                    {
                        Console.WriteLine(exx);
                    }
                }
                foreach (var t in assemblyTypes.Where(x => x != null))
                    if (typeof(IAlgorithm).IsAssignableFrom(t) && !t.IsGenericType)
                        types.Add(t);
            }
            SerializableTypes = types.ToArray();
        }

        #endregion
    }
}
