//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Linq;
using Redpoint.FlowGraph;
using Tychaia.ProceduralGeneration.Flow;
using Tychaia.ProceduralGeneration.Compiler;

namespace Tychaia.ProceduralGeneration
{
    public static class StorageAccess
    {
        #region Conversion to runtime and compiled layers

        /// <summary>
        /// Converts the runtime layer representation into
        /// a storage layer so that it can be saved.
        /// </summary>
        public static StorageLayer FromRuntime(RuntimeLayer layer)
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
            for (int i = 0; i < layer.GetInputs().Length; i++)
                storage.Inputs[i] = StorageAccess.FromRuntime(layer.GetInputs()[i]);

            // Return storage.
            return storage;
        }

        /// <summary>
        /// Converts the loaded storage layer into a runtime layer.
        /// </summary>
        public static RuntimeLayer ToRuntime(StorageLayer layer)
        {
            // Handle null conversion.
            if (layer == null)
                return null;

            // Convert runtime layer.
            var runtime = new RuntimeLayer(layer.Algorithm);
            if (layer.Inputs != null)
                for (var i = 0; i < layer.Inputs.Length; i++)
                    runtime.SetInput(i, StorageAccess.ToRuntime(layer.Inputs[i]));
            return runtime;
        }

        /// <summary>
        /// Compiles the storage layer.
        /// </summary>
        public static IGenerator ToCompiled(StorageLayer layer)
        {
            return ToCompiled(ToRuntime(layer));
        }

        /// <summary>
        /// Compiles the runtime layer.
        /// </summary>
        public static IGenerator ToCompiled(RuntimeLayer layer)
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
        public static void AddRecursiveStorage(List<StorageLayer> allLayers, StorageLayer layer)
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
        public static void SaveStorage(StorageLayer layer, StreamWriter output)
        {
            SaveStorage(new StorageLayer[] { layer }, output);
        }

        /// <summary>
        /// Saves the storage layers to file.
        /// </summary>
        public static void SaveStorage(StorageLayer[] layers, StreamWriter output)
        {
            // Find all possible layers that need to saved.
            var allLayers = layers.ToList();
            foreach (var layer in layers)
                AddRecursiveStorage(allLayers, layer);

            // Save all.
            DataContractSerializer x = new DataContractSerializer(
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
        public static StorageLayer[] LoadStorage(StreamReader input)
        {
            DataContractSerializer x = new DataContractSerializer(typeof(StorageLayer[]), SerializableTypes);
            using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(input.BaseStream, new XmlDictionaryReaderQuotas() { MaxDepth = 1000 }))
                return x.ReadObject(reader, true) as StorageLayer[];
        }

        #endregion

        #region Initialization of static loading and saving logic

        private static Type[] SerializableTypes = null;

        static StorageAccess()
        {
            // Dynamically generate a list of serializable types for the
            // data contract.
            List<Type> types = new List<Type> {
                // Flow system classes
                typeof(FlowConnector),
                typeof(FlowElement),
                typeof(AlgorithmFlowConnector),
                typeof(AlgorithmFlowElement),
                typeof(StorageLayer),
                typeof(StorageLayer[]),
            };
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
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
                foreach (Type t in assemblyTypes.Where(x => x != null))
                    if (typeof(IAlgorithm).IsAssignableFrom(t) && !t.IsGenericType)
                        types.Add(t);
            }
            StorageAccess.SerializableTypes = types.ToArray();
        }

        #endregion
    }
}

