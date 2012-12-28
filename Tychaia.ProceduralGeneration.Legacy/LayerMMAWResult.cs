using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Informs MMAW that this is a layer that can be selected from the interface.
    /// </summary>
    [DataContract]
    [FlowDesignerCategory(FlowCategory.General)]
    [FlowDesignerName("Exportable to Make Me a World")]
    public class LayerMMAWResult : Layer2D
    {
        [DataMember]
        [Description("The name used to identify this export and show in the MMAW interface.")]
        public string Name
        {
            get;
            set;
        }

        public LayerMMAWResult(Layer parent)
            : base(parent)
        {
        }

        protected override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height];

            return this.Parents[0].GenerateData(x, y, width, height);
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return null;

            return this.Parents[0].GetLayerColors();
        }

        public override string ToString()
        {
            return "MMAW Export";
        }
    }
}
