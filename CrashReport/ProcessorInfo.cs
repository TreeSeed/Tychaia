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
        public string ConfigManagerErrorCode;
        public string Family;
        public string Manufacturer;
        public string MaxClockSpeed;
        public string Name;
        public string ProcessorType;
        public string Description;


        // Formatted output
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
