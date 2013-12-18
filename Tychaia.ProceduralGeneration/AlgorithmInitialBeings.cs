// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using Tychaia.Asset;
using Tychaia.Data;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Beings)]
    [FlowDesignerName("Initial Beings")]
    public class AlgorithmInitialBeings : Algorithm<int, Cell>
    {
        public AlgorithmInitialBeings()
        {
            this.Limit = 0.9;
            this.GuaranteeStartingPoint = true;
            this.PlacementType = BeingType.Enemies;
        }
        
        public override string[] InputNames
        {
            get
            {
                return new[] { "Area Level" };
            }
        }

        [DataMember]
        [DefaultValue(0.9)]
        [Description("The value between 0.0 and 1.0 above which the cell is selected.")]
        public double Limit { get; set; }

        [DataMember]
        [DefaultValue(true)]
        [Description("Whether to guarantee a 0 value at the global (0, 0) position.")]
        public bool GuaranteeStartingPoint { get; set; }

        [DataMember]
        [DefaultValue(BeingType.Enemies)]
        [Description("Determines if you spawn \"Enemies\" or \"NPCs\".")]
        public BeingType PlacementType
        {
            get;
            set;
        }

        public override bool[] InputIs2D
        {
            get
            {
                return new[] { true };
            }
        }

        public override bool Is2DOnly
        {
            get { return true; }
        }

        public override void ProcessCell(
            IRuntimeContext context,
            int[] input,
            Cell[] output,
            long x,
            long y,
            long z,
            int i,
            int j,
            int k,
            int width,
            int height,
            int depth,
            int ox,
            int oy,
            int oz)
        {
            // TODO: Figure out how this should be done.
            // var BeingClusterList = context.PreceduralAssetManager.GetAll("BeingClusterDefinitionAsset");
            var beingCluster = string.Empty;
            var outputCell = new Cell();

            // foreach (var cluster in BeingClusterList)
            // if (input > cluster.level)
            // remove cluster;

            // if (!(this.GuaranteeStartingPoint && x == 0 && y == 0) && AlgorithmUtility.GetRandomRange(context.Seed, x, y, 0, BeingClusterList.Count(), context.Modifier) > this.Limit)
            // {
            // outputCell.ClusterDefinitionAssetName = 
            // etc
            // }

            // output[(i + ox) + ((j + oy) * width)] = outputCell;
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            if (string.IsNullOrEmpty(value.ClusterDefinitionAssetName))
                return Color.White;

            return Color.FromArgb(value.ClusterDefinitionAssetName.GetHashCode());
        }

        public enum BeingType
        {
            Enemies,
            NPCs,
        }
    }
}
