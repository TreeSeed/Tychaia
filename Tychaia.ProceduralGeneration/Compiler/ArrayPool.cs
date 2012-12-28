//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Collections.Generic;

namespace Tychaia.ProceduralGeneration
{
    // FIXME: Support storing multiple free arrays of the
    // same size.
    public static class ArrayPool<T>
    {
        public static Dictionary<int, T[]> m_FreeArrays = new Dictionary<int, T[]>();

        public static T[] Grab(int size)
        {
            try
            {
                return m_FreeArrays[size];
            }
            catch (Exception)
            {
                return new T[size];
            }
        }

        public static void Release(T[] array)
        {
            m_FreeArrays[array.Length] = array;
        }
    }
}

