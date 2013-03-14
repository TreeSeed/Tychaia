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

