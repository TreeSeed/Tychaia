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
    public struct ProcessorInfo
    {
        // List of elements that we collect
        public string Architecture;
        public uint? ConfigManagerErrorCode;
        public string Family;
        public string Manufacturer;
        public string MaxClockSpeed;
        public string Name;
        public string ProcessorType;
        public string Description;

        // Formatted output
        public override string ToString()
        {
            string output = (this.Name != null && this.Name != "System.Object") ? "Name: " + this.Name + "\n" : string.Empty;
            output += (this.Description != null && this.Description != "System.Object") ? "Description: " + this.Description + "\n" : string.Empty;
            output += (this.Architecture != null && this.Architecture != "System.Object") ? "Architecture: " + this.Architecture + "\n" : string.Empty;
            output += (this.Manufacturer != null && this.Manufacturer != "System.Object") ? "Manufacturer: " + this.Manufacturer + "\n" : string.Empty;
            output += (this.Family != null && this.Family != "System.Object") ? "Family: " + this.Family + "\n" : string.Empty;
            output += (this.ProcessorType != null && this.ProcessorType != "System.Object") ? "Processor Type: " + this.ProcessorType + "\n" : string.Empty;
            output += (this.MaxClockSpeed != null && this.MaxClockSpeed != "System.Object") ? "Max Clock Speed: " + this.MaxClockSpeed + "\n" : string.Empty;
            output += (this.ConfigManagerErrorCode != null && this.ConfigManagerErrorCode != 0) ? "Error Code: " + this.ConfigManagerErrorCode + "\n" : string.Empty;
            return output;
        }
    }
}
