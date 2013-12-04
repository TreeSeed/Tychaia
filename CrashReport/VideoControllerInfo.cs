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
        public string AcceleratorCapabilities;
        public string AdapterRAM;
        public string ConfigManagerErrorCode;
        public string CurrentHorizontalResolution;
        public string CurrentVertialResolution;
        public string Description;
        public string DriverVersion;
        public string InstalledDisplayDrivers;
        public string MaxMemorySupported;
        public string Name;

        // Formatted output
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
