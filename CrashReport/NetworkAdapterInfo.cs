﻿// ====================================================================== //
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
        public uint? ConfigManagerErrorCode;
        public string Description;
        public string Manufacturer;
        public string Name;
        public string ProductName;

        // Formatted output
        public override string ToString()
        {
            string output = (this.Name != null && this.Name != "System.Object") ? "Name: " + this.Name + "\n" : string.Empty;
            output += (this.Description != null && this.Description != "System.Object") ? "Description: " + this.Description + "\n" : string.Empty;
            output += (this.ProductName != null) ? "Product Name: " + this.ProductName + "\n" : string.Empty;
            output += (this.Manufacturer != null && this.Manufacturer != "System.Object") ? "Manufacturer: " + this.Manufacturer + "\n" : string.Empty;
            output += (this.ConfigManagerErrorCode != null && this.ConfigManagerErrorCode != 0) ? "Error Code: " + this.ConfigManagerErrorCode + "\n" : string.Empty;
            return output;
        }
    }
}
