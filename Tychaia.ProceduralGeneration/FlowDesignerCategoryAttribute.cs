//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.ComponentModel;

namespace Tychaia.ProceduralGeneration
{
    public enum FlowMajorCategory
    {
        [Description("General 2D")]
        General2D,
        [Description("Specific 2D")]
        Specific2D,
        [Description("General 3D")]
        General3D,
        [Description("Specific 3D")]
        Specific3D,
        Undefined
    }

    public enum FlowCategory
    {
        // These are general tools
        General,
        Initials,
        Zooming,
        Manipulation,
        Debugging,
        Output,
        Undefined,
        // Want to have a seperate heading for anything more specific catergories
        Land,
        Biome, // Required
        Rainfall,
        Temperature,
        Rivers,
        Trees,
        Towns,
        FamilyTrees // Haven't thought this thru layer wise yet
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
    }

    public class FlowDesignerMajorCategoryAttribute : Attribute
    {
        public FlowMajorCategory MajorCategory
        {
            get;
            private set;
        }
        
        public FlowDesignerMajorCategoryAttribute(FlowMajorCategory majorcategory)
        {
            this.MajorCategory = majorcategory;
        }        
    }
}

