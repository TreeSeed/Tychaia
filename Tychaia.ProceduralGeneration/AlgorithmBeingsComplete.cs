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
    [FlowDesignerName("Complete Beings")]
    public class AlgorithmBeingsComplete : Algorithm<Cell, Cell>
    {
        public AlgorithmBeingsComplete()
        {
        }
        
        public override string[] InputNames
        {
            get
            {
                return new[] { "Beings" };
            }
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
            Cell[] input,
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
            var outputCell = new Cell();
            var current = input[(i + ox) + ((j + oy) * width)];

            if (current.ClusterComplete)
            {
                var a = -1;
                if (current.Count0 == 1)
                    a = 0;
                else if (current.Count1 == 1)
                    a = 1;
                else if (current.Count2 == 1)
                    a = 2;
                else if (current.Count3 == 1)
                    a = 3;
                else if (current.Count4 == 1)
                    a = 4;
                else if (current.Count5 == 1)
                    a = 5;
                else if (current.Count6 == 1)
                    a = 6;
                else if (current.Count7 == 1)
                    a = 7;
                else if (current.Count8 == 1)
                    a = 8;
                else if (current.Count9 == 1)
                    a = 9;

                if (a != -1)
                {
                    outputCell.BeingDefinitionAssetName = context.AssetManager.Get<BeingClusterDefinitionAsset>(current.ClusterDefinitionAssetName).BeingDefinitions[a].Name;
                    outputCell.BeingHealth = context.AssetManager.Get<BeingDefinitionAsset>(outputCell.BeingDefinitionAssetName).HealthPerLevel * current.ClusterLevel;
                }
            }

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
