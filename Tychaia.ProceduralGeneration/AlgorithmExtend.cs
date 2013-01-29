//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerCategory(FlowCategory.ZoomTools)]
    [FlowDesignerName("Extend")]
    public class AlgorithmExtend : Algorithm<int>
    {
        [DataMember]
        [DefaultValue(0)]
        [Description("The value that will be excluded or selected.")]
        public int Value
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(true)]
        [Description("If true extend all except for the given value else extend only the given value.")]
        public bool ExcludeValue
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(true)]
        [Description("Utilized the neighbors values to provide a better output.")]
        public bool NeighborChancing
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(false)]
        [Description("When setting this cell also sets neighbor cells. Makes each iteration expand more.")]
        public bool NeighborExpanding
        {
            get;
            set;
        }

        public override int RequiredXBorder { get { return 1; } }
        public override int RequiredYBorder { get { return 1; } }

        public AlgorithmExtend()
        {
            this.Value = 0;
            this.ExcludeValue = true;
            this.NeighborChancing = true;
            this.NeighborExpanding = false;
        }

        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth)
        {
            int ox = RequiredXBorder;
            int oy = RequiredYBorder;
            long rw = RequiredYBorder * 2 + width;

            // Does this if statement work?
            if ((input[(i + ox) + (j + oy) * rw] == Value && ExcludeValue == true) || (input[(i + ox) + (j + oy) * rw] != Value && ExcludeValue == false))
            {
                if (NeighborChancing == false || context.GetRandomRange(x, y, 1, 100, context.Modifier) < (input[(i + ox + 1) + (j + oy + 1) * rw] + input[(i + ox - 1) + (j + oy - 1) * rw] + input[(i + ox - 1) + (j + oy + 1) * rw] + input[(i + ox + 1) + (j + oy - 1) * rw]))
                {
                    int selected = context.GetRandomRange(x, y, 1, 4, context.Modifier);
                
                    switch (selected)
                    {
                        case 0:
                            output[i + j * width] = input[(i + ox + 1) + (j + oy + 1) * rw];
                            if (NeighborExpanding == true)
                            {
                                if ((input[(i + ox - 1) + (j + oy - 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox - 1) + (j + oy - 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox + 1) + (j + oy + 1) * rw] = input[(i + ox - 1) + (j + oy - 1) * rw];
                                if ((input[(i + ox - 1) + (j + oy + 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox - 1) + (j + oy + 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox + 1) + (j + oy + 1) * rw] = input[(i + ox - 1) + (j + oy + 1) * rw];
                                if ((input[(i + ox + 1) + (j + oy - 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox + 1) + (j + oy - 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox + 1) + (j + oy + 1) * rw] = input[(i + ox + 1) + (j + oy - 1) * rw];
                            }
                            break;
                        case 1:
                            output[i + j * width] = input[(i + ox - 1) + (j + oy - 1) * rw];
                            if (NeighborExpanding == true)
                            {
                                if ((input[(i + ox + 1) + (j + oy + 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox + 1) + (j + oy + 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox - 1) + (j + oy - 1) * rw] = input[(i + ox + 1) + (j + oy + 1) * rw];
                                if ((input[(i + ox - 1) + (j + oy + 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox - 1) + (j + oy + 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox - 1) + (j + oy - 1) * rw] = input[(i + ox - 1) + (j + oy + 1) * rw];
                                if ((input[(i + ox + 1) + (j + oy - 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox + 1) + (j + oy - 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox - 1) + (j + oy - 1) * rw] = input[(i + ox + 1) + (j + oy - 1) * rw];
                            }
                            break;
                        case 2:
                            output[i + j * width] = input[(i + ox - 1) + (j + oy + 1) * rw]; 
                            if (NeighborExpanding == true)
                            {
                                if ((input[(i + ox + 1) + (j + oy + 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox + 1) + (j + oy + 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox - 1) + (j + oy + 1) * rw] = input[(i + ox + 1) + (j + oy + 1) * rw];
                                if ((input[(i + ox - 1) + (j + oy - 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox - 1) + (j + oy - 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox - 1) + (j + oy + 1) * rw] = input[(i + ox - 1) + (j + oy - 1) * rw];
                                if ((input[(i + ox + 1) + (j + oy - 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox + 1) + (j + oy - 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox - 1) + (j + oy + 1) * rw] = input[(i + ox + 1) + (j + oy - 1) * rw];
                            }
                            break;
                        case 3:
                            output[i + j * width] = input[(i + ox + 1) + (j + oy - 1) * rw];
                            if (NeighborExpanding == true)
                            {
                                if ((input[(i + ox + 1) + (j + oy + 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox + 1) + (j + oy + 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox + 1) + (j + oy - 1) * rw] = input[(i + ox + 1) + (j + oy + 1) * rw];
                                if ((input[(i + ox - 1) + (j + oy - 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox - 1) + (j + oy - 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox + 1) + (j + oy - 1) * rw] = input[(i + ox - 1) + (j + oy - 1) * rw];
                                if ((input[(i + ox - 1) + (j + oy + 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox - 1) + (j + oy + 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox + 1) + (j + oy - 1) * rw] = input[(i + ox - 1) + (j + oy + 1) * rw];
                            }
                            break;
                    }
                }
            }
            else 
                output[i + j * width] = input[(i + ox) + (j + oy) * rw];
        }
    }
}

