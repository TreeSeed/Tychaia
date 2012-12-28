using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using Tychaia.Globals;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Informs MMAW that this is a layer that can be selected from the interface.
    /// </summary>
    [DataContract]
    [FlowDesignerCategory(FlowCategory.General)]
    [FlowDesignerName("Exportable to Make Me a World")]
    public class Layer3DMMAWResult : Layer3D
    {
        [DataMember]
        [Description("The name used to identify this export and show in the MMAW interface.")]
        public string Name
        {
            get;
            set;
        }

        public Layer3DMMAWResult(Layer parent)
            : base(parent)
        {
        }

        protected override int[] GenerateDataImpl(long x, long y, long z, long width, long height, long depth)
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height];

            return this.Parents[0].GenerateData(x, y, z, width, height, depth);
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return null;

            return this.Parents[0].GetLayerColors();
        }

        public override bool[] GetParents3DRequired()
        {
            return new bool[] { true };
        }

        public override string ToString()
        {
            return "MMAW Export 3D";
        }
    }
}
