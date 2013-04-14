//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Tychaia.ProceduralGeneration.Analysis.Reporting
{
    public struct AnalysisReport : IXmlSerializable
    {
        public List<AnalysisIssue> Issues = new List<AnalysisIssue>();
        
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

