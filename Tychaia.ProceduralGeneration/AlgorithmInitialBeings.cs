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
using System.Linq;

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
            var BeingClusterList = (BeingClusterDefinitionAsset[])context.AssetManager.GetAll().OfType<BeingClusterDefinitionAsset>().Where(b => b.Enemy == true).Where(a => a.LevelRequirement < input[(i + ox) + ((j + oy) * width)]).ToArray();

            var outputCell = new Cell();


            if (!(this.GuaranteeStartingPoint && x == 0 && y == 0) && BeingClusterList.Count() > 0 && AlgorithmUtility.GetRandomDouble(context.Seed, x, y, 0, context.Modifier) > this.Limit)
            {
                var a = AlgorithmUtility.GetRandomRange(context.Seed, x, y, 0, BeingClusterList.Count(), context.Modifier);
                outputCell.ClusterDefinitionAssetName = BeingClusterList[a].Name;
                outputCell.Count0 = AlgorithmUtility.GetRandomRange(context.Seed, x, y, BeingClusterList[a].Minimum[0], BeingClusterList[a].Maximum[0] + 1, context.Modifier);
                outputCell.Count1 = AlgorithmUtility.GetRandomRange(context.Seed, x, y, BeingClusterList[a].Minimum[1], BeingClusterList[a].Maximum[1] + 1, context.Modifier);
                outputCell.Count2 = AlgorithmUtility.GetRandomRange(context.Seed, x, y, BeingClusterList[a].Minimum[2], BeingClusterList[a].Maximum[2] + 1, context.Modifier);
                outputCell.Count3 = AlgorithmUtility.GetRandomRange(context.Seed, x, y, BeingClusterList[a].Minimum[3], BeingClusterList[a].Maximum[3] + 1, context.Modifier);
                outputCell.Count4 = AlgorithmUtility.GetRandomRange(context.Seed, x, y, BeingClusterList[a].Minimum[4], BeingClusterList[a].Maximum[4] + 1, context.Modifier);
                outputCell.Count5 = AlgorithmUtility.GetRandomRange(context.Seed, x, y, BeingClusterList[a].Minimum[5], BeingClusterList[a].Maximum[5] + 1, context.Modifier);
                outputCell.Count6 = AlgorithmUtility.GetRandomRange(context.Seed, x, y, BeingClusterList[a].Minimum[6], BeingClusterList[a].Maximum[6] + 1, context.Modifier);
                outputCell.Count7 = AlgorithmUtility.GetRandomRange(context.Seed, x, y, BeingClusterList[a].Minimum[7], BeingClusterList[a].Maximum[7] + 1, context.Modifier);
                outputCell.Count8 = AlgorithmUtility.GetRandomRange(context.Seed, x, y, BeingClusterList[a].Minimum[8], BeingClusterList[a].Maximum[8] + 1, context.Modifier);
                outputCell.Count9 = AlgorithmUtility.GetRandomRange(context.Seed, x, y, BeingClusterList[a].Minimum[9], BeingClusterList[a].Maximum[9] + 1, context.Modifier);
            }

            if (outputCell.Count0 + 
                outputCell.Count1 +
                outputCell.Count2 +
                outputCell.Count3 +
                outputCell.Count4 +
                outputCell.Count5 +
                outputCell.Count6 +
                outputCell.Count7 +
                outputCell.Count8 +
                outputCell.Count9 == 1)
                outputCell.ClusterComplete = true;

            output[(i + ox) + ((j + oy) * width)] = outputCell;
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
