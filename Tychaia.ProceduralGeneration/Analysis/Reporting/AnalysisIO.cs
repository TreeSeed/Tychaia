//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Collections.Generic;

namespace Tychaia.ProceduralGeneration.Analysis.Reporting
{
    public class AnalysisIO
    {
        private static Type[] m_SerializableTypes = null;

        static AnalysisIO()
        {
            List<Type> types = new List<Type> {
                typeof(Analysis),
                typeof(AnalysisLayer),
                typeof(AnalysisReport),
                typeof(AnalysisIssue),
                typeof(AnalysisLocation),
                typeof(AnalysisLocationHighlight)
            };
            m_SerializableTypes = types.ToArray();
        }

        /// <summary>
        /// Saves the analysis to a stream
        /// </summary>
        public static void Save(Analysis analysis, StreamWriter output)
        {
            using (var memory = new MemoryStream())
            {
                // Write out leading XML.
                output.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
                output.Write("<?xml-stylesheet type=\"text/xsl\" href=\"reports.xsl\"?>");
                output.Flush();

                // Write to memory.
                var x = new DataContractSerializer(
                    typeof(Analysis),
                    m_SerializableTypes,
                    Int32.MaxValue,
                    false,
                    false,
                    null);
                x.WriteObject(memory, analysis);

                // Reset memory position and write out the contents to the actual stream.
                memory.Seek(0, SeekOrigin.Begin);
                memory.CopyTo(output.BaseStream);
            }
        }

        /// <summary>
        /// Loads storage layers from a stream.
        /// </summary>
        public static Analysis Load(StreamReader input)
        {
            var x = new DataContractSerializer(typeof(Analysis), m_SerializableTypes);
            using (var reader = XmlDictionaryReader.CreateTextReader(input.BaseStream, new XmlDictionaryReaderQuotas() { MaxDepth = 1000 }))
                return x.ReadObject(reader, true) as Analysis;
        }
    }
}

