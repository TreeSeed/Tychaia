using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tychaia.ProceduralGeneration;
using TychaiaWorldGenViewer.Flow;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;

namespace MinecraftExport
{
    public static class ChunkProvider
    {
        private static Layer m_ResultLayer = null;
        private static Type[] m_SerializableTypes = null;
        private const string m_WorldConfig = "WorldConfig.xml";

        #region Initialization

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

        #endregion

        public static int[] GetData(int x, int y, int z)
        {
            return m_ResultLayer.GenerateData(
                 x,
                 y,
                 z,
                 16,
                 16,
                 256);
        }
    }
}
