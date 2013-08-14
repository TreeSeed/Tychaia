// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Manipulation)]
    [FlowDesignerName("Smooth 2D By Input")]
    public class AlgorithmSmooth2DByInput : Algorithm<int, int, int>
    {
        public override int[] RequiredXBorder
        {
            get { return new[] { 2, 2 }; }
        }

        public override int[] RequiredYBorder
        {
            get { return new[] { 2, 2 }; }
        }

        public override int[] RequiredZBorder
        {
            get { return new[] { 0, 0 }; }
        }

        public override bool[] InputWidthAtHalfSize
        {
            get
            {
                return new[]
                {
                    false,
                    false
                };
            }
        }

        public override bool[] InputHeightAtHalfSize
        {
            get
            {
                return new[]
                {
                    false,
                    false
                };
            }
        }

        public override bool[] InputDepthAtHalfSize
        {
            get
            {
                return new[]
                {
                    false,
                    false
                };
            }
        }

        public override string[] InputNames
        {
            get { return new[] { "Input", "Perlin" }; }
        }

        public override bool Is2DOnly
        {
            get { return true; }
        }

        public override void ProcessCell(IRuntimeContext context, int[] inputA, int[] inputB, int[] output, long x,
            long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            var ck = (k + oz) * width * height;
            var iox = i + ox;
            var joy = j + oy;
            var v00 = inputA[(iox - 2) + (joy - 2) * width + ck];
            var v01 = inputA[(iox - 2) + (joy - 1) * width + ck];
            var v02 = inputA[(iox - 2) + (joy + 0) * width + ck];
            var v03 = inputA[(iox - 2) + (joy + 1) * width + ck];
            var v04 = inputA[(iox - 2) + (joy + 2) * width + ck];
            var v10 = inputA[(iox - 1) + (joy - 2) * width + ck];
            var v11 = inputA[(iox - 1) + (joy - 1) * width + ck];
            var v12 = inputA[(iox - 1) + (joy + 0) * width + ck];
            var v13 = inputA[(iox - 1) + (joy + 1) * width + ck];
            var v14 = inputA[(iox - 1) + (joy + 2) * width + ck];
            var v20 = inputA[(iox + 0) + (joy - 2) * width + ck];
            var v21 = inputA[(iox + 0) + (joy - 1) * width + ck];
            var v22 = inputA[(iox + 0) + (joy + 0) * width + ck];
            var v23 = inputA[(iox + 0) + (joy + 1) * width + ck];
            var v24 = inputA[(iox + 0) + (joy + 2) * width + ck];
            var v30 = inputA[(iox + 1) + (joy - 2) * width + ck];
            var v31 = inputA[(iox + 1) + (joy - 1) * width + ck];
            var v32 = inputA[(iox + 1) + (joy + 0) * width + ck];
            var v33 = inputA[(iox + 1) + (joy + 1) * width + ck];
            var v34 = inputA[(iox + 1) + (joy + 2) * width + ck];
            var v40 = inputA[(iox + 2) + (joy - 2) * width + ck];
            var v41 = inputA[(iox + 2) + (joy - 1) * width + ck];
            var v42 = inputA[(iox + 2) + (joy + 0) * width + ck];
            var v43 = inputA[(iox + 2) + (joy + 1) * width + ck];
            var v44 = inputA[(iox + 2) + (joy + 2) * width + ck];
            var p00 = Math.Max(0, inputB[(iox - 2) + (joy - 2) * width + ck]);
            var p01 = Math.Max(0, inputB[(iox - 2) + (joy - 1) * width + ck]);
            var p02 = Math.Max(0, inputB[(iox - 2) + (joy + 0) * width + ck]);
            var p03 = Math.Max(0, inputB[(iox - 2) + (joy + 1) * width + ck]);
            var p04 = Math.Max(0, inputB[(iox - 2) + (joy + 2) * width + ck]);
            var p10 = Math.Max(0, inputB[(iox - 1) + (joy - 2) * width + ck]);
            var p11 = Math.Max(0, inputB[(iox - 1) + (joy - 1) * width + ck]);
            var p12 = Math.Max(0, inputB[(iox - 1) + (joy + 0) * width + ck]);
            var p13 = Math.Max(0, inputB[(iox - 1) + (joy + 1) * width + ck]);
            var p14 = Math.Max(0, inputB[(iox - 1) + (joy + 2) * width + ck]);
            var p20 = Math.Max(0, inputB[(iox + 0) + (joy - 2) * width + ck]);
            var p21 = Math.Max(0, inputB[(iox + 0) + (joy - 1) * width + ck]);
            var p22 = Math.Max(1, inputB[(iox + 0) + (joy + 0) * width + ck]);
            var p23 = Math.Max(0, inputB[(iox + 0) + (joy + 1) * width + ck]);
            var p24 = Math.Max(0, inputB[(iox + 0) + (joy + 2) * width + ck]);
            var p30 = Math.Max(0, inputB[(iox + 1) + (joy - 2) * width + ck]);
            var p31 = Math.Max(0, inputB[(iox + 1) + (joy - 1) * width + ck]);
            var p32 = Math.Max(0, inputB[(iox + 1) + (joy + 0) * width + ck]);
            var p33 = Math.Max(0, inputB[(iox + 1) + (joy + 1) * width + ck]);
            var p34 = Math.Max(0, inputB[(iox + 1) + (joy + 2) * width + ck]);
            var p40 = Math.Max(0, inputB[(iox + 2) + (joy - 2) * width + ck]);
            var p41 = Math.Max(0, inputB[(iox + 2) + (joy - 1) * width + ck]);
            var p42 = Math.Max(0, inputB[(iox + 2) + (joy + 0) * width + ck]);
            var p43 = Math.Max(0, inputB[(iox + 2) + (joy + 1) * width + ck]);
            var p44 = Math.Max(0, inputB[(iox + 2) + (joy + 2) * width + ck]);

            var sample = new[,]
            {
                { v00, v01, v02, v03, v04 },
                { v10, v11, v12, v13, v14 },
                { v20, v21, v22, v23, v24 },
                { v30, v31, v32, v33, v34 },
                { v40, v41, v42, v43, v44 }
            };
            var applier = new[,]
            {
                { p00, p01, p02, p03, p04 },
                { p10, p11, p12, p13, p14 },
                { p20, p21, p22, p23, p24 },
                { p30, p31, p32, p33, p34 },
                { p40, p41, p42, p43, p44 }
            };

            var result = 0;
            var total = 0;
            var storage = new int[5, 5];

            foreach (var v in applier)
                total += v;
            for (var ii = 0; ii < 5; ii++)
                for (var jj = 0; jj < 5; jj++)
                    storage[ii, jj] = sample[ii, jj] * applier[ii, jj];
            foreach (var v in storage)
                result += v;

            var rounded = (int) (result / (double) total);
            if (rounded < 0)
                rounded = v22;
            output[i + ox + (j + oy) * width + (k + oz) * width * height] = rounded;
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
