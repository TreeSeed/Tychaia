// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
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

    public event DataGeneratedEventHandler DataGenerated;

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
        //   * Boolean, constant and if statements have been simplified and
        //     reduced.
        //
        computations = 0;

        /****** %DECLS% ******/

        /****** %INIT% ******/

        /****** %CODE% ******/

        //
        // Now we copy the result data into an array that matches the
        // requested size.
        //
        var result = new /****** %OUTPUT_TYPE% ******/[width * height * depth];
        for (var k = z - __cz; k < (z - __cz) + depth; k++)
        for (var i = x - __cx; i < (x - __cx) + width; i++)
        for (var j = y - __cy; j < (y - __cy) + height; j++)
            result[
                (i - (x - __cx)) +
                (j - (y - __cy)) * width +
                (k - (z - __cz)) * width * height] =
                /****** %OUTPUT_VAR% ******/[i + j * __cwidth + k * __cwidth * __cheight];
        return result;
    }

    #endregion

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
}

