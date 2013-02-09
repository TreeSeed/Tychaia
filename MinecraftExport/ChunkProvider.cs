using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tychaia.ProceduralGeneration;
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
            // FIXME: Use StorageAccess to load reference
            // to world generation.
            throw new NotImplementedException();
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
