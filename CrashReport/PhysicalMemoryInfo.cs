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
    public struct PhysicalMemoryInfo
    {
        // List of elements that we collect
        public string Description;
        public string Manufacturer;
        public string Model;
        public string Name;

        // Formatted output
        public override string ToString()
        {
            string output = (this.Name != null && this.Name != "System.Object") ? "Name: " + this.Name + "\n" : string.Empty;
            output += (this.Description != null && this.Description != "System.Object") ? "Description: " + this.Description + "\n" : string.Empty;
            output += (this.Manufacturer != null && this.Manufacturer != "System.Object") ? "Manufacturer: " + this.Manufacturer + "\n" : string.Empty;
            output += (this.Model != null && this.Model != "System.Object") ? "Model: " + this.Model + "\n" : string.Empty;
            return output;
        }
    }
}
