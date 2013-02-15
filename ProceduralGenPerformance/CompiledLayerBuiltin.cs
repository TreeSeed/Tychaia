//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Tychaia.ProceduralGeneration;

// Disable specific warnings since this is generated code.
#pragma warning disable 0162
#pragma warning disable 0429

public class CompiledLayerBuiltin : IRuntimeContext, IGenerator
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

        int width_dtpcbivq = ((width / 2) + 4);
        int height_woolstkv = ((height / 2) + 4);
        int width_hwyhgeun = ((width_dtpcbivq / 2) + 4);
        int height_mzdyhqfs = ((height_woolstkv / 2) + 4);
        int width_btjvczrj = ((width_hwyhgeun / 2) + 4);
        int height_rrmfunhd = ((height_mzdyhqfs / 2) + 4);
        int width_yqkgvbpk = ((width_btjvczrj / 2) + 4);
        int height_rwsggvym = ((height_rrmfunhd / 2) + 4);
        System.Int32[] xwznywnd = new System.Int32[width_yqkgvbpk * height_rwsggvym * depth];
        System.Int32[] yixzjsyc = new System.Int32[width_btjvczrj * height_rrmfunhd * depth];
        System.Int32[] hhgbkjup = new System.Int32[width_hwyhgeun * height_mzdyhqfs * depth];
        System.Int32[] wnvoavvn = new System.Int32[width_dtpcbivq * height_woolstkv * depth];
        System.Int32[] tehueofv = new System.Int32[width * height * depth];


        for (var k = 0; k < depth; k++)
            for (var i = 0; i < width_yqkgvbpk; i++)
                for (var j = 0; j < height_rwsggvym; j++)
                {
                    if (true && (x + i - 2) == 0L && (y + j - 2) == 0L)
                    {
                        xwznywnd[i + j * width_yqkgvbpk + k * width_yqkgvbpk * height_rwsggvym] = 1;
                    }
                    else
                    {
                        if (this.GetRandomDouble((x + i - 2), (y + j - 2), (z + k - 0), this.Modifier) > 0.9)
                        {
                            xwznywnd[i + j * width_yqkgvbpk + k * width_yqkgvbpk * height_rwsggvym] = 1;
                        }
                        else
                        {
                            xwznywnd[i + j * width_yqkgvbpk + k * width_yqkgvbpk * height_rwsggvym] = 0;
                        }
                    }
                }
        for (var k = 0; k < depth; k++)
            for (var i = 0; i < width_btjvczrj; i++)
                for (var j = 0; j < height_rrmfunhd; j++)
                {
                    int num = 2;
                    int num2 = 2;
                    long num3 = (long)(width_btjvczrj / 2 + num * 2);
                    bool flag = (x + i - 2) % 2L != 0L;
                    bool flag2 = (y + j - 2) % 2L != 0L;
                    int num4 = (!flag) ? 0 : (i % 2);
                    int num5 = (!flag2) ? 0 : (j % 2);
                    int num6 = (!flag) ? 0 : ((i - 1) % 2);
                    int num7 = (!flag) ? 0 : ((i + 1) % 2);
                    int num8 = (!flag2) ? 0 : ((j - 1) % 2);
                    int num9 = (!flag2) ? 0 : ((j + 1) % 2);
                    int num10;
                    int northValue;
                    int southValue;
                    int eastValue;
                    int westValue;
                    int southEastValue;
                    checked
                    {
                        num10 = xwznywnd[(int)((IntPtr)unchecked((long)(i / 2 + num + num4) + (long)(j / 2 + num2 + num5) * num3))];
                        northValue = xwznywnd[(int)((IntPtr)unchecked((long)(i / 2 + num + num4) + (long)((j - 1) / 2 + num2 + num8) * num3))];
                        southValue = xwznywnd[(int)((IntPtr)unchecked((long)(i / 2 + num + num4) + (long)((j + 1) / 2 + num2 + num9) * num3))];
                        eastValue = xwznywnd[(int)((IntPtr)unchecked((long)((i + 1) / 2 + num + num7) + (long)(j / 2 + num2 + num5) * num3))];
                        westValue = xwznywnd[(int)((IntPtr)unchecked((long)((i - 1) / 2 + num + num6) + (long)(j / 2 + num2 + num5) * num3))];
                        southEastValue = xwznywnd[(int)((IntPtr)unchecked((long)((i + 2) / 2 + num + num4) + (long)((j + 2) / 2 + num2 + num5) * num3))];
                    }
                    if (Tychaia.ProceduralGeneration.AlgorithmZoom.ZoomType.Smooth == AlgorithmZoom.ZoomType.Smooth || Tychaia.ProceduralGeneration.AlgorithmZoom.ZoomType.Smooth == AlgorithmZoom.ZoomType.Fuzzy)
                    {
                        yixzjsyc[i + j * width_btjvczrj] = this.Smooth(Tychaia.ProceduralGeneration.AlgorithmZoom.ZoomType.Smooth == AlgorithmZoom.ZoomType.Fuzzy, (x + i - 2) + (long)i, (y + j - 2) + (long)j, northValue, southValue, westValue, eastValue, southEastValue, num10, (long)i, (long)j, (long)num, (long)num2, num3, xwznywnd);
                    }
                    else
                    {
                        yixzjsyc[i + j * width_btjvczrj] = num10;
                    }
                }
        for (var k = 0; k < depth; k++)
            for (var i = 0; i < width_hwyhgeun; i++)
                for (var j = 0; j < height_mzdyhqfs; j++)
                {
                    int num = 2;
                    int num2 = 2;
                    long num3 = (long)(width_hwyhgeun / 2 + num * 2);
                    bool flag = (x + i - 2) % 2L != 0L;
                    bool flag2 = (y + j - 2) % 2L != 0L;
                    int num4 = (!flag) ? 0 : (i % 2);
                    int num5 = (!flag2) ? 0 : (j % 2);
                    int num6 = (!flag) ? 0 : ((i - 1) % 2);
                    int num7 = (!flag) ? 0 : ((i + 1) % 2);
                    int num8 = (!flag2) ? 0 : ((j - 1) % 2);
                    int num9 = (!flag2) ? 0 : ((j + 1) % 2);
                    int num10;
                    int northValue;
                    int southValue;
                    int eastValue;
                    int westValue;
                    int southEastValue;
                    checked
                    {
                        num10 = yixzjsyc[(int)((IntPtr)unchecked((long)(i / 2 + num + num4) + (long)(j / 2 + num2 + num5) * num3))];
                        northValue = yixzjsyc[(int)((IntPtr)unchecked((long)(i / 2 + num + num4) + (long)((j - 1) / 2 + num2 + num8) * num3))];
                        southValue = yixzjsyc[(int)((IntPtr)unchecked((long)(i / 2 + num + num4) + (long)((j + 1) / 2 + num2 + num9) * num3))];
                        eastValue = yixzjsyc[(int)((IntPtr)unchecked((long)((i + 1) / 2 + num + num7) + (long)(j / 2 + num2 + num5) * num3))];
                        westValue = yixzjsyc[(int)((IntPtr)unchecked((long)((i - 1) / 2 + num + num6) + (long)(j / 2 + num2 + num5) * num3))];
                        southEastValue = yixzjsyc[(int)((IntPtr)unchecked((long)((i + 2) / 2 + num + num4) + (long)((j + 2) / 2 + num2 + num5) * num3))];
                    }
                    if (Tychaia.ProceduralGeneration.AlgorithmZoom.ZoomType.Smooth == AlgorithmZoom.ZoomType.Smooth || Tychaia.ProceduralGeneration.AlgorithmZoom.ZoomType.Smooth == AlgorithmZoom.ZoomType.Fuzzy)
                    {
                        hhgbkjup[i + j * width_hwyhgeun] = this.Smooth(Tychaia.ProceduralGeneration.AlgorithmZoom.ZoomType.Smooth == AlgorithmZoom.ZoomType.Fuzzy, (x + i - 2) + (long)i, (y + j - 2) + (long)j, northValue, southValue, westValue, eastValue, southEastValue, num10, (long)i, (long)j, (long)num, (long)num2, num3, yixzjsyc);
                    }
                    else
                    {
                        hhgbkjup[i + j * width_hwyhgeun] = num10;
                    }
                }
        for (var k = 0; k < depth; k++)
            for (var i = 0; i < width_dtpcbivq; i++)
                for (var j = 0; j < height_woolstkv; j++)
                {
                    int num = 2;
                    int num2 = 2;
                    long num3 = (long)(width_dtpcbivq / 2 + num * 2);
                    bool flag = (x + i - 2) % 2L != 0L;
                    bool flag2 = (y + j - 2) % 2L != 0L;
                    int num4 = (!flag) ? 0 : (i % 2);
                    int num5 = (!flag2) ? 0 : (j % 2);
                    int num6 = (!flag) ? 0 : ((i - 1) % 2);
                    int num7 = (!flag) ? 0 : ((i + 1) % 2);
                    int num8 = (!flag2) ? 0 : ((j - 1) % 2);
                    int num9 = (!flag2) ? 0 : ((j + 1) % 2);
                    int num10;
                    int northValue;
                    int southValue;
                    int eastValue;
                    int westValue;
                    int southEastValue;
                    checked
                    {
                        num10 = hhgbkjup[(int)((IntPtr)unchecked((long)(i / 2 + num + num4) + (long)(j / 2 + num2 + num5) * num3))];
                        northValue = hhgbkjup[(int)((IntPtr)unchecked((long)(i / 2 + num + num4) + (long)((j - 1) / 2 + num2 + num8) * num3))];
                        southValue = hhgbkjup[(int)((IntPtr)unchecked((long)(i / 2 + num + num4) + (long)((j + 1) / 2 + num2 + num9) * num3))];
                        eastValue = hhgbkjup[(int)((IntPtr)unchecked((long)((i + 1) / 2 + num + num7) + (long)(j / 2 + num2 + num5) * num3))];
                        westValue = hhgbkjup[(int)((IntPtr)unchecked((long)((i - 1) / 2 + num + num6) + (long)(j / 2 + num2 + num5) * num3))];
                        southEastValue = hhgbkjup[(int)((IntPtr)unchecked((long)((i + 2) / 2 + num + num4) + (long)((j + 2) / 2 + num2 + num5) * num3))];
                    }
                    if (Tychaia.ProceduralGeneration.AlgorithmZoom.ZoomType.Smooth == AlgorithmZoom.ZoomType.Smooth || Tychaia.ProceduralGeneration.AlgorithmZoom.ZoomType.Smooth == AlgorithmZoom.ZoomType.Fuzzy)
                    {
                        wnvoavvn[i + j * width_dtpcbivq] = this.Smooth(Tychaia.ProceduralGeneration.AlgorithmZoom.ZoomType.Smooth == AlgorithmZoom.ZoomType.Fuzzy, (x + i - 2) + (long)i, (y + j - 2) + (long)j, northValue, southValue, westValue, eastValue, southEastValue, num10, (long)i, (long)j, (long)num, (long)num2, num3, hhgbkjup);
                    }
                    else
                    {
                        wnvoavvn[i + j * width_dtpcbivq] = num10;
                    }
                }
        for (var k = 0; k < depth; k++)
            for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                {
                    int num = 2;
                    int num2 = 2;
                    long num3 = (long)(width / 2 + num * 2);
                    bool flag = (x + i - 0) % 2L != 0L;
                    bool flag2 = (y + j - 0) % 2L != 0L;
                    int num4 = (!flag) ? 0 : (i % 2);
                    int num5 = (!flag2) ? 0 : (j % 2);
                    int num6 = (!flag) ? 0 : ((i - 1) % 2);
                    int num7 = (!flag) ? 0 : ((i + 1) % 2);
                    int num8 = (!flag2) ? 0 : ((j - 1) % 2);
                    int num9 = (!flag2) ? 0 : ((j + 1) % 2);
                    int num10;
                    int northValue;
                    int southValue;
                    int eastValue;
                    int westValue;
                    int southEastValue;
                    checked
                    {
                        num10 = wnvoavvn[(int)((IntPtr)unchecked((long)(i / 2 + num + num4) + (long)(j / 2 + num2 + num5) * num3))];
                        northValue = wnvoavvn[(int)((IntPtr)unchecked((long)(i / 2 + num + num4) + (long)((j - 1) / 2 + num2 + num8) * num3))];
                        southValue = wnvoavvn[(int)((IntPtr)unchecked((long)(i / 2 + num + num4) + (long)((j + 1) / 2 + num2 + num9) * num3))];
                        eastValue = wnvoavvn[(int)((IntPtr)unchecked((long)((i + 1) / 2 + num + num7) + (long)(j / 2 + num2 + num5) * num3))];
                        westValue = wnvoavvn[(int)((IntPtr)unchecked((long)((i - 1) / 2 + num + num6) + (long)(j / 2 + num2 + num5) * num3))];
                        southEastValue = wnvoavvn[(int)((IntPtr)unchecked((long)((i + 2) / 2 + num + num4) + (long)((j + 2) / 2 + num2 + num5) * num3))];
                    }
                    if (Tychaia.ProceduralGeneration.AlgorithmZoom.ZoomType.Smooth == AlgorithmZoom.ZoomType.Smooth || Tychaia.ProceduralGeneration.AlgorithmZoom.ZoomType.Smooth == AlgorithmZoom.ZoomType.Fuzzy)
                    {
                        tehueofv[i + j * width] = this.Smooth(Tychaia.ProceduralGeneration.AlgorithmZoom.ZoomType.Smooth == AlgorithmZoom.ZoomType.Fuzzy, (x + i - 0) + (long)i, (y + j - 0) + (long)j, northValue, southValue, westValue, eastValue, southEastValue, num10, (long)i, (long)j, (long)num, (long)num2, num3, wnvoavvn);
                    }
                    else
                    {
                        tehueofv[i + j * width] = num10;
                    }
                }


        return tehueofv;
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
        unchecked
        {
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
            
            long rng = seed * seed;
            rng += (x - 11) * 2990430311017;
            rng *= (y - 12) * 14475080218213;
            rng *= (z - 13) * 23281823741513;
            rng -= seed * 28124722524383;
            rng *= (x - 14) * 16099760261113;
            rng += seed * this.m_Seed;
            rng *= (y - 15) * this.m_Seed;
            rng *= (z - 16) * 18193477834921;
            rng += (x - 199) * (y - 241) * (z - 1471) * 9018110272013;
            rng += modifier;
            rng += 3661988493967;
            
            return rng;
        }
        /*
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
        */
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


