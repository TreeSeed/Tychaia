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
            return base.ToString();
        }
    }
}
