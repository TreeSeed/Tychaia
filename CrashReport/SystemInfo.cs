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
    public struct SystemInfo
    {
        // List of elements that we collect
        public List<VideoControllerInfo> VideoControllers;
        public List<KeyboardInfo> Keyboards;
        public List<PointingDeviceInfo> PointingDevices;
        public List<DiskDriveInfo> DiskDrives;
        public List<NetworkAdapterInfo> NetworkAdapters;
        public List<PhysicalMemoryInfo> PhysicalMemory;
        public List<ProcessorInfo> Processors;
        public List<OperatingSystemInfo> OperatingSystems;

        // Formatted output
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
