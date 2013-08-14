// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.IO;
using ICSharpCode.NRefactory.CSharp;

namespace Tychaia.ProceduralGeneration.Analysis.Output
{
    public static class AstNodeExtensions
    {
        public static string GetTrackedText(this AstNode node, CSharpFormattingOptions formattingOptions = null)
        {
            if (node.IsNull)
            {
                return "";
            }
            var stringWriter = new StringWriter();
            var wrapper = new TrackingOutputFormatter(stringWriter);
            wrapper.IndentationString = "    ";
            node.AcceptVisitor(new CSharpOutputVisitor(wrapper,
                formattingOptions ?? FormattingOptionsFactory.CreateMono()));
            return stringWriter.ToString();
        }
    }
}
