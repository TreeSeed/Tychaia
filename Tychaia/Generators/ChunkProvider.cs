using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tychaia.ProceduralGeneration;
using System.Runtime.Serialization;
using TychaiaWorldGenViewer.Flow;
using System.Xml;
using System.Reflection;
using System.IO;
using Tychaia.Globals;

namespace Tychaia.Generators
{
    public static class ChunkProvider
    {
        private static Layer m_ResultLayer = null;
        private static Type[] m_SerializableTypes = null;
        private const string m_WorldConfig = "WorldConfig.xml";

        static ChunkProvider()
        {
            // Dynamically generate a list of serializable types for the
            // data contract.
            List<Type> types = new List<Type> {
                // Flow system classes
                typeof(FlowConnector),
                typeof(FlowElement),
                typeof(LayerFlowConnector),
                typeof(LayerFlowElement),
            };
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type t in a.GetTypes())
                    if (typeof(Layer).IsAssignableFrom(t))
                        types.Add(t);
            m_SerializableTypes = types.ToArray();

            // Load configuration.
            DataContractSerializer x = new DataContractSerializer(typeof(FlowInterfaceControl.ListFlowElement), m_SerializableTypes);
            FlowInterfaceControl.ListFlowElement config = null;
            using (FileStream fstream = new FileStream(m_WorldConfig, FileMode.Open))
            using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fstream, new XmlDictionaryReaderQuotas()))
                config = x.ReadObject(reader, true) as FlowInterfaceControl.ListFlowElement;

            // Find the result layer.
            foreach (FlowElement fe in config)
            {
                if (fe is LayerFlowElement)
                {
                    if ((fe as LayerFlowElement).Layer is Layer3DStoreResult)
                    {
                        m_ResultLayer = (fe as LayerFlowElement).Layer;
                        return;
                    }
                }
            }
        }

        public static void FillChunk(Block[, ,] blocks, ChunkInfo info)
        {
            if (m_ResultLayer == null)
                throw new InvalidOperationException("No 3D store result layer was found in the world configuration.");
            int depth = Settings.ChunkDepth;
            int[] data = m_ResultLayer.GenerateData(info.Bounds.X, info.Bounds.Y, 0, info.Bounds.Width, info.Bounds.Height, depth);
            for (int i = 0; i < info.Bounds.Width; i++)
                for (int j = 0; j < info.Bounds.Height; j++)
                    for (int k = 0; k < depth; k++)
                    {
                        int id = data[i + j * info.Bounds.Width + k * info.Bounds.Width * info.Bounds.Height];
                        if (id == -1)
                            blocks[i, j, k] = null;
                        else
                            blocks[i, j, k] = Block.BlockIDMapping[data[i + j * info.Bounds.Width + k * info.Bounds.Width * info.Bounds.Height]];
                    }
        }
    }
}
