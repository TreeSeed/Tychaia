//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.ComponentModel;
using System.Reflection;

namespace Tychaia.ProceduralGeneration
{
    public enum FlowMajorCategory
    {
        [Description("General Layers")]
        General,
		/*[Description("Specific Layers")]
        Specific,*/
		[Description("Flow Bundles")]
		FlowBundle,
        Undefined
    }

    public enum FlowCategory
    {
        // General Layers
        General,
        Initials,
        [Description("Chanced Initials")]
        Chanced,
        Zooming,
        Manipulation,
        Debugging,

		//FlowBundles Options
		Add,
		Extract,

        Undefined,
        [Description("Bugged Layers")]
        Buggy,

        // Specific Layers
        Biomes, // Includes things such as BiomeToTerrain, BiomeToColor, BiomeToTreeDensity
        NPCs,
        Terrain,

        // Not yet implemented / not needed
        Rivers,
        Trees,
        Towns,
        FamilyTrees,

		//Output should always be at the bottom
		Output
    }

    public class FlowDesignerCategoryAttribute : Attribute
    {
        public FlowCategory Category
        {
            get;
            private set;
        }

        public FlowDesignerCategoryAttribute(FlowCategory category)
        {
            this.Category = category;
        }

        public static string GetDescription(FlowCategory value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }

    public class FlowDesignerMajorCategoryAttribute : Attribute
    {
        public FlowMajorCategory MajorCategory
        {
            get;
            private set;
        }

        // Can actually move this somewhere else.
        // Retrieved from http://blog.spontaneouspublicity.com/associating-strings-with-enums-in-c
        public static string GetDescription(FlowMajorCategory value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public FlowDesignerMajorCategoryAttribute(FlowMajorCategory majorcategory)
        {
            this.MajorCategory = majorcategory;
        }
    }
}

