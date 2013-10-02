// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;

namespace Tychaia.Globals
{
    /// <summary>
    /// Use this attribute on highly-used methods or classes to prevent code coverage
    /// from analyising the function.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor)]
    public class NoInstrumentationAttribute : Attribute
    {
    }
}
