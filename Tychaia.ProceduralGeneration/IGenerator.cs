//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;

namespace Tychaia.ProceduralGeneration
{
    public interface IGenerator
    {
        long Seed { get; set; }

        dynamic GenerateData(long x, long y, long z, int width, int height, int depth);
    }
}

