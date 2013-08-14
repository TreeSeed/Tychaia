// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.IO;
using ICSharpCode.NRefactory.CSharp;

namespace Tychaia.ProceduralGeneration.Analysis.Output
{
    public class StartTrackingInfo
    {
        public int CharacterPosition;
    }

    public class EndTrackingInfo
    {
        public int CharacterPosition;
    }

    public class TrackingOutputFormatter : TextWriterOutputFormatter
    {
        private readonly TextWriter m_Writer;

        public TrackingOutputFormatter(TextWriter textWriter)
            : base(textWriter)
        {
            this.m_Writer = textWriter;
        }

        public override void StartNode(AstNode node)
        {
            node.AddAnnotation(new StartTrackingInfo { CharacterPosition = this.m_Writer.ToString().Length });
            base.StartNode(node);
        }

        public override void EndNode(AstNode node)
        {
            base.EndNode(node);
            node.AddAnnotation(new EndTrackingInfo { CharacterPosition = this.m_Writer.ToString().Length });
        }
    }
}
