using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Informs the game that this is a layer it can take and use for the actual
    /// generation of the in-game world.
    /// </summary>
    [DataContract]
    [FlowDesignerCategory(FlowCategory.General)]
    [FlowDesignerName("Store Result")]
    public class LayerStoreResult : Layer2D
    {
        [DataMember]
        [DefaultValue(FinishType.Biome)]
        [Description("The type of result that is used by the game.")]
        public FinishType FinishType
        {
            get;
            set;
        }

        public LayerStoreResult(Layer parent)
            : base(parent)
        {
            // Set defaults.
            this.FinishType = FinishType.Biome;
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
            return "Store Result";
        }

    }

    public enum FinishType
    {
        Biome = 0,
        Rainfall = 1,
        Temperature = 2,
        Terrain = 3,
        FamilyTree = 4,
        Towns = 5
    }
}
