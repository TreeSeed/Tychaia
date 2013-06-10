//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;

namespace Tychaia.ProceduralGeneration
{
    public static class AlgorithmUtility
    {
        /// <summary>
        /// Returns a random positive integer between the specified 0 and
        /// the exclusive end value.
        /// </summary>
        public static int GetRandomRange(long seed, long x, long y, long z, int end, long modifier = 0)
        {
            unchecked
            {
                int a = AlgorithmUtility.GetRandomInt(seed, x, y, z, modifier);
                if (a < 0)
                    a += int.MaxValue;
                return a % end;
            }
        }

        /// <summary>
        /// Returns a random positive integer between the specified inclusive start
        /// value and the exclusive end value.
        /// </summary>
        public static int GetRandomRange(long seed, long x, long y, long z, int start, int end, long modifier)
        {
            unchecked
            {
                int a = AlgorithmUtility.GetRandomInt(seed, x, y, z, modifier);
                if (a < 0)
                    a += int.MaxValue;
                return a % (end - start) + start;
            }
        }

        /// <summary>
        /// Returns a random integer over the range of valid integers based
        /// on the provided X and Y position, and the specified modifier.
        /// </summary>
        public static int GetRandomInt(long seed, long x, long y, long z, long modifier = 0)
        {
            unchecked
            {
                return (int)(AlgorithmUtility.GetRandomNumber(seed, x, y, z, modifier) % int.MaxValue);
            }
        }

        /// <summary>
        /// Returns a random long integer over the range of valid long integers based
        /// on the provided X and Y position, and the specified modifier.
        /// </summary>
        public static long GetRandomLong(long seed, long x, long y, long z, long modifier = 0)
        {
            return AlgorithmUtility.GetRandomNumber(seed, x, y, z, modifier);
        }

        /// <summary>
        /// Returns a random double between the range of 0.0 and 1.0 based on
        /// the provided X and Y position, and the specified modifier.
        /// </summary>
        public static double GetRandomDouble(long seed, long x, long y, long z, long modifier = 0)
        {
            long a = AlgorithmUtility.GetRandomNumber(seed, x, y, z, modifier) / 2;
            if (a < 0)
                a += long.MaxValue;
            return (double)a / (double)long.MaxValue;
        }

        private static long GetRandomNumber(long _seed, long x, long y, long z, long modifier)
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
                seed += (x - 7) * _seed;
                seed *= (y - 8) * _seed;
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
                rng += seed * _seed;
                rng *= (y - 15) * _seed;
                rng *= (z - 16) * 18193477834921;
                rng += (x - 199) * (y - 241) * (z - 1471) * 9018110272013;
                rng += modifier;
                rng += 3661988493967;

                return rng;
            }
        }


        /// <summary>
        /// Smoothes the specified data according to smoothing logic.  Apparently
        /// inlining this functionality causes the algorithms to run slower, so we
        /// leave this function on it's own.
        /// </summary>
        public static int Smooth(
            long seed, bool isFuzzy,
            long x, long y,
            int northValue, int southValue, int westValue, int eastValue, int southEastValue,
            int currentValue,
            long i, long j, long ox, long oy, long rw, int[] parent)
        {
            // Parent-based Smoothing
            var selected = 0;

            if (x % 2 == 0)
            {
                if (y % 2 == 0)
                {
                    return currentValue;
                }
                else
                {
                    selected = GetRandomRange(seed, x, y, 0, 2);
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
                    selected = GetRandomRange(seed, x, y, 0, 2);
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
                        selected = GetRandomRange(seed, x, y, 0, 3);
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
                        selected = GetRandomRange(seed, x, y, 0, 4);
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
            selected = GetRandomRange(seed, x, y, 0, 4);

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
    }
}

