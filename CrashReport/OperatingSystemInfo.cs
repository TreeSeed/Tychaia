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
        // List of elements that we collect
        public string BuildNumber;
        public string Description;
        public string Name;
        public string OSArchitecture;
        public string Primary;
        public string TotalVisibleMemorySize;
        public string Version;

        // Formatted output
        public override string ToString()
        {
            if (this.Primary == "True")
            {
                string output = (this.Name != null && this.Name != "System.Object") ? "Name: " + this.Name + "\n" : string.Empty;
                output += (this.Description != null && this.Description != "System.Object") ? "Description: " + this.Description + "\n" : string.Empty;
                output += (this.OSArchitecture != null) ? "Architecture: " + this.OSArchitecture + "\n" : string.Empty;
                output += (this.Version != null) ? "Version: " + this.Version + "\n" : string.Empty;
                output += (this.BuildNumber != null) ? "Build Number: " + this.BuildNumber + "\n" : string.Empty;
                output += (this.TotalVisibleMemorySize != null) ? "Memory Size: " + this.TotalVisibleMemorySize + "\n" : string.Empty;
                return output;
            }
            else
                return string.Empty;
        }
    }
}
