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

    public CompiledLayer()
    {
        this.Seed = 0;
        this.Modifier = 0;
    }

    #region AUTO-GENERATED CODE

    public dynamic GenerateData(long x, long y, long z, int width, int height, int depth, out int computations)
    {
        //
        // This is auto-generated code.  Things to observe:
        //   * Properties have been evaluated at compile time and hard-coded
        //     into the result.
        //   * References to context have been updated to use this instead.
        //   * Auto-detection of required arrays and their types to support
        //     quickly inlining of code and reduction in the array creation.
        //
        computations = 0;

        /****** %DECLS% ******/

        /****** %CODE% ******/

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
        private set
        {
            this.m_Seed = value;
        }
    }

    /// <summary>
    /// Sets the seed of this compiled layer.
    /// </summary>
    /// <param name="seed">Seed.</param>
    public void SetSeed(long seed)
    {
        this.Seed = seed;
    }
    
    /// <summary>
    /// Returns a random positive integer between the specified 0 and
    /// the exclusive end value.
    /// </summary>
    public int GetRandomRange(long x, long y, long z, int end, long modifier = 0)
    {
        return AlgorithmUtility.GetRandomRange(this.Seed, x, y, z, end, modifier);
    }
    
    /// <summary>
    /// Returns a random positive integer between the specified inclusive start
    /// value and the exclusive end value.
    /// </summary>
    public int GetRandomRange(long x, long y, long z, int start, int end, long modifier)
    {
        return AlgorithmUtility.GetRandomRange(this.Seed, x, y, z, start, end, modifier);
    }
    
    /// <summary>
    /// Returns a random integer over the range of valid integers based
    /// on the provided X and Y position, and the specified modifier.
    /// </summary>
    public int GetRandomInt(long x, long y, long z, long modifier = 0)
    {
        return AlgorithmUtility.GetRandomInt(this.Seed, x, y, z, modifier);
    }
    
    /// <summary>
    /// Returns a random long integer over the range of valid long integers based
    /// on the provided X and Y position, and the specified modifier.
    /// </summary>
    public long GetRandomLong(long x, long y, long z, long modifier = 0)
    {
        return AlgorithmUtility.GetRandomLong(this.Seed, x, y, z, modifier);
    }
    
    /// <summary>
    /// Returns a random double between the range of 0.0 and 1.0 based on
    /// the provided X and Y position, and the specified modifier.
    /// </summary>
    public double GetRandomDouble(long x, long y, long z, long modifier = 0)
    {
        return AlgorithmUtility.GetRandomDouble(this.Seed, x, y, z, modifier);
    }
    
    #endregion
    
    #region Other
    
    /// <summary>
    /// Smoothes the specified data according to smoothing logic.  Apparently
    /// inlining this functionality causes the algorithms to run slower, so we
    /// leave this function on it's own.
    /// </summary>
    public int Smooth(bool isFuzzy, long x, long y, int northValue, int southValue, int westValue, int eastValue, int southEastValue, int currentValue, long i, long j, long ox, long oy, long rw, int[] parent)
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
                if (!isFuzzy)
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
                            return southEastValue;
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

