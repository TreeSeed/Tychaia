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

        public override bool[] InputIs2D
        {
            get { return new[] { true, true }; }
        }

        public override bool Is2DOnly
        {
            get { return true; }
        }

        public override void ProcessCell(
            IRuntimeContext context,
            int[] inputA,
            int[] inputB,
            int[] output,
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
            var ck = (k + oz) * width * height;
            var iox = i + ox;
            var joy = j + oy;
            var v00 = inputA[(iox - 2) + ((joy - 2) * width) + ck];
            var v01 = inputA[(iox - 2) + ((joy - 1) * width) + ck];
            var v02 = inputA[(iox - 2) + ((joy + 0) * width) + ck];
            var v03 = inputA[(iox - 2) + ((joy + 1) * width) + ck];
            var v04 = inputA[(iox - 2) + ((joy + 2) * width) + ck];
            var v10 = inputA[(iox - 1) + ((joy - 2) * width) + ck];
            var v11 = inputA[(iox - 1) + ((joy - 1) * width) + ck];
            var v12 = inputA[(iox - 1) + ((joy + 0) * width) + ck];
            var v13 = inputA[(iox - 1) + ((joy + 1) * width) + ck];
            var v14 = inputA[(iox - 1) + ((joy + 2) * width) + ck];
            var v20 = inputA[(iox + 0) + ((joy - 2) * width) + ck];
            var v21 = inputA[(iox + 0) + ((joy - 1) * width) + ck];
            var v22 = inputA[(iox + 0) + ((joy + 0) * width) + ck];
            var v23 = inputA[(iox + 0) + ((joy + 1) * width) + ck];
            var v24 = inputA[(iox + 0) + ((joy + 2) * width) + ck];
            var v30 = inputA[(iox + 1) + ((joy - 2) * width) + ck];
            var v31 = inputA[(iox + 1) + ((joy - 1) * width) + ck];
            var v32 = inputA[(iox + 1) + ((joy + 0) * width) + ck];
            var v33 = inputA[(iox + 1) + ((joy + 1) * width) + ck];
            var v34 = inputA[(iox + 1) + ((joy + 2) * width) + ck];
            var v40 = inputA[(iox + 2) + ((joy - 2) * width) + ck];
            var v41 = inputA[(iox + 2) + ((joy - 1) * width) + ck];
            var v42 = inputA[(iox + 2) + ((joy + 0) * width) + ck];
            var v43 = inputA[(iox + 2) + ((joy + 1) * width) + ck];
            var v44 = inputA[(iox + 2) + ((joy + 2) * width) + ck];
            var p00 = Math.Max(0, inputB[(iox - 2) + ((joy - 2) * width) + ck]);
            var p01 = Math.Max(0, inputB[(iox - 2) + ((joy - 1) * width) + ck]);
            var p02 = Math.Max(0, inputB[(iox - 2) + ((joy + 0) * width) + ck]);
            var p03 = Math.Max(0, inputB[(iox - 2) + ((joy + 1) * width) + ck]);
            var p04 = Math.Max(0, inputB[(iox - 2) + ((joy + 2) * width) + ck]);
            var p10 = Math.Max(0, inputB[(iox - 1) + ((joy - 2) * width) + ck]);
            var p11 = Math.Max(0, inputB[(iox - 1) + ((joy - 1) * width) + ck]);
            var p12 = Math.Max(0, inputB[(iox - 1) + ((joy + 0) * width) + ck]);
            var p13 = Math.Max(0, inputB[(iox - 1) + ((joy + 1) * width) + ck]);
            var p14 = Math.Max(0, inputB[(iox - 1) + ((joy + 2) * width) + ck]);
            var p20 = Math.Max(0, inputB[(iox + 0) + ((joy - 2) * width) + ck]);
            var p21 = Math.Max(0, inputB[(iox + 0) + ((joy - 1) * width) + ck]);
            var p22 = Math.Max(1, inputB[(iox + 0) + ((joy + 0) * width) + ck]);
            var p23 = Math.Max(0, inputB[(iox + 0) + ((joy + 1) * width) + ck]);
            var p24 = Math.Max(0, inputB[(iox + 0) + ((joy + 2) * width) + ck]);
            var p30 = Math.Max(0, inputB[(iox + 1) + ((joy - 2) * width) + ck]);
            var p31 = Math.Max(0, inputB[(iox + 1) + ((joy - 1) * width) + ck]);
            var p32 = Math.Max(0, inputB[(iox + 1) + ((joy + 0) * width) + ck]);
            var p33 = Math.Max(0, inputB[(iox + 1) + ((joy + 1) * width) + ck]);
            var p34 = Math.Max(0, inputB[(iox + 1) + ((joy + 2) * width) + ck]);
            var p40 = Math.Max(0, inputB[(iox + 2) + ((joy - 2) * width) + ck]);
            var p41 = Math.Max(0, inputB[(iox + 2) + ((joy - 1) * width) + ck]);
            var p42 = Math.Max(0, inputB[(iox + 2) + ((joy + 0) * width) + ck]);
            var p43 = Math.Max(0, inputB[(iox + 2) + ((joy + 1) * width) + ck]);
            var p44 = Math.Max(0, inputB[(iox + 2) + ((joy + 2) * width) + ck]);

            var total = 
                p00 + p01 + p02 + p03 + p04 +
                p10 + p11 + p12 + p13 + p14 +
                p20 + p21 + p22 + p23 + p24 +
                p30 + p31 + p32 + p33 + p34 +
                p40 + p41 + p42 + p43 + p44;
            
            var s00 = v00 * p00;
            var s01 = v01 * p01;
            var s02 = v02 * p02;
            var s03 = v03 * p03;
            var s04 = v04 * p04;
            var s10 = v10 * p10;
            var s11 = v11 * p11;
            var s12 = v12 * p12;
            var s13 = v13 * p13;
            var s14 = v14 * p14;
            var s20 = v20 * p20;
            var s21 = v21 * p21;
            var s22 = v22 * p22;
            var s23 = v23 * p23;
            var s24 = v24 * p24;
            var s30 = v30 * p30;
            var s31 = v31 * p31;
            var s32 = v32 * p32;
            var s33 = v33 * p33;
            var s34 = v34 * p34;
            var s40 = v40 * p40;
            var s41 = v41 * p41;
            var s42 = v42 * p42;
            var s43 = v43 * p43;
            var s44 = v44 * p44;
            
            var result = 
                s00 + s01 + s02 + s03 + s04 +
                s10 + s11 + s12 + s13 + s14 +
                s20 + s21 + s22 + s23 + s24 +
                s30 + s31 + s32 + s33 + s34 +
                s40 + s41 + s42 + s43 + s44;
            
            var rounded = (int)(result / (double)total);
            if (rounded < 0)
                rounded = v22;
            output[i + ox + ((j + oy) * width) + ((k + oz) * width * height)] = rounded;
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}
