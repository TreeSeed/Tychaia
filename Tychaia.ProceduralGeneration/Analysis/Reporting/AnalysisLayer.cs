// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System.Collections.Generic;
using System.Runtime.Serialization;
using ICSharpCode.Decompiler.Ast;
using Tychaia.ProceduralGeneration.Analysis.Output;
using Tychaia.ProceduralGeneration.Compiler;

namespace Tychaia.ProceduralGeneration.Analysis.Reporting
{
    [DataContract(Name = "layer")]
    public class AnalysisLayer
    {
        /// <summary>
        /// The algorithm.  This isn't stored as a data member
        /// as it's only used for internal processing when
        /// AnalysisLayer is being passed around.
        /// </summary>
        public IAlgorithm Algorithm;

        /// <summary>
        /// Same as algorithm, just used internally for
        /// processing.
        /// </summary>
        public AstBuilder AstBuilder;

        [DataMember(Name = "code")] public string
            Code;

        [DataMember(Name = "name")] public string
            Name;

        [DataMember(Name = "reports")] public List<AnalysisReport>
            Reports = new List<AnalysisReport>();

        public AnalysisLayer()
        {
        }

        public AnalysisLayer(StorageLayer layer)
        {
            AstBuilder astBuilder;
            var method = DecompileUtil.GetMethodCode(layer.Algorithm.GetType(), out astBuilder, "ProcessCell");
            this.Name = layer.Algorithm.GetType().Name;
            this.Code = method.Body.GetTrackedText();
            this.Algorithm = layer.Algorithm;
            this.AstBuilder = astBuilder;
        }

        [DataMember(Name = "hash")]
        public uint Hash
        {
            get { return (uint) this.Name.GetHashCode(); }
            set
            {
                // Useless operation to use value just so that
                // MonoDevelop doesn't complain about an unused
                // setter.  We need the setter so that the
                // data contract can deserialize the object, even
                // though in reality we never deserialize the
                // report objects, and the getter would return the
                // same value if Name is the same anyway.
                var a = this.Name;
                this.Name = value.ToString();
                this.Name = a;
            }
        }
    }
}