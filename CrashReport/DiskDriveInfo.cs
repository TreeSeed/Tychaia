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
    public struct DiskDriveInfo
    {
        // List of elements that we collect
        public string ConfigManagerErrorCode;
        public string Description;
        public string Name;
        public string MediaType;


        // Formatted output
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
