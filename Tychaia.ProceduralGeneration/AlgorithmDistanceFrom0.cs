//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
// This layer is used for the gradual increase of monster difficulty 
// as well as increases and decreases to temperature according to height.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Initials)]
    [FlowDesignerName("Distance from 0")]
    public class AlgorithmDistanceFrom0 : Algorithm<int>
    {
        [DataMember]
        [DefaultValue(true)]
        [Description("Calculate distance from X axis.")]
        public bool Xcalculation
        {
            get;
            set;
        }        

        [DataMember]
        [DefaultValue(true)]
        [Description("Calculate distance from Y axis.")]
        public bool Ycalculation
        {
            get;
            set;
        }        

        [DataMember]
        [DefaultValue(true)]
        [Description("Calculate distance from Z axis.")]
        public bool Zcalculation
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(20)]
        [Description("Divides X value by this much.")]
        public long Xdivider
        {
            get;
            set;
        }        

        [DataMember]
        [DefaultValue(20)]
        [Description("Divides Y value by this much.")]
        public long Ydivider
        {
            get;
            set;
        }        

        [DataMember]
        [DefaultValue(20)]
        [Description("Divides Z value by this much.")]
        public long Zdivider
        {
            get;
            set;
        }

        public override bool Is2DOnly
        {
            get { return false; }
        }

        public AlgorithmDistanceFrom0()
        {
            this.Xcalculation = true;
            this.Ycalculation = true;
            this.Zcalculation = true;
            this.Xdivider = 20;
            this.Ydivider = 20;
            this.Zdivider = 20;
        }

        public override void ProcessCell(IRuntimeContext context, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {

            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = 0;
            
            if (Xcalculation)
            {
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] += ((int)(Math.Abs(x) / Xdivider) > int.MaxValue ? int.MaxValue : (int)(Math.Abs(x) / Xdivider));
            }
            if (Ycalculation)
            {
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] += ((int)(Math.Abs(y) / Ydivider) > int.MaxValue ? int.MaxValue : (int)(Math.Abs(y) / Ydivider));
            }
            if (Zcalculation)
            {
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] += ((int)(Math.Abs(z) / Zdivider) > int.MaxValue ? int.MaxValue : (int)(Math.Abs(z) / Zdivider));
            }
        }
        
        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            int a = value;

            if (a > 255)
                a = 255;
            
            return Color.FromArgb(a, a, a);
        }
    }
}

