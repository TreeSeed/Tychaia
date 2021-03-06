// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;

namespace Tychaia.Globals
{
    /// <summary>
    /// Use this attribute on highly-used classes (such as classes that provide sizing
    /// information) so that the profiler doesn't create massive overhead on operations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class NoProfileAttribute : Attribute
    {
    }
}
