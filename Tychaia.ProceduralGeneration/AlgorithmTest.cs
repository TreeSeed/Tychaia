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
    [FlowDesignerCategory(FlowCategory.Debugging)]
    [FlowDesignerName("Passthrough")]
    public class AlgorithmPassthrough : Algorithm<int, int>
    {
        public override int[] RequiredXBorder { get { return new int[] {this.XBorder}; } }
        public override int[] RequiredYBorder { get { return new int[] {this.YBorder}; } }
        public override bool[] InputWidthAtHalfSize { get { return new bool[] {WidthHalf}; } }
        public override bool[] InputHeightAtHalfSize { get { return new bool[] {HeightHalf}; } }

        public int XBorder { get; set; }
        public int YBorder { get; set; }
        public bool WidthHalf { get; set; }
        public bool HeightHalf { get; set; }

        public override string[] InputNames
        {
            get { return new string[] { "Input" }; }
        }

        public override bool Is2DOnly
        {
            get { return false; }
        }

        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth)
        {
            output[i + j * width + k * width * height] = input[i + j * width + k * width * height];
        }
        
        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }

    [DataContract]
    [FlowDesignerCategory(FlowCategory.Debugging)]
    [FlowDesignerName("Multi Passthrough")]
    public class AlgorithmMultiPassthrough : Algorithm<int, int, int, int>
    {
        public override int[] RequiredXBorder { get { return new int[] {this.XBorderA, this.XBorderB, 0}; } }
        public override int[] RequiredYBorder { get { return new int[] {this.YBorderA, this.YBorderB, 0}; } }
        public override bool[] InputWidthAtHalfSize { get { return new bool[] {WidthHalfA, false, false}; } }
        public override bool[] InputHeightAtHalfSize { get { return new bool[] {HeightHalfA, false, false}; } }
        
        public int XBorderA { get; set; }
        public int YBorderA { get; set; }
        public int XBorderB { get; set; }
        public int YBorderB { get; set; }
        public bool WidthHalfA { get; set; }
        public bool HeightHalfA { get; set; }
        
        public override string[] InputNames
        {
            get { return new string[] { "Input A", "Input B", "Input C" }; }
        }
        
        public override bool Is2DOnly
        {
            get { return false; }
        }
        
        public override void ProcessCell(IRuntimeContext context, int[] inputA, int[] inputB, int[] inputC, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth)
        {
            output[i + j * width + k * width * height] = inputA[i + j * width + k * width * height];
        }
        
        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}

