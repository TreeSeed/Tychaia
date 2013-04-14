//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Tychaia.ProceduralGeneration.Analysis.Reporting
{
    public struct AnalysisLayer : IXmlSerializable
    {
        public string ID;
        public StorageLayer StorageLayer;
        
        #region IXmlSerializable implementation
        
        public XmlSchema GetSchema()
        {
        }
        
        public void ReadXml(XmlReader reader)
        {
        }
        
        public void WriteXml(XmlWriter writer)
        {
        }
        
        #endregion
    }
}

