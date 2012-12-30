//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;

namespace Tychaia.ProceduralGeneration
{
    public interface IRuntimeContext
    {
        /// <summary>
        /// The modifier used by algorithms as an additional input to the
        /// random function calls.
        /// </summary>
        long Modifier { get; }

        /// <summary>
        /// Returns a random positive integer between the specified 0 and
        /// the exclusive end value.
        /// </summary>
        int GetRandomRange(long x, long y, long z, int end, long modifier = 0);
        
        /// <summary>
        /// Returns a random positive integer between the specified inclusive start
        /// value and the exclusive end value.
        /// </summary>
        int GetRandomRange(long x, long y, long z, int start, int end, long modifier);
        
        /// <summary>
        /// Returns a random integer over the range of valid integers based
        /// on the provided X and Y position, and the specified modifier.
        /// </summary>
        int GetRandomInt(long x, long y, long z, long modifier = 0);
        
        /// <summary>
        /// Returns a random long integer over the range of valid long integers based
        /// on the provided X and Y position, and the specified modifier.
        /// </summary>
        long GetRandomLong(long x, long y, long z, long modifier = 0);
        
        /// <summary>
        /// Returns a random double between the range of 0.0 and 1.0 based on
        /// the provided X and Y position, and the specified modifier.
        /// </summary>
        double GetRandomDouble(long x, long y, long z, long modifier = 0);

        int FindZoomedPoint(int[] parent, long i, long j, long ox, long oy, long x, long y, long rw);
        int Smooth(long x, long y, int northValue, int southValue, int westValue, int eastValue, int currentValue, long i, long j, long ox, long oy, long rw, int[] parent);
    }
}

