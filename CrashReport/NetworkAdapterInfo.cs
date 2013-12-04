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
    public struct NetworkAdapterInfo
    {
        // List of elements that we collect
        public string AdapterTypeID;
        public string ConfigManagerErrorCode;
        public string Description;
        public string Manufacturer;
        public string Name;
        public string NetConnectionID;
        public string NetConnectionStatus;
        public string NetEnabled;
        public string ProductName;


        // Formatted output
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
