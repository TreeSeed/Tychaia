// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrashReport
{
    public struct OperatingSystemInfo
    {
        public string BuildNumber;
        public string Description;
        public string Name;
        public string OperatingSystemSKU;
        public string OSArchitecture;
        public string Primary;
        public string TotalVisibleMemorySize;
        public string Version;
        // List of elements that we collect



        // Formatted output
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
