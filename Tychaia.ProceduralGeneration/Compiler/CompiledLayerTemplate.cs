//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Tychaia.ProceduralGeneration;
/****** %USING% ******/

public class CompiledLayer : IRuntimeContext, IGenerator
{
    /// <summary>
    /// The modifier used by algorithms as an additional input to the
    /// random function calls.
    /// </summary>
    public long Modifier
    {
        get;
        set;
    }

    #region AUTO-GENERATED CODE

    public dynamic GenerateData(long x, long y, int width, int height)
    {
        return this.GenerateData(x, y, 0, width, height, 1);
    }

    public dynamic GenerateData(long __compiled_x, long __compiled_y, long __compiled_z, int __compiled_width, int __compiled_height, int __compiled_depth)
    {
        //
        // This is auto-generated code.  Things to observe:
        //   * Properties have been evaluated at compile time and hard-coded
        //     into the result.
        //   * References to context have been updated to use this instead.
        //   * Auto-detection of required arrays and their types to support
        //     quickly inlining of code and reduction in the array creation.
        //

        /****** %CODE% ******/
    }

    #endregion

    #region Randomness
    
    private long m_Seed;
    
    /// <summary>
    /// The world seed.
    /// </summary>
    public long Seed
    {
        get
        {
            return this.m_Seed;
        }
        set
        {
            this.m_Seed = value;
        }
    }
    
    /// <summary>
    /// Returns a random positive integer between the specified 0 and
    /// the exclusive end value.
    /// </summary>
    public int GetRandomRange(long x, long y, long z, int end, long modifier = 0)
    {
        unchecked
        {
            int a = this.GetRandomInt(x, y, z, modifier);
            if (a < 0)
                a += int.MaxValue;
            return a % end;
        }
    }
    
    /// <summary>
    /// Returns a random positive integer between the specified inclusive start
    /// value and the exclusive end value.
    /// </summary>
    public int GetRandomRange(long x, long y, long z, int start, int end, long modifier)
    {
        unchecked
        {
            int a = this.GetRandomInt(x, y, z, modifier);
            if (a < 0)
                a += int.MaxValue;
            return a % (end - start) + start;
        }
    }
    
    /// <summary>
    /// Returns a random integer over the range of valid integers based
    /// on the provided X and Y position, and the specified modifier.
    /// </summary>
    public int GetRandomInt(long x, long y, long z, long modifier = 0)
    {
        unchecked
        {
            return (int)(this.GetRandomNumber(x, y, z, modifier) % int.MaxValue);
        }
    }
    
    /// <summary>
    /// Returns a random long integer over the range of valid long integers based
    /// on the provided X and Y position, and the specified modifier.
    /// </summary>
    public long GetRandomLong(long x, long y, long z, long modifier = 0)
    {
        return this.GetRandomNumber(x, y, z, modifier);
    }
    
    /// <summary>
    /// Returns a random double between the range of 0.0 and 1.0 based on
    /// the provided X and Y position, and the specified modifier.
    /// </summary>
    public double GetRandomDouble(long x, long y, long z, long modifier = 0)
    {
        long a = this.GetRandomNumber(x, y, z, modifier) / 2;
        if (a < 0)
            a += long.MaxValue;
        return (double)a / (double)long.MaxValue;
    }
    
    private long GetRandomNumber(long x, long y, long z, long modifier)
    {
        /* From: http://stackoverflow.com/questions/2890040/implementing-gethashcode
         * Although we aren't implementing GetHashCode, it's still a good way to generate
         * a unique number given a limited set of fields */
        long seed = (x - 1) * 3661988493967 + (y - 1);
        seed += (x - 2) * 2990430311017;
        seed *= (y - 3) * 14475080218213;
        seed += modifier;
        seed += (y - 4) * 28124722524383;
        seed += (z - 5) * 25905201761893;
        seed *= (x - 6) * 16099760261113;
        seed += (x - 7) * this.m_Seed;
        seed *= (y - 8) * this.m_Seed;
        seed += (z - 9) * 55497960863;
        seed *= (z - 10) * 611286883423;
        seed += modifier;
        // Prevents the seed from being 0 along an axis.
        seed += (x - 199) * (y - 241) * (z - 1471) * 9018110272013;
            
        return seed;
    }
    
    #endregion

}

