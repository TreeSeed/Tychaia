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
    public struct VideoControllerInfo
    {
        // List of elements that we collect
        public string AdapterRAM;
        public uint? ConfigManagerErrorCode;
        public uint? CurrentHorizontalResolution;
        public uint? CurrentVertialResolution;
        public string Description;
        public string DriverVersion;
        public string InstalledDisplayDrivers;
        public uint? MaxMemorySupported;
        public string Name;

        // Formatted output
        public override string ToString()
        {
            string output = (this.Name != null && this.Name != "System.Object") ? "Name: " + this.Name + "\n" : string.Empty;
            output += (this.Description != null && this.Description != "System.Object") ? "Description: " + this.Description + "\n" : string.Empty;
            output += (this.CurrentHorizontalResolution != null && this.CurrentVertialResolution != null) ? "Resolution: " + this.CurrentHorizontalResolution + "x" + this.CurrentVertialResolution + "\n" : string.Empty;
            output += (this.AdapterRAM != null) ? "Adapter RAM: " + this.AdapterRAM + "\n" : string.Empty;
            output += (this.MaxMemorySupported != null) ? "Max Memory Supported: " + this.MaxMemorySupported + "\n" : string.Empty;
            output += (this.DriverVersion != null) ? "Max Clock Speed: " + this.DriverVersion + "\n" : string.Empty;
            output += (this.InstalledDisplayDrivers != null) ? "Installed Display Drivers: " + this.InstalledDisplayDrivers + "\n" : string.Empty;
            output += (this.ConfigManagerErrorCode != null && this.ConfigManagerErrorCode != 0) ? "Error Code: " + this.ConfigManagerErrorCode + "\n" : string.Empty;
            return output;
        }
    }
}
