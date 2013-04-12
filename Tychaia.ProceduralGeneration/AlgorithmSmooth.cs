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
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Zooming)]
    [FlowDesignerName("Smooth 2D")]
    public class AlgorithmSmooth2D : Algorithm<int, int>
    {
        [DataMember]
        [DefaultValue(SmoothType.Linear)]
        [Description("The smoothing algorithm to use.")]
        public SmoothType Mode
        {
            get;
            set;
        }
        
        public override int[] RequiredXBorder { get { return new int[] {2}; } }
        public override int[] RequiredYBorder { get { return new int[] {2}; } }
        public override int[] RequiredZBorder { get { return new int[] {0}; } }
        public override bool[] InputWidthAtHalfSize { get { return new bool[] {false}; } }
        public override bool[] InputHeightAtHalfSize { get { return new bool[] {false}; } }
        public override bool[] InputDepthAtHalfSize { get { return new bool[] {false}; } }
        
        public AlgorithmSmooth2D()
        {
            this.Mode = SmoothType.Linear;
        }
        
        public override string[] InputNames
        {
            get { return new string[] { "Input" }; }
        }
        
        public override bool Is2DOnly
        {
            get { return true; }
        }
        
        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            var ck = (k + oz) * width * height;
            var iox = i + ox;
            var joy = j + oy;
            var v00 = input[(iox - 2) + (joy - 2) * width + ck];
            var v01 = input[(iox - 2) + (joy - 1) * width + ck];
            var v02 = input[(iox - 2) + (joy + 0) * width + ck];
            var v03 = input[(iox - 2) + (joy + 1) * width + ck];
            var v04 = input[(iox - 2) + (joy + 2) * width + ck];
            var v10 = input[(iox - 1) + (joy - 2) * width + ck];
            var v11 = input[(iox - 1) + (joy - 1) * width + ck];
            var v12 = input[(iox - 1) + (joy + 0) * width + ck];
            var v13 = input[(iox - 1) + (joy + 1) * width + ck];
            var v14 = input[(iox - 1) + (joy + 2) * width + ck];
            var v20 = input[(iox + 0) + (joy - 2) * width + ck];
            var v21 = input[(iox + 0) + (joy - 1) * width + ck];
            var v22 = input[(iox + 0) + (joy + 0) * width + ck];
            var v23 = input[(iox + 0) + (joy + 1) * width + ck];
            var v24 = input[(iox + 0) + (joy + 2) * width + ck];
            var v30 = input[(iox + 1) + (joy - 2) * width + ck];
            var v31 = input[(iox + 1) + (joy - 1) * width + ck];
            var v32 = input[(iox + 1) + (joy + 0) * width + ck];
            var v33 = input[(iox + 1) + (joy + 1) * width + ck];
            var v34 = input[(iox + 1) + (joy + 2) * width + ck];
            var v40 = input[(iox + 2) + (joy - 2) * width + ck];
            var v41 = input[(iox + 2) + (joy - 1) * width + ck];
            var v42 = input[(iox + 2) + (joy + 0) * width + ck];
            var v43 = input[(iox + 2) + (joy + 1) * width + ck];
            var v44 = input[(iox + 2) + (joy + 2) * width + ck];
            
            var result = 0;
            var total = 0;
            var applier = new int[5, 5];
            switch (this.Mode)
            {
                case SmoothType.None:
                    applier = new int[,]
                    {
                        { 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0 },
                        { 0, 0, 1, 0, 0 },
                        { 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0 }
                    };
                    break;
                case SmoothType.Linear:
                    applier = new int[,]
                    {
                        { 1, 1, 1, 1, 1 },
                        { 1, 1, 1, 1, 1 },
                        { 1, 1, 1, 1, 1 },
                        { 1, 1, 1, 1, 1 },
                        { 1, 1, 1, 1, 1 }
                    };
                    break;
                case SmoothType.Parabolic:
                    applier = new int[,]
                    {
                        { 1, 4, 9, 4, 1 },
                        { 4, 9, 16, 9, 4 },
                        { 9, 16, 25, 16, 9 },
                        { 4, 9, 16, 9, 4 },
                        { 1, 4, 9, 4, 1 }
                    };
                    break;
                case SmoothType.Cubic:
                    applier = new int[,]
                    {
                        { 1, 8, 27, 8, 1 },
                        { 8, 27, 64, 27, 8 },
                        { 27, 64, 125, 64, 27 },
                        { 8, 27, 64, 27, 8 },
                        { 1, 8, 27, 8, 1 }
                    };
                    break;
            }
            var sample = new int[,]
            {
                { v00, v01, v02, v03, v04 },
                { v10, v11, v12, v13, v14 },
                { v20, v21, v22, v23, v24 },
                { v30, v31, v32, v33, v34 },
                { v40, v41, v42, v43, v44 }
            };
            var storage = new int[5, 5];

            foreach (int v in applier)
                total += v;
            for (var ii = 0; ii < 5; ii++)
                for (var jj = 0; jj < 5; jj++)
                    storage[ii, jj] = sample[ii, jj] * applier[ii, jj];
            foreach (var v in storage)
                result += v;
            
            output[i + ox + (j + oy) * width + (k + oz) * width * height] = (int)((double)result / (double)total);
        }
        
        /// <summary>
        /// An enumeration defining the type of smooth to perform.
        /// </summary>
        public enum SmoothType
        {
            None,
            Linear,
            Parabolic,
            Cubic,
        }
        
        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}

