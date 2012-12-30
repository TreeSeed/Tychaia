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

    public dynamic GenerateData(long x, long y, long z, int width, int height, int depth)
    {
        //
        // This is auto-generated code.  Things to observe:
        //   * Properties have been evaluated at compile time and hard-coded
        //     into the result.
        //   * References to context have been updated to use this instead.
        //   * Auto-detection of required arrays and their types to support
        //     quickly inlining of code and reduction in the array creation.
        //

        /****** %DECLS% ******/

        //for (var k = 0; k < depth; k++)
        //    for (var i = 0; i < width; i++)
        //        for (var j = 0; j < height; j++)
        //        {
        /****** %CODE% ******/
        //        }

        /****** %RETURN% ******/
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
    
    #region TESTING EXTRA
    
    public int FindZoomedPoint(int[] parent, long i, long j, long ox, long oy, long x, long y, long rw)
    {
        int ocx = (x % 2 != 0 && i % 2 != 0 ? (i < 0 ? -1 : 1) : 0);
        int ocy = (y % 2 != 0 && j % 2 != 0 ? (j < 0 ? -1 : 1) : 0);
        
        return parent[(i / 2 + ox + ocx) + (j / 2 + oy + ocy) * rw];
    }
    
    public int Smooth(long x, long y, int northValue, int southValue, int westValue, int eastValue, int currentValue, long i, long j, long ox, long oy, long rw, int[] parent)
    {
        // Parent-based Smoothing
        int selected = 0;
        
        if (x % 2 == 0)
        {
            if (y % 2 == 0)
            {
                return currentValue;
            }
            else
            {
                selected = this.GetRandomRange(x, y, 0, 2);
                switch (selected)
                {
                    case 0:
                        return currentValue;
                    case 1:
                        return southValue;
                }
            }
        }
        else
        {
            if (y % 2 == 0)
            {
                selected = this.GetRandomRange(x, y, 0, 2);
                switch (selected)
                {
                    case 0:
                        return currentValue;
                    case 1:
                        return eastValue;
                }
            }
            else
            {
                if (true)//mode == AlgorithmZoom.ZoomType.Smooth)
                {
                    selected = this.GetRandomRange(x, y, 0, 3);
                    switch (selected)
                    {
                        case 0:
                            return currentValue;
                        case 1:
                            return southValue;
                        case 2:
                            return eastValue;
                    }
                }
                else
                {
                    selected = this.GetRandomRange(x, y, 0, 4);
                    switch (selected)
                    {
                        case 0:
                            return currentValue;
                        case 1:
                            return southValue;
                        case 2:
                            return eastValue;
                        case 3:
                            return this.FindZoomedPoint(parent, i + 2, j + 2, ox, oy, x - i, y - j, rw);
                    }
                }
            }
        }
        
        // Select one of the four options if we couldn't otherwise
        // determine a value.
        selected = this.GetRandomRange(x, y, 0, 4);
        
        switch (selected)
        {
            case 0:
                return northValue;
            case 1:
                return southValue;
            case 2:
                return eastValue;
            case 3:
                return westValue;
        }
        
        throw new InvalidOperationException();
    }
    
#endregion
}

