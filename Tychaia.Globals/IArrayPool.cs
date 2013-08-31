// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;

namespace Tychaia.Globals
{
    public interface IArrayPool
    {
        T[] Get<T>(int size);
        dynamic Get(Type type, int size);
        void Release<T>(T[] array);
        void Release(dynamic array);
    }
}

